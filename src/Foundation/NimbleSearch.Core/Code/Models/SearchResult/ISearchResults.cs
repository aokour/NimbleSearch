namespace NimbleSearch.Foundation.Core.Models
{
    using System;
    using System.Collections.Generic;

    public interface ISearchResults
    {
        IEnumerable<ISearchResultFacet> Facets { get; set; }
        IEnumerable<ISearchResult> Results { get; }
        int TotalNumberOfResults { get; }
        /// <summary>
        /// Duration of the query execution in milliseconds
        /// </summary>
        long Duration { get; }
    }
}