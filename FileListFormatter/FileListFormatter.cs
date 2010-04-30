using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using System.Text.RegularExpressions;
using ScrewTurn.Wiki.PluginFramework;
using ScrewTurn.Wiki;
using System.Linq;

namespace Keeper.Garrett.ScrewTurn.FileListFormatter
{
    public class FileListFormatter : FormatterBase
    {
        public override bool PerformPhase1 { get { return true; } }
        public override bool PerformPhase2 { get { return false; } }
        public override bool PerformPhase3 { get { return false; } }

        private enum SortOrder
        {
            FilenameAsc = 0,
            FilenameDesc = 1,
            DownloadsAsc = 2,
            DownloadsDesc = 3,
            FileSizeAsc = 4,
            FileSizeDesc = 5,
            FileModDateAsc = 6,
            FileModDateDesc = 7
        }

        private enum Details
        {
            None = 0,
            Downloads = 1,
            DownloadsAndSize = 2,
            DownloadsAndModDate = 3,
            DownloadsAndSizeAndModDate = 4,
            Size = 5,
            SizeAndModDate = 6,
            ModDate = 7
        }

        //Tag format {CategoryList(Category)}
        // Category,output,include,head,headers,tbl,head,row                                                                                                                        
        private static readonly Regex TagRegex = new Regex(@"\{FileList\(('(?<filePattern>(.*?))'),('(?<storageProvider>(.*?))')?,(?<outputType>(.*?))?,(?<sortMethod>(.*?))?,(?<asLinks>(.*?))?,(?<showDetails>(.*?))?,('(?<heading>(.*?))')?,('(?<headers>(.*?))')?,('(?<tblFormat>(.*?))')?,('(?<headFormat>(.*?))')?,('(?<rowFormat>(.*?))')?\)\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        private static string m_DateTimeFormat = "";

        public override void Init(IHostV30 _host, string _config)
        {
            base.Init(_host, _config, 55, Help.HelpPages);

            LogEntry("FileListFormatter - Init success", LogEntryType.General);
        }

        public override string Format(string raw, ContextInformation context, FormattingPhase phase)
        {
            try
            {
                if(    context.Context == FormattingContext.PageContent
                    && context.ForIndexing == false
                    && context.ForWysiwyg == false)
                {
                    switch (phase)
                    {
                        case FormattingPhase.Phase1:
                            var matches = TagRegex.Matches(raw);
                            if (matches != null && matches.Count > 0)
                            {
                                //Get current file providers
                                var providers = m_Host.GetFilesStorageProviders(true);

                                if (providers.Length <= 0)
                                {
                                    throw new Exception("No active FileStorageProviders found!");
                                }

                                //Foreach query
                                foreach (Match match in matches)
                                {
                                    var defaultProvider = m_Host.GetSettingValue(SettingName.DefaultFilesStorageProvider).ToLower();
                                    IFilesStorageProviderV30 stoProvider = providers[0];
                                    string path = string.Empty;
                                    string filePattern = string.Empty;
                                    string storageProvider = string.Empty;
                                    string outputType = string.Empty;
                                    bool asLinks = false;
                                    int showDetails = 0;
                                    int sortMethod = 7;

                                    //Get params 
                                    string tmpPattern = (string.IsNullOrEmpty(match.Groups["filePattern"].Value) == true ? "/*.*" : match.Groups["filePattern"].Value);
                                    path = tmpPattern.Substring(0,tmpPattern.LastIndexOf('/') + 1);
                                    filePattern = tmpPattern.Substring(tmpPattern.LastIndexOf('/') + 1);

                                    storageProvider = (string.IsNullOrEmpty(match.Groups["storageProvider"].Value) == true ? defaultProvider : match.Groups["storageProvider"].Value).ToLower();
                                    bool.TryParse(match.Groups["asLinks"].Value, out asLinks);

                                    int.TryParse(match.Groups["showDetails"].Value, out showDetails);
                                    showDetails = (showDetails < 0 || showDetails > 7 ? 0 : showDetails);

                                    int.TryParse(match.Groups["sortMethod"].Value, out sortMethod);
                                    sortMethod = (sortMethod < 0 || sortMethod > 7 ? 7 : sortMethod);

                                    m_DateTimeFormat = m_Host.GetSettingValue(SettingName.DateTimeFormat); //Update datetime format

                                    //Get formatting
                                    var columns = new List<int>();
                                    var headers = new List<string>();

                                    outputType = (string.IsNullOrEmpty(match.Groups["outputType"].Value) == false ? match.Groups["outputType"].Value.Trim() : "");
                                    outputType = (outputType != "*" && outputType != "#" && outputType != "table" ? "*" : outputType); //Force table if wrong type

                                    var heading = (string.IsNullOrEmpty(match.Groups["heading"].Value) == false ? match.Groups["heading"].Value.Trim() : "");

                                    //Parse custom headers to show
                                    if (string.IsNullOrEmpty(match.Groups["headers"].Value) == false)
                                    {
                                        var tmpColumns = match.Groups["headers"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach (var str in tmpColumns)
                                        {
                                            headers.Add(str);
                                        }
                                    }
 
                                    //Setup table formatting, default to system/wiki theme if no override present
                                    var tblFormat = (string.IsNullOrEmpty(match.Groups["tblFormat"].Value) == false ? match.Groups["tblFormat"].Value.Trim() : "");
                                    var headFormat = (string.IsNullOrEmpty(match.Groups["headFormat"].Value) == false ? match.Groups["headFormat"].Value.Trim() : "");
                                    var rowFormat = (string.IsNullOrEmpty(match.Groups["rowFormat"].Value) == false ? match.Groups["rowFormat"].Value.Trim() : "");

                                    //Find matching provider, use default if none
                                    foreach (var prov in providers)
                                    {
                                        if (prov.Information.Name.ToLower() == storageProvider)
                                        {
                                            stoProvider = prov;
                                            break;
                                        }//Else if default provider
                                        else if (prov.Information.Name.ToLower() == defaultProvider)
                                        {
                                            stoProvider = prov;
                                        }
                                    }

                                    string list = string.Format("(No files found matching \"{0}\".)", filePattern);

                                    //Get info from database
                                    var fileList = new Dictionary<string,FileDetails>();

                                    if (stoProvider != null)
                                    {
                                        try
                                        {
                                            var files = stoProvider.ListFiles(path);

                                            //Create usable regex from wildcard
                                            var regex = Regex.Escape(filePattern).Replace(@"\*", ".*").Replace(@"\?", ".");

                                            foreach (var file in files)
                                            {
                                                //Add only if pattern match
                                                if (Regex.IsMatch(file, regex) == true)
                                                {
                                                    fileList.Add(file, stoProvider.GetFileDetails(file));
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            list = string.Format("(No directory found matching \"{0}\".)", path);
                                        }
                                    }

                                    if (fileList.Count > 0)
                                    {
                                        fileList = SortFiles(fileList,sortMethod);

                                        if (outputType != "table") //Use primitive style?
                                        {
                                            list = GeneratePrimitiveList(fileList, outputType, asLinks, showDetails);
                                        }
                                        else
                                        {
                                            list = GenerateTableList(fileList, asLinks, showDetails, heading, headers, tblFormat, headFormat, rowFormat);
                                        }
                                    }

                                    //Add a final newline
                                    list = string.Format("{0} \n", list);

                                    //Insert list
                                    //Recall position as string may allready have been modified by other table match entry
                                    int pos = TagRegex.Match(raw).Index;
                                    int length = TagRegex.Match(raw).Length;
                                    raw = raw.Remove(pos, length);
                                    raw = raw.Insert(pos, list);

                                }
                            }
                            break;
                        case FormattingPhase.Phase2:
                            break;
                        case FormattingPhase.Phase3:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                LogEntry(string.Format("FileListFormatter error: {0} {1}", e.Message, e.StackTrace), LogEntryType.Error);
            }

            return raw;
        }

        private Dictionary<string, FileDetails> SortFiles(Dictionary<string, FileDetails> _filesToSort, int _sortMethod)
        {
            var retval = new Dictionary<string,FileDetails>();

            switch ((SortOrder)_sortMethod)
	        {
		        case SortOrder.FilenameAsc:
                    retval = _filesToSort.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case SortOrder.FilenameDesc:
                    retval = _filesToSort.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case SortOrder.DownloadsAsc:
                    retval = _filesToSort.OrderBy(x => x.Value.RetrievalCount).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case SortOrder.DownloadsDesc:
                    retval = _filesToSort.OrderByDescending(x => x.Value.RetrievalCount).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case SortOrder.FileSizeAsc:
                    retval = _filesToSort.OrderBy(x => x.Value.Size).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case SortOrder.FileSizeDesc:
                    retval = _filesToSort.OrderByDescending(x => x.Value.Size).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case SortOrder.FileModDateAsc:
                    retval = _filesToSort.OrderBy(x => x.Value.LastModified).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case SortOrder.FileModDateDesc:
                    retval = _filesToSort.OrderByDescending(x => x.Value.LastModified).ToDictionary(x => x.Key, x => x.Value);
                    break;
            };

            return retval;
        }

        #region List
        private string GeneratePrimitiveList(Dictionary<string, FileDetails> _listOfFiles, string _outputType, bool _asLinks, int _showDetails)
        {
            string retval = "";

            foreach (var file in _listOfFiles)
            {
                retval = string.Format("{0} \n{1} {2} {3}",
                     retval,
                     _outputType,
                     (_asLinks == true ? GenerateLink(file.Key) : file.Key.Substring(file.Key.LastIndexOf("/") + 1)),
                     GetPrimitiveDetails(file.Value,_showDetails) );
            }

            return retval;
        }

        private string GetPrimitiveDetails(FileDetails _file, int _showDetails)
        {
            var retval = "";

            switch ((Details)_showDetails)
            {
                case Details.None:
                    break;
                case Details.Downloads:
                    retval = string.Format("({0} downloads)", _file.RetrievalCount);
                    break;
                case Details.DownloadsAndSize:
                    retval = string.Format("({0}, {1} downloads)", GetFileSize(_file.Size), _file.RetrievalCount);
                    break;
                case Details.DownloadsAndModDate:
                    retval = string.Format("({0}, {1} downloads)", _file.LastModified.ToString(m_DateTimeFormat), _file.RetrievalCount);
                    break;
                case Details.DownloadsAndSizeAndModDate:
                    retval = string.Format("({0}, {1}, {2} downloads)", _file.LastModified.ToString(m_DateTimeFormat), GetFileSize(_file.Size), _file.RetrievalCount);
                    break;
                case Details.Size:
                    retval = string.Format("({0})", GetFileSize(_file.Size));
                    break;
                case Details.SizeAndModDate:
                    retval = string.Format("({0}, {1})", _file.LastModified.ToString(m_DateTimeFormat), GetFileSize(_file.Size));
                    break;
                case Details.ModDate:
                    retval = string.Format("({0})", _file.LastModified.ToString(m_DateTimeFormat));
                    break;
            }

            return retval;
        }

        #endregion

        #region Table

        private string GenerateTableList(Dictionary<string, FileDetails> _list, bool _asLinks, int _showDetails, string _tblHeading, List<string> _headers, string _tblFormat, string _headFormat, string _rowFormat)
        {
            var tableRowDict = new Dictionary<int, List<string>>();
            int i = 0;

            //Generate table header if missing
            if(_headers.Count <= 0)
            {
                _headers = GetDefaultTableHeaders(_showDetails);
            }

            foreach(var file in _list)
            {
                tableRowDict.Add(i, new List<string>());

                //Filename
                tableRowDict[i].Add( (_asLinks == true ? GenerateLink(file.Key) : file.Key.Substring(file.Key.LastIndexOf("/") + 1)));

                //Details
                switch ((Details)_showDetails)
                {
                    case Details.None:
                        break;
                    case Details.Downloads:
                        tableRowDict[i].Add(file.Value.RetrievalCount.ToString());
                        break;
                    case Details.DownloadsAndSize:
                        tableRowDict[i].Add(GetFileSize(file.Value.Size));
                        tableRowDict[i].Add(file.Value.RetrievalCount.ToString());
                        break;
                    case Details.DownloadsAndModDate:
                        tableRowDict[i].Add(file.Value.LastModified.ToString(m_DateTimeFormat));
                        tableRowDict[i].Add(file.Value.RetrievalCount.ToString());
                        break;
                    case Details.DownloadsAndSizeAndModDate:
                        tableRowDict[i].Add(file.Value.LastModified.ToString(m_DateTimeFormat));
                        tableRowDict[i].Add(GetFileSize(file.Value.Size));
                        tableRowDict[i].Add(file.Value.RetrievalCount.ToString());
                        break;
                    case Details.Size:
                        tableRowDict[i].Add(GetFileSize(file.Value.Size));
                        break;
                    case Details.SizeAndModDate:
                        tableRowDict[i].Add(file.Value.LastModified.ToString(m_DateTimeFormat));
                        tableRowDict[i].Add(GetFileSize(file.Value.Size));
                        break;
                    case Details.ModDate:
                        tableRowDict[i].Add(file.Value.LastModified.ToString(m_DateTimeFormat));
                        break;
                };

                i++;
            }

            //Generate table
            return Keeper.Garrett.ScrewTurn.Utility.TableGenerator.GenerateTable(tableRowDict, _tblHeading, new List<int>(), new List<string>(), _headers, _tblFormat, _headFormat, _rowFormat);
        }

        private List<string> GetDefaultTableHeaders(int _showDetails)
        {
            var retval = new List<string>();

            retval.Add("Filename");

            switch ((Details)_showDetails)
            {
                case Details.None:
                    break;
                case Details.Downloads:
                    retval.Add("Downloads");
                    break;
                case Details.DownloadsAndSize:
                    retval.Add("Size");
                    retval.Add("Downloads");
                    break;
                case Details.DownloadsAndModDate:
                    retval.Add("Last Modified");
                    retval.Add("Downloads");
                    break;
                case Details.DownloadsAndSizeAndModDate:
                    retval.Add("Last Modified");
                    retval.Add("Size");
                    retval.Add("Downloads");
                    break;
                case Details.Size:
                    retval.Add("Size");
                    break;
                case Details.SizeAndModDate:
                    retval.Add("Last Modified");
                    retval.Add("Size");
                    break;
                case Details.ModDate:
                    retval.Add("Last Modified");
                    break;
            }

            return retval;
        }

        #endregion

        private string GenerateLink(string _file)
        {
            return string.Format("[GetFile.aspx?File={0}|{1}]", _file.Replace("/","%2f"), _file.Substring(_file.LastIndexOf("/") + 1));
        }

        private string GetFileSize(long _size)
        {
            var retval = "";
            if (_size >= 1000)
            {
                retval = string.Format("{0} KB", Math.Round((decimal)(_size / 1000)));
            }
            else
            {
                retval = string.Format("{0} B", _size);
            }
            return retval;
        }
    }
}
