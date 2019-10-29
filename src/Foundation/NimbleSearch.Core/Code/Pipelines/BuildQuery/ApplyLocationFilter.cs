using NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using System.Linq;

namespace NimbleSearch.Foundation.Core.Pipelines.BuildQuery
{
    public class ApplyLocationFilter : BuildQueryProcessor
    {
        public override void Process(BuildQueryArgs args)
        {
            // If tab defines start locations...
            var locations = args.TabItem?.StartLocations;
            if (locations == null || !locations.Any())
                return;

            // Apply Start Locations
            var rootPredicates = PredicateBuilder.False<SearchResultItem>();
            foreach (var rootId in locations)
            {
                rootPredicates = rootPredicates.Or(item => item.Paths.Contains(rootId));
            }

            args.Query = args.Query.Filter(rootPredicates);            
        }
        
    }
}