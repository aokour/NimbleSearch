namespace NimbleSearch.Foundation.Abstractions.Templates
{
    using Sitecore.Data;

    public struct Facet
    {
        public static readonly ID TemplateID = ID.Parse("{F028B954-8A64-4E0D-B22D-5C8B20414134}");
        public struct FieldName
        {
            public const string IndexProperty = "FieldName";
            public const string MinimumValues = "MinimumValues";
            public const string LimitValues = "LimitValues";
            public const string CollapsedByDefault = "CollapsedByDefault";
            public const string AndValues = "AndValues";
            public const string SortBy = "SortBy";
        }

        public enum FacetSortDirection
        {
            Occurrences = 0,
            AlphabeticalAsc = 1,
            AlphabeticalDesc = 2
        }
    }
}