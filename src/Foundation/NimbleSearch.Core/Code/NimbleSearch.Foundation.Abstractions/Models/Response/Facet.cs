using System;
using System.Collections.Generic;

namespace NimbleSearch.Foundation.Abstractions.Models.Response
{
    public class Facet
    {
        public Facet()
        {
            Custom = new Dictionary<string, object>();
        }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public string FieldName { get; set; }

        public bool CollapsedByDefault { get; set; }
        public IDictionary<string, object> Custom { get; set; }

        public IList<FacetValue> Values { get; set; }
    }
}