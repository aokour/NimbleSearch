using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.Linq;
using FieldName = NimbleSearch.Foundation.Abstractions.Templates.ViewOption.FieldName;

namespace NimbleSearch.Foundation.Abstractions.Models.Definitions
{
    public class ViewItem : CustomItem
    {
        public ViewItem(Item item) : base(item)
        {
        }

        public string CssClass { get { return this[FieldName.ClassName]; } }


        public Item[] PageSizeItems
        {
            get
            {
                var fld = (LookupField)this.InnerItem?.Fields[FieldName.PageSizeOptions];
                var pageListItem = fld?.TargetItem;
                if (pageListItem == null || !pageListItem.HasChildren)
                    return null;

                return pageListItem.GetChildren().ToArray();
            }
        }

        public int[] PageSizes { get {
                var pageItems = PageSizeItems;
                if (pageItems == null || pageItems.Length <= 0)
                    return null;

                var result = new List<int>();
                foreach (var size in pageItems)
                {
                    int value;
                    if (int.TryParse(size.Name, out value)) {
                        result.Add(value);
                    }
                }
                return result.ToArray();
            } }

        //public List<dynamic> PageViews { get; set; }
        //public dynamic DefaultPageView { get; set; }

        //public List<dynamic> Sorts { get; set; }
        //public dynamic DefaultSort { get; set; }


        // Easy item conversions
        public static implicit operator Item (ViewItem view) {
            return view?.InnerItem;
        }
        public static implicit operator ViewItem(Item item)
        {
            if (item == null)
                return null;
            return new ViewItem(item);
        }
    }
}