using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearchTemplates = NimbleSearch.Foundation.Api.Templates;

namespace NimbleSearch.Foundation.Api.Models.Configurations
{
    public class SearchConfigurations
    {

        public SearchConfigurations(Item tabItem) 
            : this(tabItem?.Parent, tabItem?.ID.Guid ?? Guid.Empty)
        {}

        public SearchConfigurations(Item searchConfigItem, Guid selectedTab )
        {
            ID selectedTabID = ID.Parse(selectedTab);
            
            Tabs = new List<Tab>();
            foreach (Item item in searchConfigItem.Children)
            {
                if (item.TemplateID == SearchTemplates.Tab.ID)
                {
                    if(selectedTabID == item.ID)
                        Tabs.Add(new Tab(item));
                    else
                        Tabs.Add(new Tab(item, false));
                }
            }
        }

       
        public List<Tab> Tabs { get; private set; }

    }

}