using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Models.Response;
using NimbleSearch.Foundation.Abstractions.Pipelines.MapResult;
using Sitecore.Data;
using System;
using System.Collections.Generic;

namespace NimbleSearch.Foundation.Core.Pipelines.MapResult
{
    public class HandlePaging : MapResultProcessor
    {
        public override void Process(MapResultArgs args)
        {
            var pageSizes = new List<PagingOption>();
            var viewItem = args.TabItem.Database.GetItem(new ID(args.SearchParameters.View));
            if (viewItem != null) {
                var view = (ViewItem)viewItem;
                var pageOptions = view.PageSizes;
                if (pageOptions != null) {
                    foreach (var size in pageOptions) {
                        pageSizes.Add(new PagingOption {
                            Value = size,
                            IsSelected = args.SearchParameters.PageSize == size
                        });
                    }
                }
            }
            
            var totalPages = args.TotalSearchResults <= 0? 1 
                : (int)Math.Ceiling((double)args.TotalSearchResults / args.SearchParameters.PageSize);

            args.Response.Paging = new Abstractions.Models.Response.Paging {
                Page = args.SearchParameters.Page,
                TotalPages = totalPages,
                PagingSizes = pageSizes
            };
        }
    }
}