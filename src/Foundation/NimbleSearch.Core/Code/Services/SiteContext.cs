using NimbleSearch.Foundation.Abstractions.Services;
using NimbleSearch.Foundation.Core.Util;
using Sitecore.Data.Items;
using Sitecore.Sites;

namespace NimbleSearch.Foundation.Core.Services
{
    public class SiteContextService : ISiteContext
    {
        public virtual SiteContext GetSiteContext(Item item)
        {
            var itemPath = item?.Paths.FullPath;
            if (string.IsNullOrEmpty(itemPath))
                return null;

            itemPath = itemPath.EnsurePrefix("/").EnsureSuffix("/").ToUpperInvariant();

            foreach (var site in SiteContextFactory.Sites)
            {
                // skip system sites
                if (Abstractions.Constants.SystemSites.IndexOf(site.Name) >= 0)
                    continue;

                // skip sites without a path
                if (site.RootPath.Length == 0)
                    continue;

                var startpath = site.RootPath.EnsurePrefix("/").EnsureSuffix("/").ToUpperInvariant();
                // not adding +site.StartItem; -- so it can match for shared content items too
                if (itemPath.StartsWith(startpath))
                {
                    return SiteContextFactory.GetSiteContext(site.Name);
                }

            }
            return null;
        }
    }
}