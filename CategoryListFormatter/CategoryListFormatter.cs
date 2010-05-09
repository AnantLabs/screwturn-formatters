using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using System.Text.RegularExpressions;
using ScrewTurn.Wiki.PluginFramework;
using Keeper.Garrett.ScrewTurn.Utility;

namespace Keeper.Garrett.ScrewTurn.CategoryListFormatter
{
    public class CategoryListFormatter : FormatterBase
    {
        public override bool PerformPhase1 { get { return true; } }
        public override bool PerformPhase2 { get { return false; } }
        public override bool PerformPhase3 { get { return false; } }

        //Tag format {CategoryList(Category)}
        // Category,output,include,head,headers,tbl,head,row
        private static readonly Regex TagRegex = new Regex(@"\{CategoryList(?<arguments>(.*?))\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

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
                                    //Get formatting
                                    var columns = new List<string>();
                                    var headers = new List<string>();
                                    string style = "";
                                    bool includeSummary = false;

                                    var args = new ArgumentParser().Parse(match.Groups["arguments"].Value);

                                    var category = (args.ContainsKey("cat") == true ? args["cat"] : "");

                                    var outputType = (args.ContainsKey("type") == true ? args["type"] : "");
                                    outputType = (outputType != "*" && outputType != "#" && outputType != "" ? "*" : outputType);

                                    var header = (args.ContainsKey("head") == true ? args["head"] : "");
                                    var footer = (args.ContainsKey("foot") == true ? args["foot"] : "");

                                    //Parse columns to show
                                    if (args.ContainsKey("cols") == true)
                                    {
                                        var value = args["cols"];

                                        switch (value.ToLower())
                                        {
                                            case "all":
                                                //Handled by header setup
                                                includeSummary = true;
                                                break;
                                            default:
                                                var tmpColumns = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                foreach (var str in tmpColumns)
                                                {
                                                    columns.Add(str.ToLower());

                                                    if (str.ToLower() == "sum")
                                                    {
                                                        includeSummary = true;
                                                    }
                                                }
                                                break;
                                        };
                                    }

                                    //Parse custom headers to show
                                    if (args.ContainsKey("colnames") == true)
                                    {
                                        var tmpColumns = args["colnames"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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

                                    style = (args.ContainsKey("style") == true ? args["style"] : "");

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
                                            list = GenerateTableList(dict, includeSummary, header, footer, headers, style);
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

        private string GenerateTableList(SortedDictionary<string, PageContent> _list, bool _includeSummary, string _tblHeading, string _tblFooter, List<string> _headers, string _style)
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
            return Keeper.Garrett.ScrewTurn.Utility.XHtmlTableGenerator.GenerateTable(tableRowDict, _tblHeading, _tblFooter, new List<int>(), new List<string>(), _headers, _style);
        }

        private string GenerateLink(PageContent _content)
        {
            return string.Format("[{0}|{1}]", _content.PageInfo.FullName, _content.Title);
        }
    }
}
