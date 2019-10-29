using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Pipelines.ApplyFacet;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data.Items;
using System.Linq;

namespace NimbleSearch.Foundation.Core.Pipelines.ApplySelectedFacet
{
    public class SimpleFacet : ApplyFacetProcessor
    {

        public override void Process(ApplyFacetArgs args)
        {
            if (!IsSimpleFacet(args.FacetItem))
                return;
            
            var selectedValues = args.SearchParameters.SelectedFacets?.FirstOrDefault(x => x.FacetId == args.FacetItem.ID.Guid)?.SelectedValues;
            if (selectedValues == null || !selectedValues.Any())
                return;

            var indexProp = args.FacetItem.IndexProperty;
            if (string.IsNullOrWhiteSpace(indexProp))
                return;

            var facetPredicate = PredicateBuilder.True<SearchResultItem>();
            foreach (var value in selectedValues)
            {
                facetPredicate = args.FacetItem.UseAnd?
                    facetPredicate.And(x => x[indexProp] == value)
                    : facetPredicate.Or(x => x[indexProp] == value);
            }

            args.Query = args.Query.Where(facetPredicate);            
        }


        // ////////////////////////////////////////////////////////////////////////////////

        protected virtual bool IsSimpleFacet(Item facet)
        {
            if (facet == null)
                return false;

            return Settings.SimpleFacets.TemplateIds.Contains(facet.TemplateID);
        }

    }
}