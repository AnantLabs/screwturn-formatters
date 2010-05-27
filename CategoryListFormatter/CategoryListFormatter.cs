using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using System.Text.RegularExpressions;
using ScrewTurn.Wiki.PluginFramework;
using Keeper.Garrett.ScrewTurn.Utility;
using System.Reflection;
using System.Web;

namespace Keeper.Garrett.ScrewTurn.CategoryListFormatter
{
    public class CategoryListFormatter : FormatterBase
    {
        public override bool PerformPhase1 { get { return true; } }
        public override bool PerformPhase2 { get { return false; } }
        public override bool PerformPhase3 { get { return false; } }

        private Dictionary<string, int> m_ColumnDictionary = new Dictionary<string, int>()
        {
            { "comment", 0 },
            { "summary", 1 },
            { "keywords", 2 },
            { "lastmodified", 3 },
            { "linkedpages", 4 },
            { "createtime", 5 },
            { "pagename", 6 },
            { "user", 7 },
            { "creator", 8 }
        };

        private List<string> m_ColumnNames = new List<string>() { "Comment", "Summary", "Keywords", "Last Modified", "Linked Pages", "Created", "Page name", "Last Modified By", "Created By" };
        private string m_DefaultColumNames = "pagename";
        private string m_DateTimeFormat = "";

        private static readonly Regex TagRegex = new Regex(@"\{CategoryList(?<arguments>(.*?))\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        public override void Init(IHostV30 _host, string _config)
        {
            base.Init(_host, _config, 55, Help.HelpPages);

            StoreFiles(_host);

            LogEntry("CategoryListFormatter - Init success", LogEntryType.General);
        }

        private void StoreFiles(IHostV30 _host)
        {
            try
            {
                XHtmlTableGenerator.StoreFiles(_host, "CategoryListFormatter");
            }
            catch (Exception e)
            {
                LogEntry(string.Format("CategoryListFormatter - StoreFiles - Error: {0}",e.Message), LogEntryType.Error);
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
                                //Get current provider
                                var provider = context.Page.Provider;

                                //Foreach query
                                foreach (Match match in matches)
                                {   
                                    //Get formatting
                                    var columns = new List<string>();
                                    var headers = new List<string>();
                                    string style = "";

                                    var args = new ArgumentParser().Parse(match.Groups["arguments"].Value);

                                    var category = (args.ContainsKey("cat") == true ? args["cat"] : "");

                                    var outputType = (args.ContainsKey("type") == true ? args["type"] : "");//Default is table
                                    outputType = (outputType != "*" && outputType != "#" && outputType != "table" ? "*" : outputType);

                                    var ns = (args.ContainsKey("ns") == true ? args["ns"] : "");

                                    var head = (args.ContainsKey("head") == true ? args["head"] : "");
                                    var foot = (args.ContainsKey("foot") == true ? args["foot"] : "");

                                    var cols = (args.ContainsKey("cols") == true ? args["cols"] : m_DefaultColumNames);
                                    var colnames = (args.ContainsKey("colnames") == true ? args["colnames"] : "");
                                    var newCols = new List<int>();
                                    var newColNames = new List<string>();

                                    XHtmlTableGenerator.GenerateColumnsAndColumnNames(m_ColumnDictionary, m_ColumnNames, m_DefaultColumNames, cols, colnames, out newCols, out newColNames);

                                    m_DateTimeFormat = m_Host.GetSettingValue(SettingName.DateTimeFormat); //Update datetime format

                                    style = (args.ContainsKey("style") == true ? args["style"] : "");

                                    //Get info from database
                                    var dict = new SortedDictionary<string, PageDescription>(StringComparer.InvariantCultureIgnoreCase);
                                
                                    if(string.IsNullOrEmpty(category) == false && provider != null)
                                    {
                                        var currentNs = NameTools.GetNamespace(context.Page.FullName);
                                        var catInfos = CategoryTools.GetCategoryInformation(m_Host, provider, category, currentNs, ns);

                                        foreach(var catInfo in catInfos)
                                        {
                                            foreach (var page in catInfo.Pages)
                                            {
                                                var pageInfo = m_Host.FindPage(page);
                                                if (pageInfo != null)
                                                { 
                                                    var content = new PageDescription();
                                                    content.Content = m_Host.GetPageContent(pageInfo);

                                                    if(newCols.Contains(8) == true) //Are they asking for creator, make another lookup (this is optimization, only make a lookup whne required)
                                                    {
                                                        var revs = provider.GetBackups(pageInfo);

                                                        if(revs != null && revs.Length > 0)
                                                        {
                                                            content.CreatorName = provider.GetBackupContent(pageInfo, 0).User;
                                                        }
                                                        else
                                                        {
                                                            content.CreatorName = content.Content.User;
                                                        }

                                                        var user = m_Host.FindUser(content.CreatorName);
                                                        if (user != null)
                                                        {
                                                            content.CreatorDisplayName = user.DisplayName;
                                                        }
                                                    }

                                                    //Build dict
                                                    if (content.Content != null)
                                                    {
                                                        var user = m_Host.FindUser(content.Content.User);
                                                        if (user != null)
                                                        {
                                                            content.UserName = user.Username;
                                                            content.UserDisplayName = user.DisplayName;
                                                        }

                                                        //Key must be unique
                                                        dict.Add(string.Format("{0} - {1}",content.Content.Title,content.Content.PageInfo.FullName), content);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    string list = string.Format("(((No category name \"{0}\" found or an error occured.)))", category);

                                    if (dict.Count > 0)
                                    {
                                        if (outputType != "table") //Use primitive style?
                                        {
                                            list = GeneratePrimitiveList(dict, outputType, newCols);
                                        }
                                        else
                                        {
                                            list = GenerateTableList(dict, head, foot, newCols, newColNames, style);
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
                LogEntry(string.Format("CategoryListFormatter error: {0} {1}", e.StackTrace, e.StackTrace), LogEntryType.Error);
            }

            return raw;
        }

        private List<CategoryInfo> GetCategoryInformation(IPagesStorageProviderV30 _provider, string _category, string _currentNS, string _ns)
        {
            var retval = new List<CategoryInfo>();
            //Get current NS
            var nsInfo = m_Host.FindNamespace(_currentNS);

            //No override -> use current ns
            if (string.IsNullOrEmpty(_ns) == true)
            {
                var cat = GetCategoryFromNamespace(_provider, nsInfo, _category);
                if (cat != null)
                {
                    retval.Add(cat);
                }
            }
            else
            {
                var namespaces = _ns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var ns in namespaces)
                {
                    NamespaceInfo namespaceInfo = null;
                    if (ns.ToLower() != "root") //Root is alwyas null
                    {
                        namespaceInfo = m_Host.FindNamespace(ns);
                    }

                    var cat = GetCategoryFromNamespace(_provider, namespaceInfo, _category);

                    if (cat != null)
                    {
                        retval.Add(cat);
                    }
                }
            }

            return retval;
        }

        private CategoryInfo GetCategoryFromNamespace(IPagesStorageProviderV30 _provider, NamespaceInfo _nsInfo, string _category)
        {
            CategoryInfo retval = null;

            //If root we are done allready
            if (_nsInfo == null)
            {
                retval = _provider.GetCategory(_category);
            }
            else
            {
                //Get all cats for the ns
                var cats = _provider.GetCategories(_nsInfo);

                //Find the correct cat and add
                foreach (var cat in cats)
                {
                    if (cat.FullName == _category)
                    {
                        retval = cat;
                        break;
                    }
                }
            }

            return retval;
        }

        private string GeneratePrimitiveList(SortedDictionary<string, PageDescription> _list, string _outputType, List<int> _cols)
        {
            string retval = "";

            foreach (var entry in _list)
            {
                retval = string.Format("{0} \n{1}", retval, _outputType);

                retval = string.Format("{0} {1}", retval, GetField(entry.Value, _cols[0]));

                for(int i = 1; i < _cols.Count; i++)
                {
                    retval = string.Format("{0} - {1}", retval, GetField(entry.Value, _cols[i]));
                }
            }

            return retval;
        }

        private string GenerateTableList(SortedDictionary<string, PageDescription> _list, string _tblHeading, string _tblFooter, List<int> _cols, List<string> _colNames, string _style)
        {
            var tableRowDict = new Dictionary<int, List<string>>();
            int i = 0;
            foreach (var entry in _list)
            {
                tableRowDict.Add(i++, new List<string>());
                
                //Get all data fields
                for(int j = 0; j < m_ColumnNames.Count; j++)
                {
                    tableRowDict[i - 1].Add(GetField(entry.Value, j));
                }
            }

            //Generate table
            return Keeper.Garrett.ScrewTurn.Utility.XHtmlTableGenerator.GenerateTable(tableRowDict, _tblHeading, _tblFooter, _cols, _colNames, m_ColumnNames, _style);
        }

        private string GetField(PageDescription _page, int _col)
        {
            string retval = "";

            switch(_col)
            {
                case 0://comment
                    retval = _page.Content.Comment;
                    break;
                case 1://description
                    retval = _page.Content.Description;
                    break;
                case 2://keywords
                    foreach (var word in _page.Content.Keywords)
                    {
                        retval = string.Format("{0},{1}",retval, word);
                    }
                    if (retval.Length > 0 && retval[0] == ',')
                    {
                        retval = retval.Remove(0, 1);
                    }
                    break;
                case 3://lastmodified
                    retval = _page.Content.LastModified.ToString(m_DateTimeFormat);
                    break;
                case 4://linkedpages
                    foreach (var link in _page.Content.LinkedPages)
                    {
                        retval = string.Format("{0},[{1}]", retval, link);
                    }
                    if (retval.Length > 0 && retval[0] == ',')
                    {
                        retval = retval.Remove(0, 1);
                    }
                    break;
                case 5://createtime
                    retval = _page.Content.PageInfo.CreationDateTime.ToString(m_DateTimeFormat);
                    break;
                case 6://title
                    retval = string.Format("[{0}|{1}]", _page.Content.PageInfo.FullName, _page.Content.Title);
                    break;
                case 7://user
                    retval = string.Format("[User.aspx?Username={0}|{1}]", _page.UserName, _page.UserDisplayName); ;
                    break;
                case 8://creator
                    retval = string.Format("[User.aspx?Username={0}|{1}]", _page.CreatorName, _page.CreatorDisplayName); ;
                    break;
            }
            return retval;
        }
    }
}
