using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Core.ComputedFields
{
    public class TemplateName : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        public object ComputeFieldValue(IIndexable indexable)
        {
            var item = (Item)(indexable as SitecoreIndexableItem);
            if (item == null) return null;
            if (TemplateManager.IsTemplate(item)) return null;

            return item.TemplateName;
        }
    }
}