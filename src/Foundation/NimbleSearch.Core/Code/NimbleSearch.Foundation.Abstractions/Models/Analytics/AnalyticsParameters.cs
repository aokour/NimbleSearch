using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Models.Response;

namespace NimbleSearch.Foundation.Abstractions.Models.Analytics
{
    public class AnalyticsParameters
    {
        public SearchParameters SearchParameters { get; set; }
        public SearchResponse SearchResponse { get; set; }
    }
}
