using NimbleSearch.Foundation.Abstractions.Templates;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Linq;
using FieldName = NimbleSearch.Foundation.Abstractions.Templates.Tab.FieldName;

namespace NimbleSearch.Foundation.Abstractions.Models.Definitions
{
    public class TabItem : CustomItem
    {
        public TabItem(Item item) : base(item)
        {
        }

        #region Query Builder

        public string IndexName { get { return this[FieldName.IndexName]; } }

        public ID[] StartLocations {
            get {
                var fld = (MultilistField)this.InnerItem?.Fields[FieldName.StartLocations];
                return fld?.TargetIDs;
            }
        }
        public ID[] TemplateIDs
        {
            get
            {
                var fld = (MultilistField)this.InnerItem?.Fields[FieldName.Templates];
                return fld?.TargetIDs;
            }
        }
        public string QueryFilter
        {
            get
            {
                return this[FieldName.QueryFilter];
            }
        }

        public bool ApplySecurity { get { return this[FieldName.ApplySecurity] == "1"; } }

        public bool ApplyLatestVersion { get { return this[FieldName.ApplyLatestVersion] == "1"; } }
        public bool ApplyCurrentLanguage { get { return this[FieldName.ApplyCurrentLanguage] == "1"; } }

        #endregion

        #region Facets and Boosting

        public FacetItem[] Facets
        {
            get
            {
                return this.InnerItem?.Children?.FirstOrDefault(x => x.TemplateID == FacetFolder.TemplateID && x.HasChildren)?.Children?.Select(x => (FacetItem)x).ToArray();
            }
        }
        public Item[] Boosts
        {
            get
            {
                return this.InnerItem?.Children?.FirstOrDefault(x => x.TemplateID == BoostFolder.TemplateID && x.HasChildren)?.Children?.ToArray();
            }
        }
        
        #endregion

        #region Paging and Sorting

        public ViewItem[] ViewOptions
        {
            get
            {
                var fld = (MultilistField)this.InnerItem.Fields[FieldName.ViewOptions];
                return fld?.GetItems().Where(x => x!= null).Select(x => (ViewItem)x).ToArray();
            }
        }
        public Item DefaultViewOption
        {
            get
            {
                var fld = (LookupField)this.InnerItem.Fields[FieldName.DefaultView];
                return fld?.TargetItem;
            }
        }

        public int DefaultPageSize
        {
            get
            {
                if(DefaultViewOption!=null)
                {
                    var viewItem = DefaultViewOption != null ? (ViewItem)DefaultViewOption : null;
                    if(viewItem!=null)
                    {
                        return viewItem.PageSizes != null && viewItem.PageSizes.Length > 0 ? viewItem.PageSizes.First() : 10;
                    }
                }
                return 10;
            }
        }


        public Item[] SortOptions
        {
            get
            {
                var fld = (MultilistField)this.InnerItem.Fields[FieldName.SortOptions];
                return fld?.GetItems();
            }
        }

        public Item DefaultSortOption
        {
            get
            {
                var fld = (LookupField)this.InnerItem.Fields[FieldName.DefaultSort];
                return fld?.TargetItem;
            }
        }

        public string DefaultSortField
        {
            get
            {
                return DefaultSortOption?[NimbleSearch.Foundation.Abstractions.Templates.SortOption.FieldName.IndexProperty];
            }
        }


        #endregion

        #region Response

        public string[] ResultFields {
            get
            {
                if (string.IsNullOrWhiteSpace(this[FieldName.ResultFields]))
                    return null;

                var fld = (MultilistField)this.InnerItem.Fields[FieldName.ResultFields];
                if(fld == null)
                    return null;

                return fld.Items;
            }
        }

        public string NoResultsHTML { get { return this[FieldName.NoResultsMessage]; } }

        #endregion

        #region Analytics

        public bool RegisterSearchTerm { get { return this[FieldName.RegisterSearchTerm] == "1"; } }

        #endregion

        #region Item conversions

        public static implicit operator Item (TabItem tab) {
            return tab?.InnerItem;
        }
        public static implicit operator TabItem(Item item)
        {
            if (item == null)
                return null;
            return new TabItem(item);
        }

        #endregion
    }
}