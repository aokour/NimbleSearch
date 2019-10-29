using NimbleSearch.Foundation.Abstractions.Models.ComputedFields;
using Sitecore.ContentSearch;
using Sitecore.Data.Items;

namespace NimbleSearch.Foundation.Core.ComputedFields
{
    public class BestText : CommonComputedFieldBase
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item scIndexable = indexable as SitecoreIndexableItem;
            if (ShouldSkip(scIndexable))
                return null;

            return GetFirstValidValue(scIndexable, Configuration.Settings.Instance.BestSummaryFields?.ToArray());
        }        

    }
}