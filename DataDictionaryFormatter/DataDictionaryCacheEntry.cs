using System;
using System.Collections.Generic;
using System.Text;
using ScrewTurn.Wiki.PluginFramework;

namespace Keeper.Garrett.ScrewTurn.DataDictionary
{
    public sealed class DataDictionaryCacheEntry
    {
        public PageInfo PageInfo { get; set; }
        public PageContent PageContent { get; set; }
        public SortedDictionary<string, List<string>> DictionaryEntries { get; set; }
    }
}
