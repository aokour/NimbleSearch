using Microsoft.Extensions.DependencyInjection;
using NimbleSearch.Foundation.Api.Controllers;
using Sitecore.DependencyInjection;

namespace NimbleSearch.Foundation.Api.IoC
{
    public class ApiConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(NimbleSearchController));
            serviceCollection.AddTransient(typeof(NimbleAnalyticsController));
        }
    }
}