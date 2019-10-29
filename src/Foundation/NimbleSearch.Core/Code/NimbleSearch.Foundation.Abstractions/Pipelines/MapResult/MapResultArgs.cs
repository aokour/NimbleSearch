using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Models.Response;
using NimbleSearch.Foundation.Abstractions.Models.Search;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Pipelines;
using System.Collections.Generic;

namespace NimbleSearch.Foundation.Abstractions.Pipelines.MapResult
{
    public class MapResultArgs : PipelineArgs
    {
        // Input
        public TabItem TabItem { get; set; }
        public SearchParameters SearchParameters { get; set; }
        public int TotalSearchResults { get; set; }
        public IEnumerable<SearchHit<NimbleSearchResultItem>> Hits { get; set; }
        public List<FacetCategory> FacetResults { get; set; }
        public long QueryDuration { get; set; }

        // Output
        public SearchResponse Response { get; set; }
    }
}