using NimbleSearch.Foundation.Abstractions.Models.Search;
using NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery;
using Sitecore.ContentSearch.Linq;
using System.Linq;

namespace NimbleSearch.Foundation.Core.Pipelines.BuildQuery
{
    public class ApplyContextLanguage : BuildQueryProcessor
    {
        public override void Process(BuildQueryArgs args)
        {
            if(args.TabItem?.ApplyCurrentLanguage ?? false)
            {
                var lang = Sitecore.Context.Language?.Name;
                if(!string.IsNullOrWhiteSpace(lang))
                { 
                    args.Query = args.Query.Cast<NimbleSearchResultItem>().Filter(x => x.Language == lang);
                }
            }
        }
    }
}