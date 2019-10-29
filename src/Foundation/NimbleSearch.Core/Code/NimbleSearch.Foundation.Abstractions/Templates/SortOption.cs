namespace NimbleSearch.Foundation.Abstractions.Templates
{
    using Sitecore.Data;

    public struct SortOption
    {
        public static readonly ID TemplateID = ID.Parse("{45B55AE2-1550-4B27-A888-B63A993AA75B}");

        public struct FieldName
        {
            public const string IsDescending = "IsDescending";
            public const string IndexProperty = "FieldName";
        }
    }

}