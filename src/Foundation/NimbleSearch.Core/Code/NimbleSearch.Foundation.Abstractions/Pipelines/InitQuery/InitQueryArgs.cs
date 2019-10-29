using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using Sitecore.ContentSearch.Security;
using Sitecore.Pipelines;
using System;

namespace NimbleSearch.Foundation.Abstractions.Pipelines.InitQuery
{
    public class InitQueryArgs : PipelineArgs
    {
        // Input
        public TabItem TabItem { get; set; }
        public SearchParameters SearchParameters { get; set; }

        // Output
        public string IndexName { get; set; }

        public SearchSecurityOptions SearchSecurityOption { get; set; }

    }
}