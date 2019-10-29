using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery;
using Sitecore.ContentSearch.Linq;
using Sitecore.Data;
using System;
using System.Linq;

namespace NimbleSearch.Foundation.Core.Pipelines.BuildQuery
{
    public class ApplyPaging : BuildQueryProcessor
    {
        public override void Process(BuildQueryArgs args)
        {
            var viewOptions = args.TabItem.ViewOptions;
            var defaultView = args.TabItem.DefaultViewOption?.ID.Guid 
                ?? viewOptions?.FirstOrDefault()?.ID.Guid
                ?? Guid.Empty;

            // Validate view parameter
            if (   args.SearchParameters.View == Guid.Empty 
                || !( viewOptions?.Any(x => x.ID.Guid == args.SearchParameters.View) ?? false )
               )
            {
                args.SearchParameters.View = defaultView;
            }

            // Validate PageSize
            var validPageSizes = GetPageSizes(args.SearchParameters.View, args.TabItem.Database) ?? new int[] { };
            var defaultPageSize = validPageSizes.Length > 0 ? validPageSizes.First() : 10;
            if (!validPageSizes.Contains(args.SearchParameters.PageSize))
            {
                    args.SearchParameters.PageSize = defaultPageSize;
            }

            // Validate page
            if (args.SearchParameters.Page <= 0)
                args.SearchParameters.Page = 1;

            // Apply paging
            var page = args.SearchParameters.Page;
            var pageSize = args.SearchParameters.PageSize;
            
            if (pageSize > 0)
            {
                args.Query = args.Query.Page(page-1, pageSize); // input is zero base, so -1
            }
        }

        protected virtual int[] GetPageSizes(Guid viewId, Database db)
        {
            var viewItem = (ViewItem)db.GetItem(new ID(viewId));
            return viewItem?.PageSizes;
        }
    }
}