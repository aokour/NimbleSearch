namespace NimbleSearch.Foundation.Core
{
    using Sitecore.Data;

    public struct Templates
    {
        public struct ListShort
        {
            public static ID ID = new ID("{667A8E10-76DA-4E49-96E9-52CD2393317D}");
        }
        public struct ListLong
        {
            public static ID ID = new ID("{E87E7D2E-1E30-4535-911F-609DDBE4AAB2}");
        }

        public struct IndexedItem
        {
            public static ID ID = new ID("{8FD6C8B6-A9A4-4322-947E-90CE3D94916D}");

            public struct Fields
            {
                public static readonly ID IncludeInSearchResults = new ID("{8D5C486E-A0E3-4DBE-9A4A-CDFF93594BDA}");
                public const string IncludeInSearchResults_FieldName = "IncludeInSearchResults";
                public const string ExcludeFromSearchResults = "NimbleExcludeFromSearch";
            }
        }
        public struct Index
        {
            public struct Fields
            {
                public static readonly string LocalDatasourceContent_IndexFieldName = "local_datasource_content";
                public static readonly string AllContent = "_content";
            }
        }

    }
}