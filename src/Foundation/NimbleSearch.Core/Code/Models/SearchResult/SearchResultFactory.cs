namespace NimbleSearch.Foundation.Core.Models
{
    using System.Linq;
    using Sitecore.ContentSearch.SearchTypes;
    using Sitecore.Data.Items;
    using NimbleSearch.Foundation.Core.Repositories;

    public class SearchResultFactory
    {
        public static ISearchResult Create(SearchResultItem result)
        {
            var item = result.GetItem();
            var searchresult = new SearchResult(item);
            return searchresult;
        }
    }
}