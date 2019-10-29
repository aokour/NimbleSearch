using System;
using System.Collections.Generic;

namespace NimbleSearch.Foundation.Abstractions.Models.Definitions
{
    [Serializable]
    public class SearchParameters
    {
        public virtual Guid TabId { get; set; }
        public virtual string Keyword { get; set; }
        public virtual IEnumerable<SelectedFacet> SelectedFacets { get; set; }
        public virtual string SelectedFacetsRaw { get; set; }
        public virtual int Page { get; set; } // base 1
        public virtual int PageSize { get; set; }
        public virtual Guid Sort { get; set; }
        public virtual Guid View { get; set; }

        //public virtual bool ShowPopularTerms { get; set; }
        //public virtual string FieldsFilters { get; set; }  
        //public virtual string DatabaseName { get; set; }
    }
}
