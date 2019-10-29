using NimbleSearch.Foundation.Abstractions.Pipelines.MapResult;

namespace NimbleSearch.Foundation.Core.Pipelines.MapResult
{
    public class SetDuration : MapResultProcessor
    {
        public override void Process(MapResultArgs args)
        {
            args.Response.Duration = args.QueryDuration;
        }
    }
}