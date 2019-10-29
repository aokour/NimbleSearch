using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Diagnostics;
using Sitecore;
using Sitecore.Data;
using System.Web.Mvc;
using NimbleSearch.Foundation.Core.Models;
using NimbleSearch.Foundation.Core.Models.Filters;
using NimbleSearch.Foundation.Core.Services;
using NimbleSearch.Foundation.Api.Models.API;
using Sitecore.Data.Items;
using SearchTemplates = NimbleSearch.Foundation.Api.Templates;
using NimbleSearch.Foundation.Api.Models.Configurations;
using NimbleSearch.Foundation.Api.Parsers;

using System.Threading.Tasks;

namespace NimbleSearch.Foundation.Api.Repository
{
    public class SearchRepository :  ISearchRepository
    {
        private ISearchService _searchService;
        SearchConfigurations _configurations;
        SearchParameters _parameters;
        public SearchRepository()
        {
            //TO DO : Lazy man DI, proper DI needs to be implemented
            _searchService = DependencyResolver.Current.GetService<SitecoreContentSearchService>();
        }

        public void InitializeSearch(SearchParameters parameters)
        {
            ValidateSearchConfigurations(parameters);
            this._parameters = parameters;
            _configurations = BuildSearchConfigurations(parameters);
        }

        public void InitializeSearch(SearchParameters parameters, SearchConfigurations configurations)
        {
            ValidateSearchConfigurations(parameters);
            this._parameters = parameters;
            this._configurations = configurations;
        }

        public ISearchResults PerformSearch() 
        {
            var tabConfigurations = _configurations.Tabs.Where(x => x.TabID == Sitecore.Data.ID.Parse(_parameters.TabId)).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(_parameters.SortBy) && tabConfigurations.DefaultSortOption != null && !string.IsNullOrWhiteSpace(tabConfigurations.DefaultSortOption.FieldName))
                _parameters.SortBy = tabConfigurations.DefaultSortOption.FieldName;

            if (_parameters.PageSize == 0)
                _parameters.PageSize = tabConfigurations.PageSize;

            QuerySettings _settings = BuildQuerySettings(tabConfigurations);
            QueryFilter queryFilter = BuildQueryFilter();

            _searchService.InitializeFilters(_settings);
            var result = _searchService.Search(queryFilter);

            //Register search term in Analytics
            if(tabConfigurations.RegisterSearchTerm && !string.IsNullOrEmpty(_parameters.Keyword) && result.TotalNumberOfResults > 0)
            {
                //_termAnalyticsRepository.RegisterSearchTermOccurance(_parameters.Keyword);
            }
            return result;
        }

        public ISearchResults PerformAutoComplete()
        {
            var tabConfigurations = _configurations.Tabs.Where(x => x.TabID == Sitecore.Data.ID.Parse(_parameters.TabId)).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(_parameters.SortBy) && tabConfigurations.DefaultSortOption != null && !string.IsNullOrWhiteSpace(tabConfigurations.DefaultSortOption.FieldName))
                _parameters.SortBy = tabConfigurations.DefaultSortOption.FieldName;

            if (_parameters.PageSize == 0)
                _parameters.PageSize = tabConfigurations.PageSize;

            QuerySettings _settings = BuildQuerySettings(tabConfigurations);
            QueryFilter queryFilter = BuildQueryFilter();

            _searchService.InitializeFilters(_settings);
            var result = _searchService.Autocomplete(queryFilter);
            return result;
        }

        //public async Task<List<string>> GetPopularTerms()
        //{
        //    List<string> results = new List<string>();

        //    if(_parameters.ShowPopularTerms && !string.IsNullOrEmpty(_parameters.Keyword) && _parameters.Keyword.Length>=2)
        //    {
        //        var list = await _termAnalyticsRepository.GetPopularSearchTerms(_parameters.Keyword,3);
        //        results = list.Select(x => x.Term).ToList();
        //    }

        //    return results;
        //}

        private QuerySettings BuildQuerySettings(Tab tab)
        {
            var tabConfigurations = tab;
            if (tabConfigurations == null)
                throw new Exception("Error: Tab ID is incorrect");

            QuerySettings _settings = new QuerySettings();
            _settings.Facets = tabConfigurations.Facets.Select(x => x.FieldName).ToList();
            _settings.IncludedTemplates = tabConfigurations.Templates;
            _settings.SortByField = !string.IsNullOrWhiteSpace(_parameters.SortBy) ? _parameters.SortBy : string.Empty;
            _settings.SortDirection = (!string.IsNullOrWhiteSpace(_parameters.SortDirection) && (_parameters.SortDirection.ToLower() == "desc" || _parameters.SortDirection.ToLower() == "descending")
                                        ? SortDirection.Desc
                                        : SortDirection.Asc);
            _settings.IndexName = tabConfigurations.IndexName;
            _settings.DatabaseName = _parameters.DatabaseName;
            _settings.StartRoots = tabConfigurations.StartLocations;
            _settings.QueryFilter = tabConfigurations.QueryFilter;
            _settings.BoostOnFields = (!string.IsNullOrWhiteSpace(tabConfigurations.BoostOnFields) ? tabConfigurations.BoostOnFields.Split('|').ToList() : new List<string>());
            _settings.SearchAdditionalFields = (!string.IsNullOrWhiteSpace(tabConfigurations.SearchAdditionalFields) ? tabConfigurations.SearchAdditionalFields.Split('|').ToList() : new List<string>());
            _settings.AutocompleteField = tabConfigurations.AutocompleteFieldName;
            return _settings;
        }

        private QueryFilter BuildQueryFilter()
        {
            QueryFilter query = new QueryFilter();
            query.Facets = QueryParser.ParseFacets(_parameters.SelectedFacets);
            query.FieldFiltersGroups = QueryParser.ParseFieldsFilters(_parameters.FieldsFilters);
            query.Page = _parameters.PageIndex;
            query.NoOfResults = _parameters.PageSize;
            query.QueryText = _parameters.Keyword;
            return query;
        }

        private void ValidateSearchConfigurations(SearchParameters parameters)
        {
            if (parameters.TabId == null)
                throw new Exception("Error: TabId is not set");
            Sitecore.Data.Database contextDb = Sitecore.Context.Database;
            if (contextDb == null)
                contextDb = Sitecore.Configuration.Factory.GetDatabase((String.IsNullOrEmpty(parameters.DatabaseName) ? parameters.DatabaseName : "web"));
            parameters.DatabaseName = contextDb.Name;
            Item tabItem = contextDb.GetItem(Sitecore.Data.ID.Parse(parameters.TabId));

            if (tabItem == null)
                throw new Exception("Error: Tab item does not exists in sitecore - " + contextDb.Name + " Database");
            if (tabItem.TemplateID != SearchTemplates.Tab.ID)
                throw new Exception("Error: Setting item template is incorrect");                        
        }

        private SearchConfigurations BuildSearchConfigurations(SearchParameters parameters)
        {
            Sitecore.Data.Database contextDb = Sitecore.Context.Database;
            if (contextDb == null)
                contextDb = Sitecore.Configuration.Factory.GetDatabase((String.IsNullOrEmpty(parameters.DatabaseName) ? parameters.DatabaseName : "web"));
            Item tabItem = contextDb.GetItem(Sitecore.Data.ID.Parse(parameters.TabId));
            
            Item configurationsItem = tabItem.Parent;
            
            SearchConfigurations configurations = new SearchConfigurations(configurationsItem, parameters.TabId);          

            return configurations;
        }

       

    }
}