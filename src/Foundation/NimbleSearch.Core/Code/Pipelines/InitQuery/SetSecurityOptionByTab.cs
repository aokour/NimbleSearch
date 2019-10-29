using NimbleSearch.Foundation.Abstractions.Pipelines.InitQuery;
using Sitecore.ContentSearch.Security;

namespace NimbleSearch.Foundation.Core.Pipelines.InitQuery
{
    public class SetSecurityOptionByTab : InitQueryProcessor
    {
        public override void Process(InitQueryArgs args)
        {
            args.SearchSecurityOption = (args.TabItem?.ApplySecurity ?? false) ? 
                SearchSecurityOptions.EnableSecurityCheck :
                SearchSecurityOptions.DisableSecurityCheck;
        }
    }
}