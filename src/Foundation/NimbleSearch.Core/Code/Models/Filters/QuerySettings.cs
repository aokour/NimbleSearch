using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Core.Models.Filters
{
    public class QuerySettings
    {
        public List<ID> StartRoots { get; set; }
        public List<ID> IncludedTemplates { get; set; }
        public bool ShowOnlyItemsWithPresentationDetails { get; set; }
        public bool MustHaveFormatter { get; set; }
        public string IndexName { get; set; }
        public string DatabaseName { get; set; }
        public List<string> Facets { get; set; }
        public string QueryFilter { get; set; }
        public List<string> BoostOnFields { get; set; }
        public List<string> SearchAdditionalFields { get; set; }

        public string SortByField { get; set; }
        public SortDirection SortDirection { get; set; }
        public string AutocompleteField { get; set; }
    }

    public enum SortDirection
    {
        Asc = 1,
        Desc = 2
    }
}