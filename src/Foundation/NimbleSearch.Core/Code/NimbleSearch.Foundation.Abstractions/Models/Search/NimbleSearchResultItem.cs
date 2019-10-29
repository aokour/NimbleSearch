namespace NimbleSearch.Foundation.Abstractions.Models.Search
{
    using System;
    using System.Collections.Generic;
    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.SearchTypes;

      public class NimbleSearchResultItem : SearchResultItem
      {
        [IndexField(Templates.IndexedItem.FieldName.ExcludeFromSearchResults)]
        public bool ExcludeFromSearchResults { get; set; }

        [IndexField(Constants.IndexFields.AllTemplates)]
        public List<string> AllTemplates { get; set; }

        [IndexField(Constants.IndexFields.IsLatestVersion)]
        public bool IsLatestVersion { get; set; }



        [IndexField(Constants.IndexFields.NimbleName)]
        public string Title { get; set; }

        [IndexField(Constants.IndexFields.NimbleUrl)]
        public string ClickUrl { get; set; }

        [IndexField(Constants.IndexFields.NimbleImage)]
        public string ImageUrl { get; set; }

        [IndexField(Constants.IndexFields.NimbleSummary)]
        public string Summary { get; set; }


        [IndexField(Constants.IndexFields.NimbleNameSort)]
        public string NameSort { get; set; }

        [IndexField(Constants.IndexFields.NimbleDateSort)]
        public DateTime SortDate { get; set; }


    }
}