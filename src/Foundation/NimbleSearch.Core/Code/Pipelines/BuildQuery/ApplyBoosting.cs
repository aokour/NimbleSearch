using NimbleSearch.Foundation.Abstractions.Pipelines.ApplyBoost;
using NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery;
using Sitecore.Pipelines;

namespace NimbleSearch.Foundation.Core.Pipelines.BuildQuery
{
    public class ApplyBoosting: BuildQueryProcessor
    {
        public override void Process(BuildQueryArgs args)
        {

            var boosts = args.TabItem?.Boosts;
            if (boosts == null || boosts.Length <= 0)
                return;

            var boostArgs = new ApplyBoostArgs
            {
                SearchParameters = args.SearchParameters,
                Query = args.Query
            };

            foreach (var boost in boosts)
            {
                boostArgs.BoostItem = boost;
                CorePipeline.Run(Abstractions.Constants.Pipeline.ApplyBoost, boostArgs);
            }
            args.Query = boostArgs.Query;
        }
    }
}