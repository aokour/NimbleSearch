using Sitecore.ContentSearch.ComputedFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch;
using System.Xml;
using Sitecore.Xml;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Data;
using Sitecore;
using Sitecore.Data.Fields;

namespace NimbleSearch.Foundation.Core.ComputedFields
{
    public class LinkedItemComputedField : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }
        public string SourceField { get; set; }        
        public Database Database { get; set; }

        public LinkedItemComputedField(XmlNode configNode)
        {                        
            this.SourceField = XmlUtil.GetAttribute("sourceField", configNode);            
        }


        public object ComputeFieldValue(IIndexable indexable)
        {
            var indexItem = indexable as SitecoreIndexableItem;
            if (indexItem == null)
            {
                return null;
            }
            var item = indexItem.Item;
            Database = item.Database;
            if (!string.IsNullOrEmpty(this.SourceField))
            {
                return this.ComputeLinkValue(item, item.Language);
            }
            return new List<string>();
        }

        private string ComputeLinkValue(Item item, Language p_SourceItemLanguage)
        {
            if (item.Fields[this.SourceField] == null || string.IsNullOrEmpty(item.Fields[this.SourceField].Value))
            {
                return string.Empty;
            }

            var linkedItemReference = new ReferenceField(item.Fields[this.SourceField]);

            return linkedItemReference.TargetItem?.Name;
        }
    }
}