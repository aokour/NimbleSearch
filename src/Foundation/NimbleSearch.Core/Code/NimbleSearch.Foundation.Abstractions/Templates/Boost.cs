namespace NimbleSearch.Foundation.Abstractions.Templates
{
    using Sitecore.Data;

    public struct Boost
    {
        public static readonly ID TemplateID = ID.Parse("{DDE3ABBE-94A9-4F3F-B9D2-37EDC79EEDA2}");
        public struct FieldName
        {
            public const string BoostField = "BoostField";
            public const string BoostAmount = "BoostAmount";
        }
    }

}