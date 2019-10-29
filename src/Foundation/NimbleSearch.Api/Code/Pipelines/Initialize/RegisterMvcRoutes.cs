using Sitecore.Pipelines;
using System.Web.Mvc;
using System.Web.Routing;

namespace NimbleSearch.Foundation.Api.Pipelines.Initialize
{
    public class RegisterMvcRoutes
    {
        public virtual void Process(PipelineArgs args)
        {
            RouteTable.Routes.MapRoute("NimbleAnalytics", "api/nimble/analytics", new {
                controller = "NimbleAnalytics",
                action = "Analytics"
            });
        }
    }
}