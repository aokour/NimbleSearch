using NimbleSearch.Foundation.Api.Fields;
using NimbleSearch.Foundation.Api.Models.API;
using NimbleSearch.Foundation.Api.Models.Configurations;
using Sitecore.Data;
using Sitecore.Data.Items;
using NimbleSearch.Foundation.Core.Models;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NimbleSearch.Foundation.Api.Util;
using Sitecore.Resources.Media;

namespace NimbleSearch.Foundation.Api.Mapper
{
    public static class SearchResultMapper
    {
        public static NimbleSearch.Foundation.Api.Models.API.SearchResult Map(ISearchResults searchResult, SearchParameters parameters, SearchConfigurations configurations, Guid selectedTabId, int currentPage, bool isAutoComplete)
        {
            NimbleSearch.Foundation.Api.Models.API.SearchResult apiSearchResult = new Models.API.SearchResult();
            Sitecore.Data.ID tabId = ID.Parse(selectedTabId);
            var defaultTab = configurations.Tabs.Where(i => i.TabID == tabId).FirstOrDefault();
            if (defaultTab == null)
                return apiSearchResult;
            apiSearchResult.TotalNumberOfResults = searchResult.TotalNumberOfResults;

            if (parameters.PageSize == 0)
                parameters.PageSize = defaultTab.PageSize;

            apiSearchResult.TotalPages = (int)Math.Ceiling((double)searchResult.TotalNumberOfResults / parameters.PageSize);
            apiSearchResult.CurrentPageIndex = currentPage;
            apiSearchResult.PageSize = parameters.PageSize;
            apiSearchResult.DocumentStartIndex = (apiSearchResult.PageSize * (apiSearchResult.CurrentPageIndex + 1)) - apiSearchResult.PageSize + 1;
            apiSearchResult.DocumentEndIndex = (apiSearchResult.PageSize * (apiSearchResult.CurrentPageIndex + 1));
            if(apiSearchResult.DocumentEndIndex> apiSearchResult.TotalNumberOfResults)
            {
                apiSearchResult.DocumentEndIndex = apiSearchResult.TotalNumberOfResults;
            }
            apiSearchResult.SortOptions = !isAutoComplete ? BuildSortOptions(defaultTab): new List<Models.API.SortOption>();
            apiSearchResult.Facets = !isAutoComplete ? BuildFacets(searchResult, defaultTab) : new List<FacetResult>();
            apiSearchResult.Tabs = BuildTabs(configurations, searchResult, tabId);
            apiSearchResult.NoResultsMessage = defaultTab.NoResultsMessage;
            apiSearchResult.MoveToNextTabWhenNoResults = defaultTab.MoveToNextTabWhenEmpty;
            apiSearchResult.NextTab = defaultTab.NextTab;
            apiSearchResult.Items = BuildItemResult(searchResult, defaultTab);
            apiSearchResult.DefaultSortOption = !isAutoComplete && defaultTab.DefaultSortOption != null ? (apiSearchResult.SortOptions.Where(x => x.Value == defaultTab.DefaultSortOption.FieldName).FirstOrDefault()) : null ;
            apiSearchResult.Duration = searchResult.Duration;
            return apiSearchResult;
        }

        private static List<TabItem> BuildTabs(SearchConfigurations configurations, ISearchResults searchResult, ID selectedTabId)
        {
            var Tabs = new List<TabItem>();
            foreach (var tab in configurations.Tabs)
            {
                TabItem tabResult = new TabItem();
                tabResult.ID = tab.TabID;
                tabResult.Selected = (tabResult.ID == selectedTabId);
                tabResult.Title = tab.Title;
                if (tabResult.Selected)
                {
                    tabResult.Count = searchResult.Results.Count(); //TO DO : calculate count for each tab
                }
                Tabs.Add(tabResult);
            }
            return Tabs;
        }
        
        private static List<Models.API.FacetResult> BuildFacets(ISearchResults searchResult, Tab tab)
        {
            var facetResults = new List<Models.API.FacetResult>();
            foreach (var facetConfiguration in tab.Facets)
            {
                ISearchResultFacet facet = searchResult.Facets.Where(x => x.Definition.FieldName == facetConfiguration.FieldName).FirstOrDefault();
                if (facet != null)
                {
                    FacetResult facetResult = new FacetResult();
                    facetResult.Values = new List<FacetValue>();
                    facetResult.Name = facetConfiguration.Name;
                    facetResult.FieldName = facetConfiguration.FieldName;

                    foreach (var facetValue in facet.Values)
                    {
                        FacetValue facetResultValue = new FacetValue();
                        facetResultValue.Count = facetValue.Count;
                        facetResultValue.Name = facetValue.Value.ToString();
                        facetResultValue.Selected = facetValue.Selected;
                        facetResultValue.Value = facetValue.Value.ToString();
                        facetResult.Values.Add(facetResultValue);
                    }
                    facetResult = SortFacetValues(facetConfiguration, facetResult);
                    facetResults.Add(facetResult);
                }
            }
            return facetResults;
        }

        private static FacetResult SortFacetValues(Facet facetConfiguration, FacetResult facetResult )
        {
            if (facetConfiguration.SortBy == FacetSortDirection.Occurrences)
            {
                facetResult.Values = facetResult.Values.OrderByDescending(x => x.Count).ToList();
            }
            else if (facetConfiguration.SortBy == FacetSortDirection.AlphabeticalAsc)
            {
                facetResult.Values = facetResult.Values.OrderBy(x => x.Value, new SemiNumericComparer()).ToList();
            }
            else if (facetConfiguration.SortBy == FacetSortDirection.AlphabeticalDesc)
            {
                facetResult.Values = facetResult.Values.OrderByDescending(x => x.Value, new SemiNumericComparer()).ToList();
            }

            return facetResult;
        }



        private static List<Models.API.SortOption> BuildSortOptions(Tab tab)
        {
            List<Models.API.SortOption> sortOptions = new List<Models.API.SortOption>();
            if(tab.SortOptions.Any())
            {
                foreach(var option in tab.SortOptions)
                {
                    Models.API.SortOption sortOption = new Models.API.SortOption();
                    sortOption.Name= option.Title;
                    sortOption.Value = option.FieldName;
                    sortOption.Direction = option.DefaultSortDirection;
                    sortOptions.Add(sortOption);
                }
            }
            return sortOptions;
        }

        private static List<ItemResult> BuildItemResult(ISearchResults searchResult, Tab tab)
        {
            var itemsList = new List<ItemResult>();
            foreach (var resultRow in searchResult.Results)
            {
                if (resultRow.Item != null)
                {
                    ItemResult itemResult = new ItemResult();
                    if (resultRow.Item != null)
                    {
                        itemResult.Url = GetItemUrl(resultRow.Item);
                        itemResult.ItemID = resultRow.Item.ID;
                        itemResult.ItemName = resultRow.Item.Name;
                        itemResult.ItemPath = resultRow.Item.Paths.FullPath;
                        itemResult.ItemTemplateName = resultRow.Item.TemplateName;
                        itemResult.ItemTemplateId = resultRow.Item.TemplateID;
                        itemResult.Properties = new Dictionary<string, string>();

                        if (tab.ItemFields.Any())
                        {
                            itemResult.Properties = BuildDynamicFields(resultRow.Item, tab.ItemFields);                            
                        }
                        //if(tab.CalculatedFields.Any())
                        //{
                        //    var calculatedProperties = BuildCalculatedFields(resultRow.Item, tab, tab.CalculatedFields);
                        //    foreach (var keyValuePair in calculatedProperties)
                        //    {
                        //        if(!itemResult.Properties.ContainsKey(keyValuePair.Key))
                        //        {
                        //            itemResult.Properties.Add(keyValuePair.Key, keyValuePair.Value);
                        //        }
                        //    }
                        //}
                    }
                    itemsList.Add(itemResult);
                }
            }

            return itemsList;
        }

        private static string GetItemUrl(Item item)
        {
            if (item.Paths.IsMediaItem)
            {
                return GetMediaItemUrl(item);// Sitecore.Resources.Media.MediaManager.GetMediaUrl(item);
            }
            else
            {
                return LinkManager.GetItemUrl(item);
            }
        }

        private static Dictionary<string, string> BuildDynamicFields(Item item, List<Field> dynamicFieldsNames)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            foreach (var field in dynamicFieldsNames)
            {
                string referenceItemFieldName = field.ReferencedItemFieldName;
                string dynamicFieldName = field.FieldName;                

                properties.Add(dynamicFieldName, string.Empty);
                if (!string.IsNullOrWhiteSpace(dynamicFieldName) && item.Fields[dynamicFieldName] != null && !string.IsNullOrEmpty(item.Fields[dynamicFieldName].Value))
                {
                    var dynamicField = item.Fields[dynamicFieldName];

                    if (dynamicField.Type == "Image")
                    {
                        var mediaItem = ((Sitecore.Data.Fields.ImageField)dynamicField).MediaItem;
                        if (mediaItem != null)
                        {
                            string imageUrl = GetItemUrl(mediaItem);// Sitecore.Resources.Media.MediaManager.GetMediaUrl(mediaItem);
                            if (!string.IsNullOrWhiteSpace(field.Parameters))
                            {
                                var parameters = HttpUtility.ParseQueryString(field.Parameters);
                                foreach(var paramKey in parameters.AllKeys)
                                {
                                    if(paramKey=="w")
                                    {
                                        imageUrl += (imageUrl.Contains("?h=") ? "&w=" : "?w=") + parameters[paramKey];
                                    }
                                    if (paramKey == "h")
                                    {
                                        imageUrl +=  (imageUrl.Contains("?w=") ? "&h=" : "?h=") + parameters[paramKey];
                                    }
                                    imageUrl = Sitecore.Resources.Media.HashingUtils.ProtectAssetUrl(imageUrl);
                                }
                            }
                            properties[dynamicFieldName] = imageUrl;
                        }
                    }
                    else if (dynamicField.Type == "Date" || dynamicField.Type == "Datetime")
                    {
                        properties[dynamicFieldName] = ((Sitecore.Data.Fields.DateField)dynamicField).DateTime.ToString();
                    }
                    else if (dynamicField.Type == "Droplink" || dynamicField.Type == "Droptree")
                    {
                        var referenceItem = ((Sitecore.Data.Fields.ReferenceField)dynamicField).TargetItem;
                        if (referenceItem != null)
                            properties[dynamicFieldName] = ( (!string.IsNullOrEmpty(referenceItemFieldName) && referenceItem.Fields[referenceItemFieldName]!=null)? referenceItem.Fields[referenceItemFieldName].Value:
                                                            (!string.IsNullOrEmpty(referenceItem.DisplayName) ? referenceItem.DisplayName : referenceItem.Name));
                    }
                    else if (dynamicField.Type == "Multilist" || dynamicField.Type == "Multilist with Search"
                            || dynamicField.Type == "Checklist" || dynamicField.Type == "Treelist" || dynamicField.Type == "TreelistEx")
                    {
                        var multilistField = ((Sitecore.Data.Fields.MultilistField)dynamicField);
                        var listValues = new StringBuilder();
                        string delimiter = string.Empty;
                        if (multilistField != null)
                        {
                            foreach (var referenceItem in multilistField.GetItems())
                            {
                                if (referenceItem != null)
                                {
                                    listValues.Append(delimiter + ((!string.IsNullOrEmpty(referenceItemFieldName) && referenceItem.Fields[referenceItemFieldName] != null) ? referenceItem.Fields[referenceItemFieldName].Value :
                                                            (!string.IsNullOrEmpty(referenceItem.DisplayName) ? referenceItem.DisplayName : referenceItem.Name)) );
                                    if(string.IsNullOrEmpty(delimiter))
                                        delimiter = "|";
                                }
                            }
                        }
                        properties[dynamicFieldName] = listValues.ToString();
                    }
                    else if (dynamicField.Type == "General Link" || dynamicField.Type == "General Link with Search")
                    {
                        var linkField = ((Sitecore.Data.Fields.LinkField)dynamicField);
                        properties[dynamicFieldName] = BuildLinkUrl(linkField);
                    }
                    else
                    {
                        properties[dynamicFieldName] = dynamicField.Value;
                    }
                }
            }
            return properties;
        }
        public static string GetMediaItemUrl(this Item item)
        {
            var mediaUrlOptions = new MediaUrlOptions() { UseItemPath = false, AbsolutePath = true };
            return Sitecore.Resources.Media.MediaManager.GetMediaUrl(item, mediaUrlOptions);
        }
        //private static Dictionary<string, string> BuildCalculatedFields(Item item, Tab tab, List<CalculatedField> calculatedFields)
        //{
        //    Dictionary<string, string> properties = new Dictionary<string, string>();
        //    foreach (var field in calculatedFields)
        //    {
        //        if(!string.IsNullOrWhiteSpace(field.ClassName) && !string.IsNullOrWhiteSpace(field.AssemblyName))
        //        {
        //            Type calculatedType = Sitecore.Reflection.ReflectionUtil.GetTypeInfo(field.AssemblyName, field.ClassName );
        //            if(calculatedType == null)
        //            {                        
        //                continue;
        //            }
        //            if(!typeof(ICalculatedField).IsAssignableFrom(calculatedType))
        //            {
        //                Sitecore.Diagnostics.Log.Error("SearchResultMapper: Class : '" + field.ClassName + "' does not implement interface 'ICalculatedField' ", "");
        //                continue;
        //            }
        //            CalculatedFieldArgs args = new CalculatedFieldArgs(item, tab, field.Parameters);
        //            try
        //            { 
        //                var calculatedObj = Sitecore.Reflection.ReflectionUtil.CreateObject(calculatedType);
        //                if (calculatedObj == null)
        //                {
        //                    Sitecore.Diagnostics.Log.Error("SearchResultMapper: Could not create object of : '" + field.ClassName + "' ", "");
        //                    continue;
        //                }
        //                var valueObj = Sitecore.Reflection.ReflectionUtil.CallMethod(calculatedObj, "CalculateFieldValue", new object[] { args });
        //                properties.Add(field.Name, valueObj.ToString());
        //            }
        //            catch(Exception ex)
        //            {
        //                Sitecore.Diagnostics.Log.Error(ex.Message, ex, "");
        //                continue;
        //            }
                   
        //        }
        //    }
        //    return properties;
        //}

        public static String BuildLinkUrl(Sitecore.Data.Fields.LinkField lf)
        {
            switch (lf.LinkType.ToLower())
            {
                case "internal":
                    // Use LinkMananger for internal links, if link is not empty
                    if(lf.TargetItem!=null && lf.TargetItem.Paths.IsMediaItem)
                    {
                        return GetMediaItemUrl(lf.TargetItem);//Sitecore.Resources.Media.MediaManager.GetMediaUrl(lf.TargetItem);
                    }                    
                    return lf.TargetItem != null ? Sitecore.Links.LinkManager.GetItemUrl(lf.TargetItem) : string.Empty;
                case "media":
                    // Use MediaManager for media links, if link is not empty
                    return lf.TargetItem != null ? GetMediaItemUrl(lf.TargetItem) : string.Empty;
                case "external":
                    // Just return external links
                    return lf.Url;
                case "anchor":
                    // Prefix anchor link with # if link if not empty
                    return !string.IsNullOrEmpty(lf.Anchor) ? "#" + lf.Anchor : string.Empty;
                case "mailto":
                    // Just return mailto link
                    return lf.Url;
                case "javascript":
                    // Just return javascript
                    return lf.Url;
                default:
                    // Just please the compiler, this
                    // condition will never be met
                    return lf.Url;
            }
        }
    }
}