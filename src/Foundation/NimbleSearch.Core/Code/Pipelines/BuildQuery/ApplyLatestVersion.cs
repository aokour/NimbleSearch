using NimbleSearch.Foundation.Abstractions.Models.Search;
using NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery;
using Sitecore.ContentSearch.Linq;
using System.Linq;

namespace NimbleSearch.Foundation.Core.Pipelines.BuildQuery
{
    public class ApplyLatestVersion : BuildQueryProcessor
    {
        public override void Process(BuildQueryArgs args)
        {
            if(args.TabItem?.ApplyLatestVersion ?? false)
            {
                args.Query = args.Query.Cast<NimbleSearchResultItem>().Filter(x => x.IsLatestVersion);
            }
        }
    }
}