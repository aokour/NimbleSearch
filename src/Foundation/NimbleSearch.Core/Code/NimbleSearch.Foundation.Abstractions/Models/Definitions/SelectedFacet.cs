using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Abstractions.Models.Definitions
{
    [Serializable]
    public class SelectedFacet
    {
        public virtual Guid FacetId { get; set; }
        public virtual IEnumerable<string> SelectedValues { get; set; }
    }
}
