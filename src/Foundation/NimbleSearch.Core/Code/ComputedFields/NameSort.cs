using NimbleSearch.Foundation.Abstractions.Models.ComputedFields;
using Sitecore.ContentSearch;
using Sitecore.Data.Items;
using System.Text.RegularExpressions;

namespace NimbleSearch.Foundation.Core.ComputedFields
{
    public class NameSort : CommonComputedFieldBase
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item scIndexable = indexable as SitecoreIndexableItem;
            if (ShouldSkip(scIndexable))
                return null;

            var title = GetFirstValidValue(scIndexable, Configuration.Settings.Instance.BestNameFields?.ToArray()) ?? scIndexable.DisplayName;

            return NormalizeSortValue(title);
        }

        /// <summary>
        /// normalize value for expected sorting behaviour... remove punctuation, make case invariant
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string NormalizeSortValue(string value)
        {

            return Regex.Replace(value.ToLowerInvariant(), @"[^\w\s]", string.Empty);
        }
        
    }
}