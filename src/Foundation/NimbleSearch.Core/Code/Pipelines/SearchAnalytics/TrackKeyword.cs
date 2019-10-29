using NimbleSearch.Foundation.Abstractions.Pipelines.SearchAnaltyics;
using Sitecore.Analytics;
using Sitecore.Analytics.Data;

namespace NimbleSearch.Foundation.Core.Pipelines.SearchAnalytics
{
    public class TrackKeyword : SearchAnaltyicsProcessor
    {
        public override void Process(SearchAnalyticsArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.SearchParameters.Keyword))
                return;

            if (!Tracker.IsActive)
            {
                if (!Tracker.Enabled)
                    return;
                Tracker.StartTracking();
            }
            
            var keywords = args.SearchParameters.Keyword;
            var pageEventData = new PageEventData("Search", AnalyticsIds.SearchEvent.Guid)
            {
                ItemId = args.TabItem.ID.Guid,
                Data = keywords,
                DataKey = keywords,
                Text = keywords
            };
            var interaction = Tracker.Current?.Session?.Interaction;
            if (interaction != null)
            {
                interaction.CurrentPage.Register(pageEventData);
            }
            
        }
    }
}