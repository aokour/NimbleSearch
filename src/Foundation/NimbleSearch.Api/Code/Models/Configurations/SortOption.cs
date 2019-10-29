using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearchTemplates = NimbleSearch.Foundation.Api.Templates;

namespace NimbleSearch.Foundation.Api.Models.Configurations
{
    public class SortOption
    {

        public SortOption(Item sortOptionItem)
        {
            Title = sortOptionItem[SearchTemplates.SortOption.Fields.Title];
            FieldName = sortOptionItem[SearchTemplates.SortOption.Fields.FieldName];
            DefaultSortDirection = sortOptionItem[SearchTemplates.SortOption.Fields.DefaultSortDirection];
        }

        public string Title { get; private set; }
        public string FieldName { get; private set; }
        public string DefaultSortDirection { get; private set; }
    }

}