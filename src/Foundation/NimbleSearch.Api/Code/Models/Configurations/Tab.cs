using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearchTemplates = NimbleSearch.Foundation.Api.Templates;

namespace NimbleSearch.Foundation.Api.Models.Configurations
{
   
    public class Tab
    {
        public Tab(Item TabItem, bool defaultTab = true)
        {
            IndexName = TabItem[SearchTemplates.Tab.Fields.IndexName];
            Title = TabItem[SearchTemplates.Tab.Fields.Title];
            QueryFilter = TabItem[SearchTemplates.Tab.Fields.QueryFilter];       
            BoostOnFields = TabItem[SearchTemplates.Tab.Fields.BoostOnFields];
            SearchAdditionalFields = TabItem[SearchTemplates.Tab.Fields.SearchAdditionalFields];
            Sitecore.Data.Fields.MultilistField startLocationsField = (Sitecore.Data.Fields.MultilistField)TabItem.Fields[SearchTemplates.Tab.Fields.StartLocations];
            StartLocations = startLocationsField.TargetIDs.ToList();
            AutocompleteFieldName = TabItem[SearchTemplates.Tab.Fields.AutocompleteFIeldName];
            if (defaultTab && TabItem.Fields[SearchTemplates.Tab.Fields.DefaultSort]!=null)
            {
                var defaultSortField = (Sitecore.Data.Fields.ReferenceField)TabItem.Fields[SearchTemplates.Tab.Fields.DefaultSort];
                if (defaultSortField != null && defaultSortField.TargetItem != null && defaultSortField.TargetItem.TemplateID == SearchTemplates.SortOption.ID)
                {
                    DefaultSortOption = new SortOption(defaultSortField.TargetItem);
                }
            }
            TabID = TabItem.ID;

            Sitecore.Data.Fields.MultilistField templatesField = (Sitecore.Data.Fields.MultilistField)TabItem.Fields[SearchTemplates.Tab.Fields.Templates];
            Templates = templatesField.TargetIDs.ToList();
            int size;
            Int32.TryParse(TabItem[SearchTemplates.Tab.Fields.PageSize], out size);
            RegisterSearchTerm = TabItem[SearchTemplates.Tab.Fields.RegisterSearchTerm] == "1";
            MoveToNextTabWhenEmpty = TabItem[SearchTemplates.Tab.Fields.MoveToNextTabWhenEmpty] == "1";
            NextTab = TabItem[SearchTemplates.Tab.Fields.NextTab];
            NoResultsMessage = TabItem[SearchTemplates.Tab.Fields.EmptyResultsMessage];
            PageSize = size;
            Facets = new List<Facet>();
            SortOptions = new List<SortOption>();
            ItemFields = new List<Field>();
            // CalculatedFields = new List<CalculatedField>();
            if (defaultTab)
            {
                foreach (Item item in TabItem.Axes.GetDescendants())
                {
                    if (item.TemplateID == SearchTemplates.Facet.ID)
                    {
                        Facets.Add(new Facet(item));
                    }
                    else if (item.TemplateID == SearchTemplates.SortOption.ID)
                    {
                        SortOptions.Add(new SortOption(item));
                    }
                    else if (item.TemplateID == SearchTemplates.Field.ID)
                    {
                        ItemFields.Add(new Field(item));
                    }
                    //else if (item.TemplateID == SearchTemplates.CalculatedField.ID)
                    //{
                    //    CalculatedFields.Add(new CalculatedField(item));
                    //}
                }
            }
        }
        public string IndexName { get; private set; }
        public ID TabID { get; set; }
        public string Title { get; private set; }
        public SortOption DefaultSortOption { get; private set; }
        public List<SortOption> SortOptions { get; set; }
        public List<ID> StartLocations { get; private set; }
        public List<ID> Templates { get; private set; }
        public string QueryFilter { get; private set; }
        public string BoostOnFields { get; private set; }
        public string SearchAdditionalFields { get; private set; }
        public int PageSize { get; private set; }
        public List<Facet> Facets { get; private set; }
        public List<Field> ItemFields { get; private set; }
        // public List<CalculatedField> CalculatedFields { get; private set; }
        public string AutocompleteFieldName{get; private set;}
        public bool RegisterSearchTerm { get; private set; }
        public bool MoveToNextTabWhenEmpty { get; private set; }
        public string NextTab { get; private set; }
        public string NoResultsMessage { get; private set; }

    }

}