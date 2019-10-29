namespace NimbleSearch.Foundation.Core
{
    using NimbleSearch.Foundation.Core.Configuration;

    public struct Settings
    {
        public static readonly SimpleFacets SimpleFacets = Sitecore.Configuration.Factory.CreateObject("nimble/simpleFacets", true) as SimpleFacets;
    }
}