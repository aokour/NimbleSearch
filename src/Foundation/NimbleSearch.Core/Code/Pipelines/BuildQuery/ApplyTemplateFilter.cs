using NimbleSearch.Foundation.Abstractions.Models.Search;
using NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.Utilities;
using System.Linq;

namespace NimbleSearch.Foundation.Core.Pipelines.BuildQuery
{
    public class ApplyTemplateFilter : BuildQueryProcessor
    {
        public override void Process(BuildQueryArgs args)
        {
            // If tab defines templates...
            var templates = args.TabItem?.TemplateIDs;
            if (templates == null || templates.Length <= 0)
                return;

            // Apply template filter
            var templatePredicates = PredicateBuilder.False<NimbleSearchResultItem>();
            foreach (var templateId in templates)
            {
                var templateGuid = IdHelper.NormalizeGuid(templateId);
                templatePredicates = templatePredicates.Or(x => x.AllTemplates.Contains(templateGuid));
            }

            args.Query = args.Query.Cast<NimbleSearchResultItem>().Filter(templatePredicates);            
        }
    }
}