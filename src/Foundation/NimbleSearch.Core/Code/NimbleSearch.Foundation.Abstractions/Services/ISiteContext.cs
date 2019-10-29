using Sitecore.Data.Items;
using Sitecore.Sites;

namespace NimbleSearch.Foundation.Abstractions.Services
{
    public interface ISiteContext
    {
        SiteContext GetSiteContext(Item item);

    }
}