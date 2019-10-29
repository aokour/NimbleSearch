using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Services;
using NimbleSearch.Foundation.Core.Services;
using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;

namespace NimbleSearch.Foundation.Api.Controllers
{
    [RoutePrefix("api/nimble")]
    public class NimbleSearchController : BaseController
    {
        private INimbleService NimbleService;

        public NimbleSearchController(INimbleService nimbleService)
        {
            NimbleService = nimbleService;
        }


        [HttpPost]
        [Route("search")]
        public IHttpActionResult GetResults(SearchParameters parameters)
        {
            Item tabItem;
            string errMsg;
            var db = Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;
            if ( !TryGetTabById( parameters.TabId, db, out tabItem, out errMsg ) )
            {
                return InternalError(errMsg);
            }
            if((parameters.SelectedFacets==null || parameters.SelectedFacets.Count()== 0) && !string.IsNullOrEmpty(parameters.SelectedFacetsRaw))
            {
                parameters.SelectedFacets = ParseFacets(parameters.SelectedFacetsRaw);
            }


            var response = NimbleService.PerformSearch(tabItem, parameters);

                
            return OkOrNotFound(response);
        }

        // Test Endpoint
        [HttpGet]
        [Route("search/{tabId}")]
        public IHttpActionResult GetResults(Guid tabId, string keyword = "", int page = 1, int pageSize = 10)
        {
            SearchParameters parameters = new SearchParameters
            {
                TabId = tabId,
                Keyword = keyword,
                Page = page,
                PageSize = pageSize
                //,
                //SelectedFacets = new List<SelectedFacet> { new SelectedFacet {
                //    FacetId = new Guid("5e83e84c-3371-4e1d-8e0a-cd25afb74e71"),
                //    SelectedValues = new List<string>{
                //        "Employee",
                //        "News Article"
                //    }
                //} }
            };

            return GetResults(parameters);
        }



        // ==============================================================================
        //          Helpers
        // ==============================================================================

            

        protected virtual bool TryGetTabById(Guid tabId, Database db, out Item tabItem, out string errorMsg)
        {
            tabItem = null;
            errorMsg = string.Empty;

            if (db == null)
            { 
                errorMsg = "Database not defined";
                return false;
            }

            if (tabId == null || tabId == Guid.Empty)
            {
                errorMsg = "Tab Id is invalid";
                return false;
            }

            tabItem = db.GetItem(ID.Parse(tabId));

            if (tabItem == null || tabItem.TemplateID != Abstractions.Templates.Tab.TemplateID)
            {
                errorMsg = "Tab Id is invalid";
                return false;
            }

            return true;
        }

        protected virtual IEnumerable<SelectedFacet> ParseFacets(string selectedFacetsRaw)
        {
            string _facetsDelimiter = "||";
            char _facetDelimiter = ':';
            char _facetValuesDelimiter = '|';
            List<SelectedFacet> selectedFacets = new List<SelectedFacet>();
            if (selectedFacetsRaw == null) return selectedFacets;

            var facetsKeyValues = selectedFacetsRaw.Split(new string[] { _facetsDelimiter }, StringSplitOptions.None);

            foreach (var facetKeyValue in facetsKeyValues)
            {
                var keyValues = facetKeyValue.Split(_facetDelimiter);

                //skip corrupt values
                if (keyValues.Length == 2)
                {
                    Guid facetId;
                    if (Guid.TryParse(keyValues[0], out facetId))
                    {
                        SelectedFacet f = new SelectedFacet();
                        if (!selectedFacets.Any(x => x.FacetId== facetId))
                        {
                            f.FacetId = facetId;
                            f.SelectedValues = keyValues[1].Split(_facetValuesDelimiter);
                            selectedFacets.Add(f);
                        }
                    }
                }
            }
            return selectedFacets;
        }

    }
}