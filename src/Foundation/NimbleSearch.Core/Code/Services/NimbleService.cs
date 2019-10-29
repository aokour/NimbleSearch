using NimbleSearch.Foundation.Abstractions.Models.Analytics;
using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Models.Response;
using NimbleSearch.Foundation.Abstractions.Models.Search;
using NimbleSearch.Foundation.Abstractions.Pipelines.ApplyFacet;
using NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery;
using NimbleSearch.Foundation.Abstractions.Pipelines.InitQuery;
using NimbleSearch.Foundation.Abstractions.Pipelines.MapResult;
using NimbleSearch.Foundation.Abstractions.Pipelines.SearchAnaltyics;
using NimbleSearch.Foundation.Abstractions.Services;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using System.Collections.Generic;
using System.Linq;
using Pipeline = NimbleSearch.Foundation.Abstractions.Constants.Pipeline;

namespace NimbleSearch.Foundation.Core.Services
{
    public class NimbleService : INimbleService
    {
        public SearchResponse PerformSearch(TabItem tab, SearchParameters parameters)
        {
            Assert.ArgumentNotNull(tab, nameof(tab));
            Assert.ArgumentNotNull(parameters, nameof(parameters));

            // Init Query
            var initArgs = new InitQueryArgs
            {
                TabItem = tab,
                SearchParameters = parameters
            };
            CorePipeline.Run(Pipeline.InitQuery, initArgs);
            Assert.ArgumentNotNullOrEmpty(initArgs.IndexName, nameof(initArgs.IndexName));

            var searchIndex = ContentSearchManager.GetIndex(initArgs.IndexName);
            using (var context = searchIndex.CreateSearchContext(initArgs.SearchSecurityOption))
            {

                var query = context.GetQueryable<SearchResultItem>();

                var watch = System.Diagnostics.Stopwatch.StartNew();
                
                    // Build Query
                    var queryArgs = new BuildQueryArgs
                    {
                        TabItem = tab,
                        SearchParameters = parameters,
                        Query = query,
                        SearchContext = context
                    };
                    CorePipeline.Run(Pipeline.BuildQuery, queryArgs);
                    query = queryArgs.Query;

                    // Apply Selected Facets
                    var selectedFacets = GetSelectedFacets(parameters.SelectedFacets, tab.Facets);
                    var finalQuery = ApplySelectedFacets(query, selectedFacets, parameters);
                
                    // Add common (unselected) facet-ons to lessen additional queries
                    var unselectedFacets = GetUnselectedFacets(parameters.SelectedFacets, tab.Facets);
                    finalQuery = ApplyFacetOn(finalQuery, unselectedFacets, parameters);
                                               
                    // Execute
                    var results = finalQuery.Cast<NimbleSearchResultItem>().GetResults();

                    // Correct facet counts (for selected facets only)
                    var facets = CorrectFacetCounts(query, selectedFacets, parameters, results.Facets?.Categories);

                
                watch.Stop();

               

                // Map Results
                var resultArgs = new MapResultArgs
                {
                    TabItem = tab,
                    SearchParameters = parameters,
                    TotalSearchResults = results.TotalSearchResults,
                    Hits = results.Hits,
                    FacetResults = facets,
                    QueryDuration = watch.ElapsedMilliseconds
                };
                CorePipeline.Run(Pipeline.MapResult, resultArgs);

                return resultArgs.Response;
            }

        }

        public void PerformAnalytics(TabItem tab, AnalyticsParameters parameters)
        {
            Assert.ArgumentNotNull(tab, nameof(tab));
            Assert.ArgumentNotNull(parameters, nameof(parameters));

            // Init Query
            var args = new SearchAnalyticsArgs
            {
                TabItem = tab,
                SearchParameters = parameters.SearchParameters ?? new SearchParameters(),
                SearchResponse = parameters.SearchResponse ?? new SearchResponse()
            };
            CorePipeline.Run(Pipeline.SearchAnalytics, args);
        }

        // ======================================================================================================

        public IQueryable<SearchResultItem> ApplySelectedFacets(IQueryable<SearchResultItem> query, FacetItem[] facets, SearchParameters searchParameters) {

            if (facets == null || !facets.Any())
                return query;

            // Run Facets
            var facetArgs = new ApplyFacetArgs
            {
                SearchParameters = searchParameters,
                Query = query
            };

            foreach (var facet in facets)
            {
                facetArgs.FacetItem = facet;
                CorePipeline.Run(Pipeline.ApplySelectedFacet, facetArgs);
            }
            return facetArgs.Query;
        }


        public IQueryable<SearchResultItem> ApplyFacetOn(IQueryable<SearchResultItem> query, FacetItem[] facets, SearchParameters searchParameters)
        {
            if (facets == null || !facets.Any())
                return query;

            // Run Facets
            var facetArgs = new ApplyFacetArgs
            {
                SearchParameters = searchParameters,
                Query = query
            };

            foreach (var facet in facets)
            {
                facetArgs.FacetItem = facet;
                CorePipeline.Run(Pipeline.ApplyFacetOn, facetArgs);
            }
            return facetArgs.Query;
        }

        /// <summary>
        /// Getting Facet Data from a single query is not good. When selected facets are applied to the query it limits the counts we want to see.
        /// So we need to run a query for every selected facet without the selected value clauses applied so we get the desired counts for all values of that facet
        /// </summary>
        /// <param name="query">same query as search results except for selected Facets clauses nor FacetOns for selected values</param>
        /// <param name="facets">Selected Facets only</param>
        /// <param name="searchParameters"></param>
        /// <param name="facetResults">facet results of final search result query... to update facet counts of selected facets</param>
        /// <returns></returns>
        public List<FacetCategory> CorrectFacetCounts(IQueryable<SearchResultItem> query, FacetItem[] facets, SearchParameters searchParameters, List<FacetCategory> facetResults)
        {
            if (facetResults == null)
                facetResults = new List<FacetCategory>();

            if (facets == null || !facets.Any())
                return facetResults;
            
            // Loop Selected facets
            foreach (var facet in facets)
            {
                var otherFacets = facets.Where(x => x.ID != facet.ID).ToArray();

                var countQuery = ApplySelectedFacets(query, otherFacets, searchParameters);
                countQuery = ApplyFacetOn(countQuery, new[] { facet }, searchParameters);
                var results = countQuery.GetFacets();

                // Usually only one to add
                if(results.Categories != null)
                { 
                    foreach( var correctCount in results.Categories )
                    { 
                        facetResults.Add(correctCount);
                    }
                }
            }
            
            return facetResults;
        }


        public FacetItem[] GetSelectedFacets(IEnumerable<SelectedFacet> selectedFacets, FacetItem[] facets)
        {
            if (selectedFacets == null || !selectedFacets.Any())
                return null;
            if (facets == null || !facets.Any())
                return null;

            return facets.Where(x => selectedFacets.Any(y => y.FacetId == x.ID.Guid)).ToArray();
        }
        public FacetItem[] GetUnselectedFacets(IEnumerable<SelectedFacet> selectedFacets, FacetItem[] facets)
        {
            if (selectedFacets == null || !selectedFacets.Any())
                return facets;
            if (facets == null || !facets.Any())
                return null;

            return facets.Where(x => !selectedFacets.Any(y => y.FacetId == x.ID.Guid)).ToArray();
        }
    }
}