namespace NimbleSearch.Foundation.Abstractions.Templates
{
    using Sitecore.Data;


    public struct Tab
    {
        public static readonly ID TemplateID = ID.Parse("{B94288AE-7FF3-452C-B842-7DEA212F210A}");

        public struct FieldName
        {
            public const string IndexName = "IndexName";
            public const string StartLocations = "StartLocations";
            public const string Templates = "Templates";
            public const string QueryFilter = "QueryFilter";
            public const string ApplySecurity = "ApplySecurity";
            public const string ApplyLatestVersion = "ApplyLatestVersion";
            public const string ApplyCurrentLanguage = "ApplyCurrentLanguage";

            public const string ViewOptions = "PageViews";
            public const string DefaultView = "DefaultPageView";

            public const string SortOptions = "SortingOptions";
            public const string DefaultSort = "DefaultSort";
            
            public const string ResultFields = "ResultFields";
            public const string NoResultsMessage = "EmptyResultsMessage";

            public const string RegisterSearchTerm = "RegisterSearchTerm";

        }
    }


}