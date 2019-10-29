using NimbleSearch.Foundation.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Api.Parsers
{
    public static class QueryParser
    {
        private readonly static string _facetsDelimiter = "||";
        private readonly static char _facetDelimiter = ':';
        private readonly static char _facetValuesDelimiter = '|';

        public static Dictionary<string, string[]> ParseFacets(string facets)
        {
            if (facets == null) return null;

            var result = new Dictionary<string, string[]>();

            var facetsKeyValues = facets.Split(new string[] { _facetsDelimiter }, StringSplitOptions.None);

            foreach (var facetKeyValue in facetsKeyValues)
            {
                var keyValues = facetKeyValue.Split(_facetDelimiter);

                //skip corrupt values
                if (keyValues.Length == 2)
                {
                    if (!result.ContainsKey(keyValues[0]))
                    {
                        result[keyValues[0]] = keyValues[1].Split(_facetValuesDelimiter);
                    }
                }
            }

            return result;
        }

        public static List<FieldFilterGroup> ParseFieldsFilters(string filter)
        {
            List<FieldFilterGroup> fieldsFilters = new List<FieldFilterGroup>();

            if (string.IsNullOrWhiteSpace(filter))
                return fieldsFilters;
            foreach (string groupQuery in filter.Split(new string[] { " AND "},  StringSplitOptions.None))
            {
                fieldsFilters.Add(ParseGroup(groupQuery));
            }
            return fieldsFilters;
        }

        private static FieldFilterGroup ParseGroup(string groupQuery)
        {
            FieldFilterGroup group = new FieldFilterGroup();
            group.FieldFilters = new List<FieldFilter>();
            string[] values;
            if(groupQuery.Contains(">"))
            {
                string groupQueryRight = groupQuery.Split('>')[1];
                string groupQueryLeft = groupQuery.Split('>')[0];

                FieldFilter fieldFilter = new FieldFilter();
                fieldFilter.FieldName = groupQueryLeft.Trim().TrimStart('\'').TrimEnd('\'');
                fieldFilter.Operator = FilterOperator.GreaterThan;
                fieldFilter.FiledValue = groupQueryRight.Trim().TrimStart('\'').TrimEnd('\'');
                group.FieldFilters.Add(fieldFilter);
            }
            else if (groupQuery.Contains("<"))
            {
                string groupQueryRight = groupQuery.Split('<')[1];
                string groupQueryLeft = groupQuery.Split('<')[0];

                FieldFilter fieldFilter = new FieldFilter();
                fieldFilter.FieldName = groupQueryLeft.Trim().TrimStart('\'').TrimEnd('\'');
                fieldFilter.Operator = FilterOperator.LessThan;
                fieldFilter.FiledValue = groupQueryRight.Trim().TrimStart('\'').TrimEnd('\'');
                group.FieldFilters.Add(fieldFilter);
            }
            else if (groupQuery.Contains("=="))
            {
                string groupQueryRight = groupQuery.Split(new string[] { "==" }, StringSplitOptions.None)[1];
                string groupQueryLeft = groupQuery.Split(new string[] { "==" }, StringSplitOptions.None)[0];
                if (groupQueryRight.Contains("|"))
                {
                    group.Operator = GroupOperator.Or;
                    values = groupQueryRight.Split('|').Select(x=>x.Trim().TrimStart('\'').TrimEnd('\'')).ToArray();
                    foreach (var value in values)
                    {
                        FieldFilter fieldFilter = new FieldFilter();
                        fieldFilter.FieldName = groupQueryLeft.Trim().TrimStart('\'').TrimEnd('\''); 
                        fieldFilter.Operator = FilterOperator.Equal;
                        fieldFilter.FiledValue = value;
                        group.FieldFilters.Add(fieldFilter);
                    }
                }
                if (groupQueryRight.Contains("&"))
                {
                    group.Operator = GroupOperator.And;
                    values = groupQueryRight.Split('&').Select(x => x.Trim().TrimStart('\'').TrimEnd('\'')).ToArray();
                    foreach (var value in values)
                    {
                        FieldFilter fieldFilter = new FieldFilter();
                        fieldFilter.FieldName = groupQueryLeft.Trim().TrimStart('\'').TrimEnd('\'');
                        fieldFilter.Operator = FilterOperator.Equal;
                        fieldFilter.FiledValue = value;
                        group.FieldFilters.Add(fieldFilter);
                    }
                }
                else
                {
                    groupQueryRight = groupQueryRight.Trim().TrimStart('\'').TrimEnd('\'');
                    if (groupQueryRight.StartsWith("*") && groupQueryRight.EndsWith("*"))
                    {
                        FieldFilter fieldFilter = new FieldFilter();
                        fieldFilter.FieldName = groupQueryLeft.Trim().TrimStart('\'').TrimEnd('\'');
                        fieldFilter.Operator = FilterOperator.Contains;
                        fieldFilter.FiledValue = groupQueryRight;
                        group.FieldFilters.Add(fieldFilter);
                    }
                    else if (groupQueryRight.StartsWith("*"))
                    {
                        FieldFilter fieldFilter = new FieldFilter();
                        fieldFilter.FieldName = groupQueryLeft.Trim().TrimStart('\'').TrimEnd('\'');
                        fieldFilter.Operator = FilterOperator.StartWith;
                        fieldFilter.FiledValue = groupQueryRight.TrimStart('*').Trim().TrimStart('\'');
                        group.FieldFilters.Add(fieldFilter);
                    }
                    else
                    {
                        FieldFilter fieldFilter = new FieldFilter();
                        fieldFilter.FieldName = groupQueryLeft.Trim().TrimStart('\'').TrimEnd('\'');
                        fieldFilter.Operator = FilterOperator.Equal;
                        fieldFilter.FiledValue = groupQueryRight;
                        group.FieldFilters.Add(fieldFilter);
                    }
                }
            }
            else if (groupQuery.Contains("!="))
            {
                string groupQueryRight = groupQuery.Split(new string[] { "==" }, StringSplitOptions.None)[1];
                string groupQueryLeft = groupQuery.Split(new string[] { "==" }, StringSplitOptions.None)[0];

                groupQueryRight = groupQueryRight.Trim().TrimStart('\'').TrimEnd('\'');
                if (groupQueryRight.StartsWith("*"))
                {
                    FieldFilter fieldFilter = new FieldFilter();
                    fieldFilter.FieldName = groupQueryLeft.Trim().TrimStart('\'').TrimEnd('\'');
                    fieldFilter.Operator = FilterOperator.NotContains;
                    fieldFilter.FiledValue = groupQueryRight;
                    group.FieldFilters.Add(fieldFilter);
                }
                else
                {
                    FieldFilter fieldFilter = new FieldFilter();
                    fieldFilter.FieldName = groupQueryLeft.Trim().TrimStart('\'').TrimEnd('\'');
                    fieldFilter.Operator = FilterOperator.NotEqual;
                    fieldFilter.FiledValue = groupQueryRight;
                    group.FieldFilters.Add(fieldFilter);
                }
            }        
            return group;
        }
    }
}