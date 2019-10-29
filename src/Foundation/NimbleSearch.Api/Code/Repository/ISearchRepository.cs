using NimbleSearch.Foundation.Api.Models.API;
using NimbleSearch.Foundation.Api.Models.Configurations;
using NimbleSearch.Foundation.Core.Models.Filters;
using Sitecore.Data;
using NimbleSearch.Foundation.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NimbleSearch.Foundation.Api.Repository
{
    public interface ISearchRepository
    {
        void InitializeSearch(SearchParameters parameters);
        void InitializeSearch(SearchParameters parameters, SearchConfigurations configurations);
        ISearchResults PerformSearch();
        ISearchResults PerformAutoComplete();
        //Task<List<string>> GetPopularTerms();
    }
}