using NimbleSearch.Foundation.Abstractions.Pipelines.MapResult;

namespace NimbleSearch.Foundation.Core.Pipelines.MapResult
{
    public class SetTitle : MapResultProcessor
    {
        public override void Process(MapResultArgs args)
        {
            args.Response.TabTitle = args.TabItem.DisplayName;
        }
    }
}