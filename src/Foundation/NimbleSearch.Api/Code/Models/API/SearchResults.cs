using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Api.Models.API
{
    public class SearchResult
    {
        public int TotalNumberOfResults { get; internal set; }
        public int TotalPages { get; internal set; }
        public int CurrentPageIndex { get; internal set; }
        public int PageSize { get; set; }
        public int DocumentStartIndex { get; set; }
        public int DocumentEndIndex { get; set; }
        public long Duration { get; internal set; }
        public string NoResultsMessage { get; set; }
        public bool MoveToNextTabWhenNoResults { get; set; }
        public string NextTab { get; set; }
        public SortOption DefaultSortOption { get; internal set; }
        public List<FacetResult> Facets { get; internal set; }
        public List<TabItem> Tabs { get; internal set; }
        public List<SortOption> SortOptions { get; internal set; }
        public List<ItemResult> Items { get; internal set; }
        public List<string> PopularTerms { get; internal set; }
    }
    public class ItemResult
    {
        public string Url { get; set; }       
        public ID ItemID { get; internal set; }
        [JsonIgnore]
        public Item Item { get; internal set; }
        public string ItemName { get; internal set; }
        public string ItemPath { get; internal set; }
        public string ItemTemplateName { get; internal set; }
        public ID ItemTemplateId { get; internal set; }
        public Dictionary<string, string> Properties { get; internal set; }
    }

    public class FacetResult
    {
        public string Name { get; internal set; }
        public string FieldName { get; internal set; }
        public Boolean ExpandByDefault { get; internal set; }
        public int HideValuesAfter { get; internal set; }

        public List<FacetValue> Values { get; internal set; }
    }

    public class FacetValue
    {
        public string Name { get; internal set; }
        public string Value { get; internal set; }
        public int Count { get; internal set; }
        public bool Selected { get; internal set; }
    }

    public class TabItem
    {
        public ID ID { get; internal set; }
        public string Title { get; internal set; }
        public int Count { get; internal set; }
        public bool Selected { get; internal set; }
    }

    public class SortOption
    {
        public string Name { get; internal set; }
        public string Value { get; internal set; }
        public string Direction { get; internal set; }

    }
}