using NimbleSearch.Foundation.Abstractions.Models.Analytics;
using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Models.Response;

namespace NimbleSearch.Foundation.Abstractions.Services
{
    public interface INimbleService
    {
        SearchResponse PerformSearch(TabItem tab, SearchParameters parameters);
        void PerformAnalytics(TabItem tab, AnalyticsParameters parameters);

    }
}