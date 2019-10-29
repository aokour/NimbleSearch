using NimbleSearch.Foundation.Abstractions.Pipelines.InitQuery;

namespace NimbleSearch.Foundation.Core.Pipelines.InitQuery
{
    public class SetIndexByTab : InitQueryProcessor
    {
        public override void Process(InitQueryArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.IndexName))
                return;

            // default
            args.IndexName = "sitecore_web_index";

            var index = args.TabItem?.IndexName;
            if (string.IsNullOrWhiteSpace(index))
                return;

            args.IndexName = index;            
        }
    }
}