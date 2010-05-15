using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using System.Text.RegularExpressions;
using ScrewTurn.Wiki.PluginFramework;
using ScrewTurn.Wiki;
using System.Linq;
using Keeper.Garrett.ScrewTurn.Utility;

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
            Size = 2,
            DownloadsAndSize = 3,
            ModDate = 4,
            DownloadsAndModDate = 5,
            SizeAndModDate = 6,
            DownloadsAndSizeAndModDate = 7
        }

        private Dictionary<string, int> m_ColumnDictionary = new Dictionary<string, int>()
        {
            { "name", 0 },
            { "downloads", 1 },
            { "size", 2 },
            { "date", 3 }
        };

        private List<string> m_ColumnNames = new List<string>() { "Name", "Downloads", "Size", "Last Modified" };
        private string m_DefaultColumNames = "name";
        private string m_DateTimeFormat = "";

        private static readonly Regex TagRegex = new Regex(@"\{FileList(?<arguments>(.*?))\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        public override void Init(IHostV30 _host, string _config)
        {
            base.Init(_host, _config, 55, Help.HelpPages);

            StoreFiles(_host);

            LogEntry("FileListFormatter - Init success", LogEntryType.General);
        }

        private void StoreFiles(IHostV30 _host)
        {
            try
            {
                XHtmlTableGenerator.StoreFiles(_host, "FileListFormatter");
            }
            catch (Exception e)
            {
                LogEntry(string.Format("FileListFormatter - StoreFiles - Error: {0}", e.Message), LogEntryType.Error);
            }
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
                                    //Get arguments
                                    var args = new ArgumentParser().Parse(match.Groups["arguments"].Value);

                                    IFilesStorageProviderV30 stoProvider = providers[providers.Length - 1]; 
                                    bool dwnl = true;
                                    int sortMethod = 7;
                                    int details = 0;

                                    //Pattern
                                    var tmpPattern = (args.ContainsKey("file") == true ? args["file"] : "/*.*");
                                    var path = tmpPattern.Substring(0,tmpPattern.LastIndexOf('/') + 1);
                                    var filePattern = tmpPattern.Substring(tmpPattern.LastIndexOf('/') + 1);

                                    var storageProvider = (args.ContainsKey("prov") == true ? args["prov"].ToLower() : "");

                                    var outputType = (args.ContainsKey("type") == true ? args["type"] : "");//Default is *
                                    outputType = (outputType != "*" && outputType != "#" && outputType != "table" ? "*" : outputType);

                                    bool.TryParse((args.ContainsKey("dwnl") == true ? args["dwnl"] : "true"), out dwnl);

                                    details = (args.ContainsKey("details") == true ? ParseDetails(args["details"]) : 0);
                                    sortMethod = (args.ContainsKey("sort") == true ? ParseSortOrder(args["sort"]) : 7);

                                    //Formatting and style
                                    var head = (args.ContainsKey("head") == true ? args["head"] : "");
                                    var foot = (args.ContainsKey("foot") == true ? args["foot"] : "");

                                    var cols = (args.ContainsKey("cols") == true ? args["cols"] : m_DefaultColumNames);
                                    var colnames = (args.ContainsKey("colnames") == true ? args["colnames"] : "");
                                    var newCols = new List<int>();
                                    var newColNames = new List<string>();
                                    XHtmlTableGenerator.GenerateColumnsAndColumnNames(m_ColumnDictionary, m_ColumnNames, m_DefaultColumNames, cols, colnames, out newCols, out newColNames);

                                    //Always insert name if not there
                                    if (newCols.Contains(0) == false)
                                    {
                                        newCols.Insert(0, 0);
                                    }

                                    var style = (args.ContainsKey("style") == true ? args["style"] : "");

                                    m_DateTimeFormat = m_Host.GetSettingValue(SettingName.DateTimeFormat); //Update datetime format

                                    //Find matching provider, use default if none
                                    foreach (var prov in providers)
                                    {
                                        if (prov.Information.Name.ToLower() == storageProvider)
                                        {
                                            stoProvider = prov;
                                            break;
                                        }
                                        //Else if default provider is allready set
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
                                            var tmp = e.Message;
                                            list = string.Format("(No directory found matching \"{0}\".)", path);
                                        }
                                    }

                                    if (fileList.Count > 0)
                                    {
                                        fileList = SortFiles(fileList,sortMethod);

                                        if (outputType != "table") //Use primitive style?
                                        {
                                            list = GeneratePrimitiveList(fileList, outputType, dwnl, details);
                                        }
                                        else
                                        {
                                            list = GenerateTableList(fileList, dwnl, head, foot, newCols, newColNames, style);
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

        private int ParseSortOrder(string _order)
        {
            int retval = (int)SortOrder.FileModDateDesc;
            var orderKeys = _order.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (orderKeys != null && orderKeys.Length >= 2)
            {
                int order = 0;
                for(int i = 0; i < orderKeys.Length; i++)
                {
                    switch (orderKeys[i].ToLower())
                    {
                        case "name":
                            order += 0;
                            break;
                        case "downloads":
                            order += 2;
                            break;
                        case "size":
                            order += 4;
                            break;
                        case "date":
                            order += 6;
                            break;
                        case "asc":
                            order += 0;
                            break;
                        case "desc":
                            order += 1;
                            break;
                    }
                }

                if(order > 7)
                {
                    order = 7;
                }

                retval = order;
            }

            return retval;
        }

        private int ParseDetails(string _details)
        {
            int retval = (int)SortOrder.FileModDateDesc;
            var detailKeys = _details.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (detailKeys != null && detailKeys.Length > 0)
            {
                int details = 0;
                for (int i = 0; i < detailKeys.Length; i++)
                {
                    switch (detailKeys[i].ToLower())
                    {
                        case "downloads":
                            details += 1;
                            break;
                        case "size":
                            details += 2;
                            break;
                        case "date":
                            details += 4;
                            break;
                    }
                }

                if (details > 7)
                {
                    details = 0;
                }

                retval = details;
            }

            return retval;
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

        private string GenerateTableList(Dictionary<string, FileDetails> _list, bool _asLinks, string _tblHeading, string _tblFooter, List<int> _cols, List<string> _colNames, string _style)
        {
            var tableRowDict = new Dictionary<int, List<string>>();
            int i = 0;

            foreach(var file in _list)
            {
                tableRowDict.Add(i, new List<string>());

                //Filename
                tableRowDict[i].Add( (_asLinks == true ? GenerateLink(file.Key) : file.Key.Substring(file.Key.LastIndexOf("/") + 1)));
                //Downloads
                tableRowDict[i].Add(file.Value.RetrievalCount.ToString());
                //Size
                tableRowDict[i].Add(GetFileSize(file.Value.Size));
                //Date
                tableRowDict[i].Add(file.Value.LastModified.ToString(m_DateTimeFormat));

                i++;
            }

            //Generate table
            return Keeper.Garrett.ScrewTurn.Utility.XHtmlTableGenerator.GenerateTable(tableRowDict, _tblHeading, _tblFooter, _cols, _colNames, m_ColumnNames, _style);
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
