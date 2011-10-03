using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScrewTurn.Wiki.PluginFramework;

namespace Keeper.Garrett.ScrewTurn.BlogFormatter
{
    class BlogPostInfo
    {
        public PageContent Content { get; set; }
        public int NoOfComments { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public string UserGravatar { get; set; }
    }
}
