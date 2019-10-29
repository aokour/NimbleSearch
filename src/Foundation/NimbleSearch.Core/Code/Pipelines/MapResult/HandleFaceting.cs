using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Models.Response;
using NimbleSearch.Foundation.Abstractions.Pipelines.MapResult;
using NimbleSearch.Foundation.Core.Util;
using System.Collections.Generic;
using System.Linq;
using static NimbleSearch.Foundation.Abstractions.Templates.Facet;

namespace NimbleSearch.Foundation.Core.Pipelines.MapResult
{
    public class HandleFaceting : MapResultProcessor
    {
        public override void Process(MapResultArgs args)
        {

            var facets = args.TabItem?.Facets;
            if (facets == null || facets.Length <= 0)
                return;

            var response = new List<Facet>();

            // Map Facets
            foreach (var facet in facets)
            {
                var model = ProcessFacet(facet, args);
                if(model != null)
                { 
                    response.Add(model);
                }
            }

            args.Response.Facets = response;
        }


        public virtual Facet ProcessFacet(FacetItem facet, MapResultArgs args) {

            // Don't even send facet if no value options exist
            var options = args.FacetResults?.FirstOrDefault(x => x.Name == facet.IndexProperty)?.Values;
            if (options == null || !options.Any())
                return null;

            var facetModel = new Facet
            {
                FieldName = facet.IndexProperty,
                ID = facet.ID.Guid,
                Name = facet.DisplayName,
                Type = facet.InnerItem.TemplateName,
                CollapsedByDefault = facet.CollapsedByDefault
            };

            var selected = args.SearchParameters.SelectedFacets?.FirstOrDefault(x => x.FacetId == facet.ID.Guid)?.SelectedValues;
            facetModel.Values = ProcessFacetValues(facet, options, selected);

            return facetModel;
        }


        public virtual IList<FacetValue> ProcessFacetValues(FacetItem facet, IList<Sitecore.ContentSearch.Linq.FacetValue> options, IEnumerable<string> selected)
        {
            options = options ?? new List<Sitecore.ContentSearch.Linq.FacetValue>();
            selected = selected ?? Enumerable.Empty<string>();

            var values = new List<FacetValue>();
            foreach (var option in options)
            {
                var value = new FacetValue()
                {
                    Name = option.Name,
                    Count = option.AggregateCount,
                    IsSelected = selected.Contains(option.Name)
                };
                values.Add(value);
            }

            values = SortFacetValues(values, facet.SortBy);

            if (facet.LimitValues > 0)
            {
                values = values.Take(facet.LimitValues).ToList();
            }

            return values;
        }


        public virtual List<FacetValue> SortFacetValues(List<FacetValue> values, FacetSortDirection sortBy)
        {
            if (values == null || !values.Any())
                return values;

            switch (sortBy) {
                case FacetSortDirection.AlphabeticalAsc:
                    values = values.OrderBy(x => x.Name, new SemiNumericComparer()).ToList();
                    break;
                case FacetSortDirection.AlphabeticalDesc:
                    values = values.OrderByDescending(x => x.Name, new SemiNumericComparer()).ToList();
                    break;
                default: // FacetSortDirection.Occurrences
                    values = values.OrderByDescending(x => x.Count).ToList();
                    break;
            }

            return values;
        }
    }
}