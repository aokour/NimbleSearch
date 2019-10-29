using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearchTemplates = NimbleSearch.Foundation.Api.Templates;

namespace NimbleSearch.Foundation.Api.Models.Configurations
{
    public enum FacetSortDirection
    {
        Occurrences = 0,
        AlphabeticalAsc = 1,
        AlphabeticalDesc = 2
    }

    public class Facet
    {

        public Facet(Item FacetItem)
        {
            Name = FacetItem[SearchTemplates.Facet.Fields.Name];
            FieldName = FacetItem[SearchTemplates.Facet.Fields.FieldName];
            var sortBy = FacetItem[SearchTemplates.Facet.Fields.SortBy];
            if(string.IsNullOrWhiteSpace(sortBy) || sortBy == "Occurrences")
            {
                SortBy = FacetSortDirection.Occurrences;
            }
            else if(sortBy == "Alphabetical Asc")
            {
                SortBy = FacetSortDirection.AlphabeticalAsc;
            }
            else if (sortBy == "Alphabetical Desc")
            {
                SortBy = FacetSortDirection.AlphabeticalDesc;
            }
            else
            {
                SortBy = FacetSortDirection.Occurrences;
            }
        }
        public string Name { get; private set; }
        public string FieldName { get; private set; }
        public FacetSortDirection SortBy { get; private set; }
    }


}