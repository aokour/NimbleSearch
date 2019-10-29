using NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery;
using System.Linq;

namespace NimbleSearch.Foundation.Core.Pipelines.BuildQuery
{
    public class ApplyKeywords: BuildQueryProcessor
    {
        public override void Process(BuildQueryArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.SearchParameters.Keyword))
                return;

            // TODO: Get keyword fields from tab
            // default _content

            args.Query = args.Query.Where(i => i.Content.Contains(args.SearchParameters.Keyword));
        }
    }
}