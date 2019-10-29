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

namespace NimbleSearch.Foundation.Core.ComputedFields
{
    public class RemoveWhitespaceAndLowerCaseComputedField : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }
        public string SourceField { get; set; }
        public bool RemoveWhitespace { get; set; }
        public string ReferencedFieldName { get; set; }
        public Database Database { get; set; }

        public RemoveWhitespaceAndLowerCaseComputedField(XmlNode configNode)
        {
            this.FieldName = XmlUtil.GetAttribute("fieldName", configNode);
            this.ReturnType = XmlUtil.GetAttribute("returnType", configNode);
            this.SourceField = XmlUtil.GetAttribute("sourceField", configNode);
            this.RemoveWhitespace = XmlUtil.GetAttribute("removeWhitespace", configNode) == "true";
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
            object obj = null;
            if (item !=null && !string.IsNullOrEmpty(this.SourceField) && item.Fields[this.SourceField] != null)
            {
                string value = this.GetItemSourceFieldValue(item);
                if(!string.IsNullOrWhiteSpace(value))
                {
                    value =  value.ToLower();
                    if(RemoveWhitespace)
                    {
                        value = value.Replace(" ", "").Replace("-", "");
                    }
                    if (value.Length > 10)
                        return value.Substring(0, 10);
                    else
                        return value;
                }
            }
            return obj;
        }

        private string GetItemSourceFieldValue(Item p_Item)
        {
            IEnumerable<string> strs = new List<string>();
            string fieldValue = (p_Item.Fields[this.SourceField] != null ? p_Item.Fields[this.SourceField].Value : null);
            return fieldValue;
        }
    }
}