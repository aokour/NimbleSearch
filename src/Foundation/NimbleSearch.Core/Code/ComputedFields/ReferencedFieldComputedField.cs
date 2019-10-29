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

namespace NimbleSearch.Foundation.Core.ComputedFields
{
    public class ReferencedFieldComputedField : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }
        public string SourceField { get; set; }
        public string ReferencedFieldName { get; set; }
        public Database Database { get; set; }

        public ReferencedFieldComputedField(XmlNode configNode)
        {
            this.FieldName = XmlUtil.GetAttribute("fieldName", configNode);
            this.ReturnType = XmlUtil.GetAttribute("returnType", configNode);
            this.SourceField = XmlUtil.GetAttribute("sourceField", configNode);
            this.ReferencedFieldName = XmlUtil.GetAttribute("referencedFieldName", configNode);
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
            if (!string.IsNullOrEmpty(this.SourceField) && !string.IsNullOrEmpty(this.ReferencedFieldName))
            {
                IEnumerable<string> referencedItemsIds = this.GetReferencedItemsIds(item);
                if (referencedItemsIds.Any<string>())
                {
                    if (EqualsIgnoreCase(ReturnType, "Integer"))
                    {
                        return this.ComputeIntValue(referencedItemsIds, item.Language);
                    }
                    else if (EqualsIgnoreCase(ReturnType, "Number"))
                    {
                        return this.ComputeNumberValue(referencedItemsIds, item.Language);
                    }
                    else if (EqualsIgnoreCase(ReturnType, "date")|| EqualsIgnoreCase(ReturnType, "datetime"))
                    {
                        return this.ComputeDateTimeValue(referencedItemsIds, item.Language);
                    }
                    else
                    {
                        return this.ComputeStringValue(referencedItemsIds, item.Language);
                    }
                }
            }
            return new List<string>();
        }

        private List<double> ComputeNumberValue(IEnumerable<string> p_ReferencedItemsIds, Language p_SourceItemLanguage)
        {
            double num;
            List<double> nums = new List<double>();
            foreach (string pReferencedItemsId in p_ReferencedItemsIds)
            {
                if (!double.TryParse(this.GetItemValue(pReferencedItemsId, p_SourceItemLanguage), out num))
                {
                    Sitecore.Diagnostics.Log.Debug(string.Concat("Double.TryParse failed in item with id ", pReferencedItemsId, ". Return null value."));
                }
                else
                {
                    nums.Add(num);
                }
            }
            if (!nums.Any<double>())
            {
                return null;
            }
            return nums;
        }

        private string GetItemValue(string p_Id, Language p_SourceItemLanguage)
        {
            string fieldValue = null;
            Item item = this.ResolveReferencedItem(p_Id, p_SourceItemLanguage);
            if (item != null)
            {
                fieldValue = (item.Fields[this.ReferencedFieldName] != null ? item.Fields[this.ReferencedFieldName].Value : null);
            }
            return fieldValue;
        }

        private Item ResolveReferencedItem(string p_Id, Language p_SourceItemLanguage)
        {
            Item item = Database.GetItem(p_Id, p_SourceItemLanguage);
           
            return item;
        }

        private List<int> ComputeIntValue(IEnumerable<string> p_ReferencedItemsIds, Language p_SourceItemLanguage)
        {
            int num;
            List<int> nums = new List<int>();
            foreach (string pReferencedItemsId in p_ReferencedItemsIds)
            {
                if (!int.TryParse(this.GetItemValue(pReferencedItemsId, p_SourceItemLanguage), out num))
                {
                    Sitecore.Diagnostics.Log.Debug(string.Concat("Int32.TryParse failed in item with id ", pReferencedItemsId, ". Return null value."));
                }
                else
                {
                    nums.Add(num);
                }
            }
            if (!nums.Any<int>())
            {
                return null;
            }
            return nums;
        }

        private List<DateTime> ComputeDateTimeValue(IEnumerable<string> p_ReferencedItemsIds, Language p_SourceItemLanguage)
        {
            List<DateTime> dateTimes = new List<DateTime>();
            foreach (string pReferencedItemsId in p_ReferencedItemsIds)
            {
                DateTime dateTime = DateUtil.IsoDateToDateTime(this.GetItemValue(pReferencedItemsId, p_SourceItemLanguage));
                if (dateTime == DateTime.MinValue)
                {
                    continue;
                }
                dateTimes.Add(dateTime);
            }
            if (!dateTimes.Any<DateTime>())
            {
                return null;
            }
            return dateTimes;
        }

        private List<string> ComputeStringValue(IEnumerable<string> p_ReferencedItemsIds, Language p_SourceItemLanguage)
        {
            List<string> strs = new List<string>();
            foreach (string pReferencedItemsId in p_ReferencedItemsIds)
            {
                string itemValue = this.GetItemValue(pReferencedItemsId, p_SourceItemLanguage);
                if (string.IsNullOrEmpty(itemValue))
                {
                    continue;
                }
                strs.Add(itemValue);
            }
            if (!strs.Any<string>())
            {
                return null;
            }
            return strs;
        }

        private IEnumerable<string> GetReferencedItemsIds(Item p_Item)
        {
            IEnumerable<string> strs = new List<string>();
            string fieldValue = (p_Item.Fields[this.SourceField] != null ? p_Item.Fields[this.SourceField].Value : null);
            if (!string.IsNullOrEmpty(fieldValue))
            {
                string[] strArrays = new string[] { "|" };
                strs = fieldValue.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
            }
            return strs;
        }

        public  bool EqualsIgnoreCase(string p_String1, string p_String2)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(p_String1, p_String2);
        }

    }
}