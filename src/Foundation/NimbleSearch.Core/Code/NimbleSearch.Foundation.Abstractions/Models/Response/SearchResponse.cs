using System.Collections.Generic;

namespace NimbleSearch.Foundation.Abstractions.Models.Response
{
    public class SearchResponse
    {
        public SearchResponse() {
            Custom = new Dictionary<string, object>();
        }

        public long Duration { get; set; }
        public int TotalResults { get; set; }

        public IList<ItemResult> Items { get; set; }

        public Paging Paging { get; set; }
        public IList<SortOption> SortOptions { get; set; }
        public IList<ViewOption> ViewOptions { get; set; }


        public IList<Facet> Facets { get; set; }


        public string TabTitle { get; set; }
        public string NoResultsMessage { get; set; }

        //public bool MoveToNextTabWhenNoResults { get; set; }
        //public string NextTab { get; set; }
        // public List<string> PopularTerms { get; set; }


        public IDictionary<string, object> Custom { get; set; }
    }
}