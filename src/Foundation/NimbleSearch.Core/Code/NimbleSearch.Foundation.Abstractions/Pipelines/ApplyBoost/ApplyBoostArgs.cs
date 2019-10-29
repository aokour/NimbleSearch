using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using System.Linq;

namespace NimbleSearch.Foundation.Abstractions.Pipelines.ApplyBoost
{
    public class ApplyBoostArgs : PipelineArgs
    {
        // Input
        public Item BoostItem { get; set; }
        public SearchParameters SearchParameters { get; set; }

        // Output
        public IQueryable<SearchResultItem> Query { get; set; }
    }
}