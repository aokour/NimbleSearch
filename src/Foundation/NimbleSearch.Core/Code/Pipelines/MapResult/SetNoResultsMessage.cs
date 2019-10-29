using NimbleSearch.Foundation.Abstractions.Pipelines.MapResult;

namespace NimbleSearch.Foundation.Core.Pipelines.MapResult
{
    public class SetNoResultsMessage : MapResultProcessor
    {
        public override void Process(MapResultArgs args)
        {
            if (args.TotalSearchResults <= 0)
            {
                args.Response.NoResultsMessage = args.TabItem.NoResultsHTML;
            }
        }
    }
}