namespace NimbleSearch.Foundation.Abstractions.Templates
{
    using Sitecore.Data;

    public struct ViewOption
    {
        public static readonly ID TemplateID = ID.Parse("{7C9BF0C8-7EDA-46D4-8BD6-8FE772CFBFD2}");

        public struct FieldName
        {
            public const string ClassName = "ClassName";
            public const string PageSizeOptions = "PageSizeOptions";
        }
    }
}