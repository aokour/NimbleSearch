using NimbleSearch.Foundation.Abstractions.Pipelines.MapResult;

namespace NimbleSearch.Foundation.Core.Pipelines.MapResult
{
    public class SetTotalCount : MapResultProcessor
    {
        public override void Process(MapResultArgs args)
        {
            args.Response.TotalResults = args.TotalSearchResults;
        }
    }
}