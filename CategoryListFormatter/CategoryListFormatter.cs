using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using System.Text.RegularExpressions;
using ScrewTurn.Wiki.PluginFramework;

namespace Keeper.Garrett.ScrewTurn.CategoryListFormatter
{
    public class CategoryListFormatter : FormatterBase
    {
        public override bool PerformPhase1 { get { return true; } }
        public override bool PerformPhase2 { get { return false; } }
        public override bool PerformPhase3 { get { return false; } }

        //Tag format {CategoryList(Category)}
        // Category,output,include,head,headers,tbl,head,row
        private static readonly Regex TagRegex = new Regex(@"\{CategoryList\((?<category>(.*?)),(?<outputtype>(.*?)),(?<includesummary>(.*?)),('(?<heading>(.*?))')?,('(?<headers>(.*?))')?,('(?<tblFormat>(.*?))')?,('(?<headFormat>(.*?))')?,('(?<rowFormat>(.*?))')?\)\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        public override void Init(IHostV30 _host, string _config)
        {
            base.Init(_host, _config, 55, Help.HelpPages);

            LogEntry("CategoryListFormatter - Init success", LogEntryType.General);
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
                                //Get current provider
                                var provider = context.Page.Provider;

                                //Foreach query
                                foreach (Match match in matches)
                                {   
                                    bool includeSummary = false;

                                    var category = (string.IsNullOrEmpty(match.Groups["category"].Value) == false ? match.Groups["category"].Value.Trim() : "");

                                    //Get formatting
                                    var columns = new List<int>();
                                    var headers = new List<string>();

                                    var outputType = (string.IsNullOrEmpty(match.Groups["outputtype"].Value) == false ? match.Groups["outputtype"].Value.Trim() : "");
                                    outputType = (outputType != "*" && outputType != "#" && outputType != "" ? "*" : outputType);

                                    bool.TryParse(match.Groups["includesummary"].Value, out includeSummary);

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
                                    else
                                    {
                                        headers.Add("Page name");
                                        if (includeSummary == true)
                                        {
                                            headers.Add("Description");
                                        }
                                    }

                                    //Setup table formatting, default to system/wiki theme if no override present
                                    var tblFormat = (string.IsNullOrEmpty(match.Groups["tblFormat"].Value) == false ? match.Groups["tblFormat"].Value.Trim() : "");
                                    var headFormat = (string.IsNullOrEmpty(match.Groups["headFormat"].Value) == false ? match.Groups["headFormat"].Value.Trim() : "");
                                    var rowFormat = (string.IsNullOrEmpty(match.Groups["rowFormat"].Value) == false ? match.Groups["rowFormat"].Value.Trim() : "");


                                    //Get info from database
                                    var dict = new SortedDictionary<string,PageContent>(StringComparer.InvariantCultureIgnoreCase);
                                
                                    if(string.IsNullOrEmpty(category) == false && provider != null)
                                    {
                                        var catInfo = provider.GetCategory(category);

                                        if (catInfo != null)
                                        {
                                            foreach (var page in catInfo.Pages)
                                            {
                                                var pageInfo = m_Host.FindPage(page);
                                                if (pageInfo != null)
                                                {
                                                    var content = m_Host.GetPageContent(pageInfo);
                                                    //Build dict
                                                    if (content != null)
                                                    {
                                                        dict.Add(content.Title, content);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    string list = string.Format("(((No category name \"{0}\" found or an error occured.)))", category);

                                    if (dict.Count > 0)
                                    {
                                        if (outputType != "") //Use primitive style?
                                        {
                                            list = GeneratePrimitiveList(dict, outputType, includeSummary);
                                        }
                                        else
                                        {
                                            list = GenerateTableList(dict, includeSummary, heading, headers, tblFormat, headFormat, rowFormat);
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
                LogEntry(string.Format("CategoryListFormatter error: {0} {1}", e.Message, e.StackTrace), LogEntryType.Error);
            }

            return raw;
        }

        private string GeneratePrimitiveList(SortedDictionary<string, PageContent> _list, string _outputType, bool _includeSummary)
        {
            string retval = "";

            if (_includeSummary == true)
            {
                foreach (var entry in _list)
                {
                    retval = string.Format("{0} \n{1} {2} - {3}",
                        retval,
                        _outputType,
                        GenerateLink(entry.Value),
                        (string.IsNullOrEmpty(entry.Value.Description) == true ? "''Missing summary''" : entry.Value.Description) );
                }
            }
            else
            {
                foreach (var entry in _list)
                {
                    retval = string.Format("{0} \n{1} {2}",
                        retval,
                        _outputType,
                        GenerateLink(entry.Value));
                }
            }

            return retval;
        }

        private string GenerateTableList(SortedDictionary<string, PageContent> _list, bool _includeSummary, string _tblHeading, List<string> _headers, string _tblFormat, string _headFormat, string _rowFormat)
        {
            var tableRowDict = new Dictionary<int, List<string>>();
            int i = 0;
            foreach(var entry in _list)
            {
                if(_includeSummary)
                {
                    tableRowDict.Add(i++, new List<string>() { 
                        GenerateLink(entry.Value), 
                        (string.IsNullOrEmpty(entry.Value.Description) == true ? "''Missing summary''" : entry.Value.Description) 
                    });
                }
                else
                {
                    tableRowDict.Add(i++,new List<string>() { GenerateLink(entry.Value) });
                }
            }

            if (_includeSummary == false && _headers.Count > 0)
            {
                _headers = new List<string>() { _headers[0] };
            }

            //Generate table
            return Keeper.Garrett.ScrewTurn.Utility.TableGenerator.GenerateTable(tableRowDict, _tblHeading, new List<int>(), new List<string>(), _headers, _tblFormat, _headFormat, _rowFormat);
        }

        private string GenerateLink(PageContent _content)
        {
            return string.Format("[{0}|{1}]", _content.PageInfo.FullName, _content.Title);
        }
    }
}
