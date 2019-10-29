using NimbleSearch.Foundation.Abstractions.Models.Response;
using NimbleSearch.Foundation.Abstractions.Pipelines.MapResult;
using Sitecore.Data;
using System.Collections.Generic;
using System.Linq;
using SortTemplate = NimbleSearch.Foundation.Abstractions.Templates.SortOption;

namespace NimbleSearch.Foundation.Core.Pipelines.MapResult
{
    public class HandleSorting : MapResultProcessor
    {
        public override void Process(MapResultArgs args)
        {
            var requested = args.SearchParameters.Sort;

            var allowed = new List<SortOption>();

            // Add View options from tab definition
            // Set active view by parameters
            var defined = args.TabItem.SortOptions;
            if (defined != null)
            {
                foreach (var option in defined)
                {
                    allowed.Add(new SortOption
                    {
                        Label = option.DisplayName,
                        Value = option.ID.Guid,
                        IsDescending = option[SortTemplate.FieldName.IsDescending] == "1",
                        IsSelected = option.ID.Guid == requested
                    });
                }
            }

            // Edge case: requested (default) is not in allowed list; add anyways
            if (!allowed.Any(x => x.Value == requested))
            {
                var option = args.TabItem.Database.GetItem(new ID(requested));
                if(option != null)
                { 
                    allowed.Add(new SortOption
                    {
                        Label = option.DisplayName,
                        Value = requested,
                        IsDescending = option[SortTemplate.FieldName.IsDescending] == "1",
                        IsSelected = true
                    });
                }
            }

            args.Response.SortOptions = allowed;
        }
    }
}