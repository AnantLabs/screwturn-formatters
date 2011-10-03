using System;
using System.Collections.Generic;
using System.Text;
using ScrewTurn.Wiki.PluginFramework;

namespace Keeper.Garrett.ScrewTurn.DataDictionary
{
    public sealed class DataDictionaryUtility
    {
        public static bool CacheInvalid { get; private set; }

        public static void CheckCache(IHostV30 _host, IPagesStorageProviderV30 _provider, DataDictionaryCache _cache, Dictionary<string, string> _dictionaries)
        {
            if (CacheInvalid == true)
            {
                _cache.UpdateCache(true, _dictionaries, _host, _provider);
                CacheInvalid = false;
                _host.LogEntry("DataDictionaryFormatter cache invalidated, the refreshed.", LogEntryType.General, _host.GetCurrentUser().Username, new DataDictionaryUtility());
            }
        }

        public static PageInfo GetInfo(string _pageName, IHostV30 _host, IPagesStorageProviderV30 _provider, DataDictionaryCache _cache, Dictionary<string, string> _dictionaries)
        {
            var page = _cache.GetPageInfo(_pageName);

            if (page == null)
            {
                page = _host.FindPage(_pageName);

                //Create page if not allready present
                if (page == null)
                {
                    var ns = NameTools.GetNamespace(_pageName);

                    page = _provider.AddPage(ns, _pageName, DateTime.Now);

                    //Generate actual page content, etc
                    var content = GenerateNewDictionaryPageContent(_pageName, page, _host);

                    //Save page content
                    _provider.ModifyPage(page,
                        content.Title,
                        content.User,
                        content.LastModified,
                        content.Comment,
                        content.Content,
                        content.Keywords,
                        content.Description,
                        SaveMode.Normal);

                    //Invalidate cache
                    CacheInvalid = false;

                    _host.LogEntry(string.Format("DataDictionaryFormatter created Page {0}", content.Title), LogEntryType.General, content.User, new DataDictionaryUtility() );
                }
            }

            return page;
        }

        public static PageContent GetContent(PageInfo _pageInfo, IPagesStorageProviderV30 _provider, DataDictionaryCache _cache, Dictionary<string, string> _dictionaries)
        {
            var page = _cache.GetPageContent(_pageInfo.FullName);
            if (page == null)
            {
                page = _provider.GetContent(_pageInfo);
                //Invalidate cache
                CacheInvalid = false;
            }
            return page;
        }

        public static SortedDictionary<string, List<string>> GetEntries(PageContent _content, IPagesStorageProviderV30 _provider, DataDictionaryCache _cache, Dictionary<string, string> _dictionaries)
        {
            var entries = _cache.GetPageEntries(_content.PageInfo.FullName);

            if (entries == null)
            {
                entries = ParseDictionaryPage(_content.Content);

                //Invalidate cache
                CacheInvalid = false;
            }

            return entries;
        }

        public static SortedDictionary<string, List<string>> ParseDictionaryPage(string _content)
        {
            var lines = _content.Split(new char[] { '\n' });

            var retval = new SortedDictionary<string, List<string>>();

            var currentEntry = "";
            foreach (var line in lines)
            {
                //An entry beings?
                if (line.StartsWith("* ") == true)
                {
                    retval.Add(line, new List<string>());
                    currentEntry = line;
                }//A SubEntry
                else if (line.StartsWith("** ") == true)
                {
                    retval[currentEntry].Add(line);
                }
            }

            return retval;
        }

        public static string GeneratePageContent(PageContent _originalContent, SortedDictionary<string, List<string>> _content)
        {
            string retval = "";

            string header = _originalContent.Content.Substring(0, _originalContent.Content.IndexOf("----") + 4);
            string footer = _originalContent.Content.Substring(_originalContent.Content.LastIndexOf("----"));

            retval = string.Format("{0}", header);
            foreach (var entry in _content)
            {
                retval = string.Format("{0}\n{1}", retval, entry.Key);

                var subEntries = entry.Value;
                subEntries.Sort();//Ensure subentries are in sorted order

                foreach (var subEntry in subEntries) //Ensure subentries are in sorted order
                {
                    retval = string.Format("{0}\n{1}", retval, subEntry);
                }

                retval = string.Format("{0}\n", retval);
            }
            retval = string.Format("{0}\n{1}", retval, footer);

            return retval;
        }

        private static PageContent GenerateNewDictionaryPageContent(string _pageName, PageInfo _pageInfo, IHostV30 _host)
        {
            string content = string.Format("The complete list of DataDictionary entries filed under {0}.\n----\n----", _pageName);
            var user = _host.GetCurrentUser().Username;

            PageContent newPage = new PageContent(_pageInfo,
                                                    _pageName,
                                                    user,
                                                    DateTime.Now,
                                                    string.Format("Generated by DataDictionaryFormatter due to entry made by {0}.", user),
                                                    content,
                                                    new string[] { _pageInfo.FullName, "Dictionary", "DataDictionary" },
                                                    string.Format("Generated by DataDictionaryFormatter due to entry made by {0}.", user));

            return newPage;
        }
    }
}
