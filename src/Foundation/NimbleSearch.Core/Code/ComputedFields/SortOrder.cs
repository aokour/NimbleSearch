using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Core.ComputedFields
{
    public class SortOrderField : IComputedIndexField
    {
        public object ComputeFieldValue(IIndexable indexable)
        {
            float DefaultSortOrderValue = 0;

            var item = (Item)(indexable as SitecoreIndexableItem);
            if (item == null) return null;

            if (string.IsNullOrEmpty(item[Sitecore.FieldIDs.Sortorder]))
            {
                return DefaultSortOrderValue;
            }

            float sortOrder;
            return float.TryParse(item[Sitecore.FieldIDs.Sortorder], out sortOrder)
                ? sortOrder
                : DefaultSortOrderValue;
        }

        public string FieldName { get; set; }

        public string ReturnType { get; set; }
    }
}