using Sitecore.ContentSearch;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Core.Util
{
    public static class Utilities
    {
        public static string GetFieldIndexType(ISearchIndex searchIndex, string fieldName)
        {
            try
            {
                string returnType = string.Empty;
                var fieldConfigurations = searchIndex.Configuration.FieldMap.GetFieldConfiguration(fieldName);
                if (fieldConfigurations == null)
                {
                    var computedFieldConfig = searchIndex.Configuration.DocumentOptions.ComputedIndexFields.Where(fld => fld.FieldName == fieldName).FirstOrDefault();
                    if (computedFieldConfig != null)
                    {
                        returnType = computedFieldConfig.ReturnType;
                    }
                }
                else
                {
                    //Handle lucene and solr field types (Lucene --> type, Solr--> returnType)
                    if (fieldConfigurations.Attributes.ContainsKey("typeName") && !string.IsNullOrEmpty( fieldConfigurations.Attributes["typeName"]))
                        return fieldConfigurations.Attributes["typeName"];
                    else if (fieldConfigurations.Attributes.ContainsKey("type") && !string.IsNullOrEmpty(fieldConfigurations.Attributes["type"]))
                        return fieldConfigurations.Attributes["type"];
                }
                return "string";
            }
            catch (Exception ex)
            {
                return "string";
            }
        }
        public static bool TryConvertValueToType<T>(string value, ref T result)
        {
            if(typeof(T) == typeof(System.Double))
            {
                Double converted;
                if(Double.TryParse(value,out converted))
                {
                    result = (T)Convert.ChangeType(converted, typeof(T));
                    return true;
                }
            }
            else if (typeof(T) == typeof(System.DateTime))
            {
                DateTime converted;
                if (DateTime.TryParseExact(value, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out converted))
                {
                    result = (T)Convert.ChangeType(converted, typeof(T));
                    return true;
                }
                if (DateTime.TryParseExact(value, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out converted))
                {
                    result = (T)Convert.ChangeType(converted, typeof(T));
                    return true;
                }
            }
            else if (typeof(T) == typeof(System.Single))
            {
                Single converted;
                if (Single.TryParse(value, out converted))
                {
                    result = (T)Convert.ChangeType(converted, typeof(T));
                    return true;
                }
            }
            else if (typeof(T) == typeof(System.Int32))
            {
                Int32 converted;
                if (Int32.TryParse(value, out converted))
                {
                    result = (T)Convert.ChangeType(converted, typeof(T));
                    return true;
                }
            }
            else if (typeof(T) == typeof(System.Int64))
            {
                Int64 converted;
                if (Int64.TryParse(value, out converted))
                {
                    result = (T)Convert.ChangeType(converted, typeof(T));
                    return true;
                }
            }
            else if (typeof(T) == typeof(System.Guid))
            {
                Guid converted;
                if (Guid.TryParse(value, out converted))
                {
                    result = (T)Convert.ChangeType(converted, typeof(T));
                    return true;
                }
            }

            return false;
        }

    }
}