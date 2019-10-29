
using System.Collections.Generic;

namespace NimbleSearch.Foundation.Abstractions.Models.Response
{
    public class FacetValue
    {
        public FacetValue()
        {
            Custom = new Dictionary<string, object>();
        }

        public string Name { get; set; }
        public int Count { get; set; }
        public bool IsSelected { get; set; }
        public IDictionary<string, object> Custom { get; set; }
    }

}