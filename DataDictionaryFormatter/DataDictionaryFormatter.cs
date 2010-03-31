using System;
using System.Collections.Generic;
using System.Text;
using ScrewTurn.Wiki.PluginFramework;
using System.Text.RegularExpressions;
using ScrewTurn.Wiki.SearchEngine;
using Keeper.Garrett.ScrewTurn.Core;

namespace Keeper.Garrett.ScrewTurn.DataDictionary
{
    public class DataDictionaryFormatter : FormatterBase
    {
        public override bool PerformPhase1 { get { return true; } }
        public override bool PerformPhase2 { get { return false; } }
        public override bool PerformPhase3 { get { return false; } }

        //Tag format {NameSpace(default to root)#UniqueIdentifier#EntryToFileUnder#Summary(Optional)}
        private static readonly Regex TagRegex = new Regex(@"\{(?<key>(.*?))#(?<entry>(.*?))#(?<summary>(.*?))\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
        private static readonly Regex TagCmdRegex = new Regex(@"\{DataDictionary#(?<cmd>(.*?))\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
        private static readonly Regex TagLinkRegex = new Regex(@"\[(?<link>(.*?))\]", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture);

        private Dictionary<string, string> m_DataDictionaryOverviewPages = new Dictionary<string, string>();
        private IPagesStorageProviderV30 m_Provider = null;
        private bool m_EnableCache = false;
        private DataDictionaryCache m_Cache = null;

        private Dictionary<string,DataDictionaryCacheEntry> m_PageTransactions = new Dictionary<string,DataDictionaryCacheEntry>();

        private readonly object m_PadLock = new object();

        public override void Init(IHostV30 _host, string _config)
        {
            try
            {
                m_Provider = _host.GetPagesStorageProviders(true)[0];
                ParseConfigurationString(_config);
                m_Cache = new DataDictionaryCache(m_EnableCache, m_DataDictionaryOverviewPages, _host, m_Provider);

                _host.LogEntry("DataDictionaryFormatter - Init success", LogEntryType.General, _host.GetCurrentUser().Username, this);
            }
            catch (Exception e)
            {
                _host.LogEntry(string.Format("DataDictionaryFormatter - Init failure: {0}", string.Format("{0}\r\n{1}", e.Message, e.StackTrace)), LogEntryType.Error, _host.GetCurrentUser().Username, this);
            }

            base.Init(_host, _config);
        }

        private void ParseConfigurationString(string _config)
        {
            m_DataDictionaryOverviewPages.Clear();
            m_Config = null;

            if (string.IsNullOrEmpty(_config) == false)
            {
                var lines = _config.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var dbEntry in lines)
                {
                    var tokens = dbEntry.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                    if (tokens.Length == 2 && tokens[0] != "EnableCache")
                    {
                        m_DataDictionaryOverviewPages.Add(tokens[0], tokens[1].Replace("\r", "").Replace("\n", "").Trim());
                    }

                    if (tokens.Length == 2 && tokens[0] == "EnableCache")
                    {
                        m_EnableCache = bool.TryParse(tokens[1].Trim().ToLower(), out m_EnableCache);
                    }
                }
            }

            if(m_DataDictionaryOverviewPages.Count == 0)
            {
                //Use default page
                m_DataDictionaryOverviewPages.Add("Dict", "DataDictionary");
            }
        }

        public override string ConfigHelpHtml
        {
            get
            {
                return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}",
                    "<p>Usage: MyKey=PageName</p>",
                    "<p>Where:</p>",
                    "<p> - MyKey is the key you write in your tag to tell the formatter to add/update the entry on page PageName.</p>",
                    "<p> - PageName, name of the page which will be the dictionary (if exist's, wont do anything).</p>",
                    "<p>1. Tag format is: {Key#DictEntry#Summary}</p>",
                    "<p> - Key</p>",
                    "<p> - DictEntry</p>",
                    "<p> - Summary  (Optional)</p>",
                    "<p>2. Tag format is: {DataDictionary#Command}</p>",
                    "<p> - DataDictionary - Always 'DataDictionary')</p>",
                    "<p> - Command - Supported commands are:</p>",
                    "<p>              'Clean' - (long time, cleans all dead entries by scan, provided for admin maintanance)</p>",
                    "<p>Usage: EnableCache=true/false - to enable dictionary caching for better performance</p>"
                    );
            }
        }

        public override string Format(string raw, ContextInformation context, FormattingPhase phase)
        {
            try
            {
                if (context.Context == FormattingContext.PageContent
                    && context.ForIndexing == false
                    && context.ForWysiwyg == false)
                {
                    switch (phase)
                    {
                        case FormattingPhase.Phase1:
                            //Save current page for when we link
                            var currentPage = context.Page;

                            var matches = TagRegex.Matches(raw);
                            if (matches != null && matches.Count > 0)
                            {
                                lock (m_PadLock)
                                {
                                    //Run through all matches
                                    foreach (Match match in matches)
                                    {
                                        var key = match.Groups["key"].Value;
                                        //If there is a dictionary for this match
                                        if (m_DataDictionaryOverviewPages.ContainsKey(key) == true)
                                        {
                                            //Fetch data from match
                                            var ns = NameTools.GetNamespace(currentPage.FullName);// (string.IsNullOrEmpty(match.Groups["namespace"].Value) ? NameTools.GetNamespace(currentPage.FullName) : match.Groups["namespace"].Value);
                                            var entry = match.Groups["entry"].Value;
                                            string summary = (match.Groups.Count >= 3 ? match.Groups["summary"].Value : "");

                                            //Add/Update entry
                                            if (AddDictionaryEntry(currentPage, ns, key, entry, summary) == true)
                                            {
                                                //Remove the tag so that it is not visible to the user
                                                raw = raw.Replace(TagRegex.Match(raw).Value, "");
                                            }
                                        }
                                        //Do nothing there is no key in the know DD pages for this match
                                    }

                                    CommitTransaction();
                                }
                            }

                            //Scan for commands
                            matches = TagCmdRegex.Matches(raw);
                            if (matches != null && matches.Count > 0)
                            {
                                foreach (Match match in matches)
                                {
                                    var cmd = match.Groups["cmd"].Value;

                                    HandleCommandRequest(cmd, currentPage);
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
                m_Host.LogEntry(string.Format("DataDictionaryFormatter error: {0} {1}", e.Message, e.StackTrace), LogEntryType.Error, m_Host.GetCurrentUser().Username, this);
            }

            return raw;
        }

        private bool AddDictionaryEntry(PageInfo _currentPage, string _ns, string _key, string _entry, string _summary)
        {
            /*
                   Entry format, expected.
                   ----
                   * Entry 1
                   ** SubEntry 1
                   ** SubEntry 2
                   * Entry 2
                   ** SubEntry 1
                   ----
             */
            DataDictionaryUtility.CheckCache(m_Host, m_Provider, m_Cache, m_DataDictionaryOverviewPages);//Always check cache before using it

            var pageInfo = DataDictionaryUtility.GetInfo(m_DataDictionaryOverviewPages[_key],m_Host,m_Provider,m_Cache,m_DataDictionaryOverviewPages);
            var pageContent = DataDictionaryUtility.GetContent(pageInfo, m_Provider, m_Cache, m_DataDictionaryOverviewPages);
            var entries = DataDictionaryUtility.GetEntries(pageContent,m_Provider,m_Cache,m_DataDictionaryOverviewPages);

            var subEntryLink = string.Format("[{0}]", _currentPage.FullName);

            var entryKey = string.Format("* '''{0}'''", _entry);
            //Entry allready present?
            if (entries.ContainsKey(entryKey) == false)
            {
                //No -> Add it
                entries.Add(entryKey, new List<string>());
            }

            //Check if there's allready a link
            foreach (var line in entries[entryKey])
            {
                //Link allready there?
                if (line.Contains(subEntryLink) == true)
                {
                    entries[entryKey].Remove(line);
                    break;
                }
            }

            //Add the new subentry
            _summary = (string.IsNullOrEmpty(_summary) == true ? "" : string.Format(" - ''{0}''", _summary));
            entries[entryKey].Add(string.Format("** [{0}]{1}", _currentPage.FullName, _summary));

            //Generate the new content
            var content = DataDictionaryUtility.GeneratePageContent(pageContent, entries);

            //Save page
            return AddPageToTransaction(pageInfo, pageContent, content);
//            return m_Provider.ModifyPage(pageInfo, pageContent.Title, pageContent.User, DateTime.Now, pageContent.Comment, content, pageContent.Keywords, pageContent.Description, SaveMode.Normal);
        }

        private bool AddPageToTransaction(PageInfo _pageInfo, PageContent _content, string _newContent)
        {
            //Update cache
            m_Cache.UpdateCachePage(_pageInfo,_content,_newContent);

            //Update transaction
            var pageUpdate = new DataDictionaryCacheEntry();
            pageUpdate.PageInfo = _pageInfo;
            pageUpdate.PageContent = new PageContent(_pageInfo, _content.Title, _content.User, DateTime.Now, _content.Comment, _newContent, _content.Keywords, _content.Description);
            pageUpdate.DictionaryEntries = DataDictionaryUtility.ParseDictionaryPage(_newContent);

            if (m_PageTransactions.ContainsKey(_pageInfo.FullName) == false)
            {
                m_PageTransactions.Add(_pageInfo.FullName,pageUpdate);
            }

            m_PageTransactions[_pageInfo.FullName] = pageUpdate;

            return true;
        }

        private void CommitTransaction()
        {
            foreach (var entry in m_PageTransactions)
            {
                m_Provider.ModifyPage(  entry.Value.PageInfo,
                                        entry.Value.PageContent.Title,
                                        entry.Value.PageContent.User,
                                        entry.Value.PageContent.LastModified,
                                        entry.Value.PageContent.Comment,
                                        entry.Value.PageContent.Content,
                                        entry.Value.PageContent.Keywords,
                                        entry.Value.PageContent.Description,
                                        SaveMode.Normal
                                        );
            }

            m_PageTransactions.Clear();
        }

        private void HandleCommandRequest(string _cmd, PageInfo _currentPage)
        {
            switch (_cmd.ToLower())
            {
                case "clean":
                    RemoveDeadLinks(_currentPage);
                    //Clear cache if enabled, will force a reload of all pages, appropiate on a cleanup
                    lock (m_PadLock)
                    {
                        m_Cache.UpdateCache(true, m_DataDictionaryOverviewPages, m_Host, m_Provider);
                    }
                    break;
            };
        }

        private void RemoveDeadLinks(PageInfo _currentPage)
        {
            //Scan all known dictionaries
            foreach (var dictPage in m_DataDictionaryOverviewPages)
            {
                var page = m_Host.FindPage(dictPage.Value);
                if (page != null)
                {
                    string removedEntries = "";

                    var pageContent = m_Provider.GetContent(page);

                    var entries = DataDictionaryUtility.ParseDictionaryPage(pageContent.Content);

                    var newEntries = ScanPage(dictPage.Key, entries, ref removedEntries);

                    //Generate the new content
                    var content = DataDictionaryUtility.GeneratePageContent(pageContent, newEntries);

                    //Save page
                    m_Provider.ModifyPage(pageContent.PageInfo, pageContent.Title, pageContent.User, DateTime.Now, pageContent.Comment, content, pageContent.Keywords, pageContent.Description, SaveMode.Normal);
                }
            }
        }

        private SortedDictionary<string, List<string>> ScanPage(string _dictKey, SortedDictionary<string, List<string>> _entries, ref string _removedSubEntries)
        {
            var entriesToRemove = new List<string>();

            foreach (var entry in _entries)
            {
                //Generate "clean" entry by removing *,',whitespace,\n
                var parsedEntry = entry.Key.Replace("*", "").Replace("'", "").Replace("\n", "").Trim();

                var subEntriesToRemove = new List<String>();
                foreach (var subEntry in entry.Value)
                {
                    var pageLink = GetPageFromSubEntry(subEntry);

                    if (string.IsNullOrEmpty(pageLink) == false)
                    {
                        var page = m_Host.FindPage(pageLink);
                        var pageContent = m_Provider.GetContent(page);

                        if (RemoveEntry(page, pageContent, _dictKey, parsedEntry) == true)
                        {
                            subEntriesToRemove.Add(subEntry);
                        }
                    }
                }

                foreach (var subEntry in subEntriesToRemove)
                {
                    entry.Value.Remove(subEntry);
                    _removedSubEntries = string.Format("{0},{1}", _removedSubEntries, entry);
                }

                //Register empty entries
                if (entry.Value.Count == 0)
                {
                    entriesToRemove.Add(entry.Key);
                }
            }

            //Remove empty entrie
            foreach (var entry in entriesToRemove)
            {
                _entries.Remove(entry);
            }

            return _entries;
        }

        private bool RemoveEntry(PageInfo _info, PageContent _content, string _dictKey, string _entry)
        {
            bool retval = true;
            var matches = TagRegex.Matches(_content.Content);

            if (matches != null && matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Groups["key"].Value == _dictKey //Correct dictionary
                        &&
                        match.Groups["entry"].Value == _entry)//Correct entry
                    {
                        retval = false;
                        break;
                    }
                }
            }

            return retval;
        }

        private string GetPageFromSubEntry(string _subEntry)
        {
            string retval = null;

            var matches = TagLinkRegex.Matches(_subEntry);
            if (matches != null && matches.Count > 0)
            {
                retval = matches[0].Groups["link"].Value;
            }

            return retval;
        }
    }
}
