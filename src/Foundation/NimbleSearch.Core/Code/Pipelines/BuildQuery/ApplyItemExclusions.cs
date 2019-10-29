using NimbleSearch.Foundation.Abstractions.Models.Search;
using NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery;
using Sitecore.ContentSearch.Linq;
using System.Linq;

namespace NimbleSearch.Foundation.Core.Pipelines.BuildQuery
{
    public class ApplyItemExclusions: BuildQueryProcessor
    {
        public override void Process(BuildQueryArgs args)
        {
            args.Query = args.Query.Cast<NimbleSearchResultItem>().Filter(x => !x.ExcludeFromSearchResults);
        }
    }
}