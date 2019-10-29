namespace NimbleSearch.Foundation.Abstractions.Templates
{
    using Sitecore.Data;
    
    public struct IndexedItem
    {
        public static readonly ID ID = new ID("{EE18AB0F-CC97-4DB6-B961-0E8B573C337E}");

        public struct FieldName
        {
            public const string ExcludeFromSearchResults = "NimbleExcludeFromSearch";
        }
    }
}