using Sitecore.Data.Items;
using static NimbleSearch.Foundation.Abstractions.Templates.Facet;
using FieldName = NimbleSearch.Foundation.Abstractions.Templates.Facet.FieldName;

namespace NimbleSearch.Foundation.Abstractions.Models.Definitions
{
    public class FacetItem : CustomItem
    {
        public FacetItem(Item item) : base(item)
        {
        }

        public string IndexProperty { get { return this[FieldName.IndexProperty]; } }

        public bool CollapsedByDefault { get { return this[FieldName.CollapsedByDefault] == "1"; } }

        public int MinimumValues { get
            {
                int minValues;
                if (!int.TryParse(this[FieldName.MinimumValues], out minValues))
                    minValues = 0;
                return minValues;
            } }

        public int LimitValues { get
            {
                int minValues;
                if (!int.TryParse(this[FieldName.LimitValues], out minValues))
                    minValues = -1;
                return minValues;
            } }


        public bool UseAnd { get { return this[FieldName.AndValues] == "1"; } }

        public FacetSortDirection SortBy {
            get {
                
                var sortBy = this[FieldName.SortBy];
                if (string.IsNullOrWhiteSpace(sortBy))
                    return FacetSortDirection.Occurrences;

                switch (sortBy) {
                    case "Alphabetical Asc":
                        return FacetSortDirection.AlphabeticalAsc;
                    case "Alphabetical Desc":
                        return FacetSortDirection.AlphabeticalDesc;
                    default:
                        return FacetSortDirection.Occurrences;
                }
            }
        }


        // Easy item conversions
        public static implicit operator Item (FacetItem facet) {
            return facet?.InnerItem;
        }
        public static implicit operator FacetItem(Item item)
        {
            if (item == null)
                return null;
            return new FacetItem(item);
        }
    }
}