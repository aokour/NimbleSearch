using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Models.Response;
using Sitecore.Pipelines;

namespace NimbleSearch.Foundation.Abstractions.Pipelines.SearchAnaltyics
{
    public class SearchAnalyticsArgs : PipelineArgs
    {
        // Input
        public TabItem TabItem { get; set; }

        public SearchParameters SearchParameters { get; set; }

        public SearchResponse SearchResponse { get; set; }
    }
}