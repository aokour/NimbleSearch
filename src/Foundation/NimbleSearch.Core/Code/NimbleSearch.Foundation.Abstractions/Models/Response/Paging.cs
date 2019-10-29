
using System.Collections.Generic;

namespace NimbleSearch.Foundation.Abstractions.Models.Response
{
    public class Paging
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public IList<PagingOption> PagingSizes { get; set; }

    }
}