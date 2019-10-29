using NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery;
using System.Linq;
using Sitecore.Data.Items;
using System;
using SortOption = NimbleSearch.Foundation.Abstractions.Templates.SortOption;

namespace NimbleSearch.Foundation.Core.Pipelines.BuildQuery
{
    public class ApplySorting : BuildQueryProcessor
    {
        public override void Process(BuildQueryArgs args)
        {

            // Validate Sort option
            Item sortItem = null;
            var sortOptions = args.TabItem.SortOptions ?? new Sitecore.Data.Items.Item[] { };

            var defaultSortOption = args.TabItem.DefaultSortOption ?? sortOptions?.FirstOrDefault();
            if (!sortOptions.Any(x => x.ID.Guid == args.SearchParameters.Sort))
            {
                args.SearchParameters.Sort = defaultSortOption?.ID.Guid ?? Guid.Empty;
                sortItem = defaultSortOption;
            }
            else
            {
                sortItem = sortOptions.First(x => x.ID.Guid == args.SearchParameters.Sort);
            }

            if (sortItem == null || sortItem.ID.Guid == Constants.RelevanceOption)
            {
                args.SearchParameters.Sort = Constants.RelevanceOption;
                return;
            }

            var sortField = sortItem[SortOption.FieldName.IndexProperty];
            if (string.IsNullOrWhiteSpace(sortField))
                return;

            var isDesc = sortItem[SortOption.FieldName.IsDescending] == "1";


            args.Query = isDesc ? 
                args.Query.OrderByDescending(x => x[sortField]) :
                args.Query.OrderBy(x => x[sortField]);
        }



        //// or fall back to default
        //sort = args.TabItem?.DefaultSort;

        //if (sort?.Fields != null && sort.Fields.Any())
        //{
        //    // Loop backwards to apply original first as greatest priority
        //    var end = args.TabItem.Sorts.Count - 1;
        //    for (var i = end; i <= 0; i--)
        //    {
        //        var s = sort.Fields[i];
        //        args.Query = s.IsDescending ? 
        //            args.Query.OrderByDescending(x => x[s.fieldName]) : 
        //            args.Query.OrderBy(x => x[s.fieldName]);
        //    }
        //}
    }
}