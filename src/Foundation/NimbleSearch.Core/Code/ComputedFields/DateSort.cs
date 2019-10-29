using NimbleSearch.Foundation.Abstractions.Models.ComputedFields;
using Sitecore.ContentSearch;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Linq;

namespace NimbleSearch.Foundation.Core.ComputedFields
{
    public class DateSort : CommonComputedFieldBase
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item scIndexable = indexable as SitecoreIndexableItem;
            if (ShouldSkip(scIndexable))
                return null;

            return GetBestDate(scIndexable, Configuration.Settings.Instance.BestDateFields?.ToArray());
        }


        public virtual DateTime? GetBestDate(Item item, params string[] fields) {

            if (item == null || fields == null || !fields.Any())
            {
                return null;
            }

            foreach (var fieldName in fields)
            {
                var fieldValue = item[fieldName];
                if (IsValid(fieldValue))
                {
                    var dateField = (DateField)item.Fields[fieldName];
                    if (dateField != null)
                    {
                        // return first hit only
                        return dateField.DateTime;
                    }
                }
            }

            return null;
        }

    }
}