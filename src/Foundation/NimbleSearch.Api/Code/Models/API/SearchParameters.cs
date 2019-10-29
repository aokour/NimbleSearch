using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Api.Models.API
{
    public class SearchParameters
    {
        public virtual string Keyword { get; set; }
        public virtual bool ShowPopularTerms { get; set; }
        public virtual string SelectedFacets { get; set; }
        public virtual string FieldsFilters { get; set; }  
        public virtual int PageSize { get; set; }
        public virtual int PageIndex { get; set; }
        public virtual string SortBy { get; set; }
        public virtual string SortDirection { get; set; }
        public virtual Guid TabId { get; set; }
        public virtual string DatabaseName { get; set; }
    }
}
