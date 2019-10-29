namespace NimbleSearch.Foundation.Core.Models
{
    internal class SearchResultFacetValue : ISearchResultFacetValue
    {
        public object Value { get; set; }
        public int Count { get; set; }
        public bool Selected { get; set; }
        public string Title { get; set;  }
    }
}