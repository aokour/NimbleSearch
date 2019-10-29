using NimbleSearch.Foundation.Abstractions.Pipelines.ApplyBoost;
using Sitecore.ContentSearch.Linq;
using Sitecore.Diagnostics;
using System.Linq;
using BoostTemplate = NimbleSearch.Foundation.Abstractions.Templates.Boost;

namespace NimbleSearch.Foundation.Core.Pipelines.ApplyBoost
{
    public class SimpleBoost : ApplyBoostProcessor
    {
        public override void Process(ApplyBoostArgs args)
        {
            if (args.BoostItem == null || args.BoostItem.TemplateID != BoostTemplate.TemplateID)
                return;

            var field = args.BoostItem[BoostTemplate.FieldName.BoostField];
            if (string.IsNullOrWhiteSpace(field))
            {
                Log.Warn($"Nimble:Boost:SimpleBoost: Missing Expected Property name on item {args.BoostItem.DisplayName}, {args.BoostItem.ID}", this);
                return;
            }
            float amount;
            if (!float.TryParse(args.BoostItem[BoostTemplate.FieldName.BoostAmount], out amount))
            {
                Log.Warn($"Nimble:Boost:SimpleBoost: Unable to parse boost amount on item {args.BoostItem.DisplayName}, {args.BoostItem.ID}", this);
                return;
            }

            args.Query = args.Query.Where(x => 
                x[field].Contains(args.SearchParameters.Keyword).Boost(amount)
            );
        }
        
    }
}