using System.Collections.Generic;

namespace NimbleSearch.Foundation.Abstractions
{
    public struct Constants
    {
        public struct Pipeline
        {
            public const string InitQuery = "nimble.initQuery";

            public const string BuildQuery = "nimble.buildQuery";
            public const string ApplyFacetOn = "nimble.applyFacetOn";
            public const string ApplySelectedFacet = "nimble.applySelectedFacet";
            public const string ApplyBoost = "nimble.applyBoost";

            public const string MapResult = "nimble.mapResult";
            public const string SearchAnalytics = "nimble.searchAnalytics";
        }
        public struct IndexFields
        {
            public const string IsLatestVersion = "_latestversion";
            public const string HasPresentation = "has_presentation";
            public const string AllTemplates = "all_templates";

            public const string HasSearchResultFormatter = "has_search_result_formatter";
            public const string ContentType = "content_type";
            public const string SearchResultFormatter = "search_result_formatter";
            

            public const string NimbleName = "nimblename";
            public const string NimbleUrl= "nimbleurl";
            public const string NimbleImage = "nimbleimage";
            public const string NimbleSummary = "nimblesummary";
            public const string NimbleNameSort = "nimblenamesort";
            public const string NimbleDateSort = "nimbledatesort";
            
        }

        /// <summary>
        /// Convenient list of system sites that can be ignored for many process
        /// </summary>
        public static readonly List<string> SystemSites = new List<string> {
            "shell", "login", "admin", "service", "modules_shell", "modules_website", "scheduler", "system", "publisher", "exm",
            "coveoapi", "coveo_website", "coveoanalytics", "coveorest",
            "unicorn"
        };

    }
}