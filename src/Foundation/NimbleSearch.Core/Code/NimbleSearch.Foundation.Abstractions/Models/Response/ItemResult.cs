using System;
using System.Collections.Generic;

namespace NimbleSearch.Foundation.Abstractions.Models.Response
{
    public class ItemResult
    {
        public ItemResult()
        {
            Custom = new Dictionary<string, object>();
        }

        public Guid ItemID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }

        public string Type { get; set; }
        public string ImageUrl { get; set; }

        public IDictionary<string, object> Custom { get; set; }
    }

}