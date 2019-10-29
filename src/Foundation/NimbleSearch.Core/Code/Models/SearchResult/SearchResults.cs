namespace NimbleSearch.Foundation.Core.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SearchResults : ISearchResults
    {
        public IEnumerable<ISearchResult> Results { get; set; }
        public int TotalNumberOfResults { get; set; }
        public IEnumerable<ISearchResultFacet> Facets { get; set; }
        /// <summary>
        /// Duration of the query execution in milliseconds
        /// </summary>
        public long Duration { get; set; }
    }
}