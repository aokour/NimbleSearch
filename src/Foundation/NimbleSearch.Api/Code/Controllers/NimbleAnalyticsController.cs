using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Services;
using NimbleSearch.Foundation.Core.Services;
using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using NimbleSearch.Foundation.Abstractions.Models.Analytics;

namespace NimbleSearch.Foundation.Api.Controllers
{
    public class NimbleAnalyticsController : Controller
    {
        private INimbleService NimbleService;

        public NimbleAnalyticsController(INimbleService nimbleService)
        {
            NimbleService = nimbleService;
        }


        [System.Web.Mvc.HttpPost]
        public ActionResult Analytics(AnalyticsParameters parameters)
        {
            Item tabItem = null;
            string errMsg;
            var db = Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;

            if (parameters.SearchParameters != null)
            {
                var searchParams = parameters.SearchParameters;
                if (!TryGetTabById(searchParams.TabId, db, out tabItem, out errMsg))
                {
                    return Content(errMsg);
                }
                if ((searchParams.SelectedFacets == null || searchParams.SelectedFacets.Count() == 0) && !string.IsNullOrEmpty(searchParams.SelectedFacetsRaw))
                {
                    searchParams.SelectedFacets = ParseFacets(searchParams.SelectedFacetsRaw);
                }
                parameters.SearchParameters = searchParams;
            }

            NimbleService.PerformAnalytics(tabItem, parameters);
                            
            return Content("done");
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