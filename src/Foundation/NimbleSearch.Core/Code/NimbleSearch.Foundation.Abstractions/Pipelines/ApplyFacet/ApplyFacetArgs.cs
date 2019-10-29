using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using System.Linq;

namespace NimbleSearch.Foundation.Abstractions.Pipelines.ApplyFacet
{
    public class ApplyFacetArgs : PipelineArgs
    {
        // Input
        public FacetItem FacetItem { get; set; }
        public SearchParameters SearchParameters { get; set; }

        // Output
        public IQueryable<SearchResultItem> Query { get; set; }
    }
}