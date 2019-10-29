using NimbleSearch.Foundation.Abstractions.Models.Search;
using NimbleSearch.Foundation.Abstractions.Pipelines.ApplyBoost;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace NimbleSearch.Foundation.Core.Pipelines.ApplyBoost
{
    public class ApplyDateBoost: ApplyBoostProcessor
    {
        private static readonly ID DateBoostTemplateId = new ID("{2A5E9B17-1C36-4DFD-961F-79DA343364D4}");
        private const string DayChecklistField = "boostRecency";

        public override void Process(ApplyBoostArgs args)
        {
            // To apply or not to apply
            if (args.BoostItem == null || args.BoostItem.TemplateID != DateBoostTemplateId)
                return;

            var daysField = (Sitecore.Data.Fields.MultilistField)args.BoostItem.Fields[DayChecklistField];
            if (daysField == null || daysField.Count <= 0)
                return;

            int i = 0;
            var daysAsInt = (from s in daysField.Items where int.TryParse(s, out i) select i).ToArray();
            if(daysAsInt.Any())
            { 
                var boost = BoostSortDate<NimbleSearchResultItem>(daysAsInt);
                args.Query = args.Query.Cast<NimbleSearchResultItem>().Where(boost);
            }
        }

        /// <summary>
        /// Each range that matches for a result boosts the relevancy score.
        /// Example: days = 7,14,30,90,365
        /// Recent items bubble to the top as they match more range expressions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="days"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> BoostSortDate<T>(int[] days) where T : NimbleSearchResultItem
        {
            var datePredicate = PredicateBuilder.True<T>();
            foreach (var range in days) {
                datePredicate = datePredicate.Or(x => x.SortDate > DateTime.Now.AddDays(-1 * range));
            }
            
            return datePredicate;
        }
    }
}