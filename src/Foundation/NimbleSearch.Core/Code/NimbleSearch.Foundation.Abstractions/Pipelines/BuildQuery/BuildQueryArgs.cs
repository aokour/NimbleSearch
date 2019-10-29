using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Pipelines;
using System.Linq;

namespace NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery
{
    public class BuildQueryArgs : PipelineArgs
    {
        // Input
        public TabItem TabItem { get; set; }
        public SearchParameters SearchParameters { get; set; }
        public IProviderSearchContext SearchContext { get; set; }

        // Output
        public IQueryable<SearchResultItem> Query { get; set; }
    }
}