using NimbleSearch.Foundation.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Core.Models.Filters
{
    public class QueryFilter : IQuery
    {
        public Dictionary<string, string[]> Facets
        {
            get; set;
        }

        public int NoOfResults
        {
            get; set;
        }

        public int Page
        {
            get; set;
        }

        public string QueryText
        {
            get; set;
        }

        public List<FieldFilterGroup> FieldFiltersGroups { get; set; }

    }
  
}