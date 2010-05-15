using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using System.Text.RegularExpressions;
using ScrewTurn.Wiki.PluginFramework;
using ScrewTurn.Wiki;
using System.Linq;
using System.IO;
using Keeper.Garrett.ScrewTurn.Utility;

namespace Keeper.Garrett.ScrewTurn.FileContentFormatter
{
    public class FileContentFormatter : FormatterBase
    {
        public override bool PerformPhase1 { get { return true; } }
        public override bool PerformPhase2 { get { return false; } }
        public override bool PerformPhase3 { get { return false; } }

        //Tag format {FileCont args}
        private static readonly Regex TagRegex = new Regex(@"\{FileCont(?<arguments>(.*?))\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        public override void Init(IHostV30 _host, string _config)
        {
            base.Init(_host, _config, 55, Help.HelpPages);

            LogEntry("FileContentFormatter - Init success", LogEntryType.General);
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
                                    IFilesStorageProviderV30 stoProvider = providers[providers.Length - 1]; 
                                    string path = string.Empty;
                                    string filePattern = string.Empty;
                                    string storageProvider = string.Empty;
                                    string style = string.Empty;

                                    var args = new ArgumentParser().Parse(match.Groups["arguments"].Value);

                                    //Get args
                                    string tmpPattern = (args.ContainsKey("file") == true ? args["file"] : "/*.*");
                                    path = tmpPattern.Substring(0, tmpPattern.LastIndexOf('/') + 1);
                                    filePattern = tmpPattern.Substring(tmpPattern.LastIndexOf('/') + 1);

                                    storageProvider = (args.ContainsKey("prov") == true ? args["prov"].ToLower() : "");

                                    //Customization/styling
                                    string height = (args.ContainsKey("height") == true ? args["height"].ToLower() : "");
                                    string width = (args.ContainsKey("width") == true ? args["width"].ToLower() : "");
                                    if (string.IsNullOrEmpty(height) == false) style = string.Format("height=\"{0}\"", height);
                                    if (string.IsNullOrEmpty(width) == false) style = (string.IsNullOrEmpty(style) == true ? string.Format("width=\"{0}\"", width) : string.Format("{0} width=\"{1}\"", style, width));

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
                                    var fileList = FindFiles(stoProvider, path, filePattern, ref list);

                                    //Generate content
                                    var content = GenerateContent(stoProvider, fileList, style);

                                    //Override only if there something there
                                    if (string.IsNullOrEmpty(content) == false)
                                    {
                                        list = content;
                                    }

                                    //Add a final newline
                                    list = string.Format("{0}", list);

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
                LogEntry(string.Format("FileContentFormatter error: {0} {1}", e.Message, e.StackTrace), LogEntryType.Error);
            }

            return raw;
        }

        private List<string> FindFiles(IFilesStorageProviderV30 _provider, string _path, string _filePattern,ref string _list)
        {
            var retval = new List<string>();

            if (_provider != null)
            {
                try
                {
                    var files = _provider.ListFiles(_path);

                    //Create usable regex from wildcard
                    var regex = Regex.Escape(_filePattern).Replace(@"\*", ".*").Replace(@"\?", ".");

                    foreach (var file in files)
                    {
                        //Add only if pattern match
                        if (Regex.IsMatch(file, regex) == true)
                        {
                            retval.Add(file);
                        }
                    }
                }
                catch (Exception e)
                {
                    var tmp = e.Message;
                    _list = string.Format("(No directory found matching \"{0}\".)", _path);
                }
            }

            return retval;
        }

        private string GenerateContent(IFilesStorageProviderV30 _provider, List<string> _files, string _style)
        {
            string retval = "";

            if (_files.Count > 0)
            {
                foreach (var file in _files)
                {
                    var ms = new MemoryStream();

                    string fileContent = string.Empty;

                    try
                    {
                        //Embedding
                        if (string.IsNullOrEmpty(_style) == false)
                        {
                            fileContent = string.Format(@"<embed src=""GetFile.aspx?File=/{0}"" {1} />", file, _style);

                        }//Raw insertion
                        else if (_provider.RetrieveFile(file, ms, false) == true)
                        {
                            var reader = new StreamReader(ms);
                            ms.Position = 0;

                            fileContent = reader.ReadToEnd();
                            reader.Close();
                        }

                        //Only add if theres something there
                        if (string.IsNullOrEmpty(fileContent) == false)
                        {
                            if (string.IsNullOrEmpty(retval) == false)
                            {
                                retval = string.Format("{0} \n {1}", retval, fileContent);
                            }
                            else
                            {
                                retval = fileContent;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogEntry(string.Format("FileContentFormatter error parsing file: {0} - {1} {2}", file, e.Message, e.StackTrace), LogEntryType.Error);
                    }
                    finally
                    {
                        ms.Close();
                    }
                }
            }

            return retval;
        }
    }
}
