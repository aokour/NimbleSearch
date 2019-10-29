using NimbleSearch.Foundation.Abstractions.Pipelines.ApplyFacet;
using Sitecore.ContentSearch.Linq;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace NimbleSearch.Foundation.Core.Pipelines.ApplyFacetOn
{
    public class SimpleFacet : ApplyFacetProcessor
    {

        public override void Process(ApplyFacetArgs args)
        {
            if (!IsSimpleFacet(args.FacetItem))
                return;

            var indexProp = args.FacetItem.IndexProperty;
            if (string.IsNullOrWhiteSpace(indexProp))
            {
                Log.Warn($"Nimble:ApplyFacet:ListFacet: Missing Expected indexProp. Skipping facet {args.FacetItem.DisplayName}, {args.FacetItem.ID}", this);
                return;
            }
            var minValues = args.FacetItem.MinimumValues;

            args.Query = args.Query.FacetOn(x => x[indexProp], minValues);
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