using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;
using System.Linq;

namespace NimbleSearch.Foundation.Abstractions.Models.ComputedFields
{
    public abstract class CommonComputedFieldBase : AbstractComputedIndexField
    {
     
        /// <summary>
        /// Used to determine if item should be processed for field
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool ShouldSkip(Item item) {

            if (item == null)
                return true;

            if (item.Database?.Name == "core")
                return true;

            if( !( item.Paths.IsContentItem || item.Paths.IsMediaItem) )
                return true;

            return false;
        }


        /// <summary>
        /// Used to determine if computed field value is acceptible
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            // tokens are not good, reject them
            return !value.StartsWith("$");
        }

        public virtual string GetFirstValidValue(Item item, params string[] fields)
        {
            if (item == null || fields == null || !fields.Any())
                return null;

            foreach (var fieldName in fields)
            {
                var fieldValue = item[fieldName];
                if (IsValid(fieldValue))
                {
                    return fieldValue;
                }
            }

            return null;
        }
    }
}