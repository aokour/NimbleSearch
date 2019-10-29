namespace NimbleSearch.Foundation.Core.Models
{
    using System;
    using System.Collections.Generic;
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.SearchTypes;

  [Obsolete("Moved to Abstractions project", false)]
  public class IndexedItem : SearchResultItem
  {
    [IndexField(Templates.IndexedItem.Fields.IncludeInSearchResults_FieldName)]
    public bool ShowInSearchResults { get; set; }

    //[IndexField(Constants.IndexFields.AllTemplates)]
    public List<string> AllTemplates { get; set; }

    //[IndexField(Constants.IndexFields.IsLatestVersion)]
    public bool IsLatestVersion { get; set; }
  }
}