using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScrewTurn.Wiki.PluginFramework;

namespace Keeper.Garrett.ScrewTurn.Utility
{
    public class CategoryTools
    {
        public static List<CategoryInfo> GetCategoryInformation(IHostV30 _host, IPagesStorageProviderV30 _provider, string _category, string _currentNS, string _ns)
        {
            var retval = new List<CategoryInfo>();
            //Get current NS
            var nsInfo = _host.FindNamespace(_currentNS);

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
                        namespaceInfo = _host.FindNamespace(ns);
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

        private static CategoryInfo GetCategoryFromNamespace(IPagesStorageProviderV30 _provider, NamespaceInfo _nsInfo, string _category)
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

                var categoryToFind = string.Format("{0}.{1}", _nsInfo.Name, _category);
                //Find the correct cat and add
                foreach (var cat in cats)
                {
                    if (cat.FullName == categoryToFind)
                    {
                        retval = cat;
                        break;
                    }
                }
            }

            return retval;
        }
    }
}
