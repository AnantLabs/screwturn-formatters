using System;
using System.Collections.Generic;
using System.Text;
using ScrewTurn.Wiki.PluginFramework;

namespace Keeper.Garrett.ScrewTurn.DataDictionary
{
    public sealed class DataDictionaryCache
    {
        public Dictionary<string, DataDictionaryCacheEntry> CachedPages { get { return m_Cache; } }

        private Dictionary<string, DataDictionaryCacheEntry> m_Cache = new Dictionary<string, DataDictionaryCacheEntry>();
        private bool m_EnableCache = false;

        public DataDictionaryCache(bool _enableCache, Dictionary<string, string> _dataDictionaries, IHostV30 _host, IPagesStorageProviderV30 _provider)
        {
            m_EnableCache = _enableCache;

            UpdateCache(m_EnableCache, _dataDictionaries,_host, _provider);
        }

        public void UpdateCache(bool _clearCache, Dictionary<string, string> _dictionaries, IHostV30 _host, IPagesStorageProviderV30 _provider)
        {
            //If caching is enabled
            if (m_EnableCache == true)
            {
                if (_clearCache == true)
                {
                    //Will force a reload of all pages, appropiate on a cleanup
                    m_Cache.Clear();
                }

                foreach (var dict in _dictionaries)
                {
                    //Dictionary cache empty?
                    if (m_Cache.ContainsKey(dict.Key) == false)
                    {
                        var entry = new DataDictionaryCacheEntry();
                        entry.PageInfo = DataDictionaryUtility.GetInfo(dict.Value,_host,_provider,this,_dictionaries);
                        entry.PageContent = DataDictionaryUtility.GetContent(entry.PageInfo,_provider,this,_dictionaries);
                        entry.DictionaryEntries = DataDictionaryUtility.ParseDictionaryPage(entry.PageContent.Content);

                        if (entry.PageInfo != null && entry.PageContent != null && entry.DictionaryEntries != null)
                        {
                            m_Cache.Add(dict.Key, entry);
                        }
                    }
                }
                _host.LogEntry(string.Format("DataDictionaryFormatter cache updated ({0} entries).", m_Cache.Count), LogEntryType.General, _host.GetCurrentUser().Username, this);
            }
        }

        public PageInfo GetPageInfo(string _pageName)
        {
            PageInfo page = null;
            if (m_EnableCache == true)
            {
                if (m_Cache.ContainsKey(_pageName) == true)
                {
                    page = m_Cache[_pageName].PageInfo;
                }
            }
            return page;
        }

        public PageContent GetPageContent(string _pageName)
        {
            PageContent page = null;
            if (m_EnableCache == true)
            {
                if (m_Cache.ContainsKey(_pageName) == true)
                {
                    page = m_Cache[_pageName].PageContent;
                }
            }
            return page;
        }

        public SortedDictionary<string, List<string>> GetPageEntries(string _pageName)
        {
            SortedDictionary<string, List<string>> page = null;
            if (m_EnableCache == true)
            {
                if (m_Cache.ContainsKey(_pageName) == true)
                {
                    page = m_Cache[_pageName].DictionaryEntries;
                }
            }
            return page;
        }

        public void UpdateCachePage(PageInfo _pageInfo, PageContent _content, string _newContent)
        {
            if (m_EnableCache == true)
            {
                if (m_Cache.ContainsKey(_pageInfo.FullName) == true)
                {
                    m_Cache[_pageInfo.FullName].PageInfo = _pageInfo;
                    m_Cache[_pageInfo.FullName].PageContent = new PageContent(_pageInfo, _content.Title, _content.User, DateTime.Now, _content.Comment, _newContent, _content.Keywords, _content.Description);
                    m_Cache[_pageInfo.FullName].DictionaryEntries = DataDictionaryUtility.ParseDictionaryPage(_newContent);
                }
            }
        }

        public void ClearCache()
        {
            m_Cache.Clear();
        }
    }
}
