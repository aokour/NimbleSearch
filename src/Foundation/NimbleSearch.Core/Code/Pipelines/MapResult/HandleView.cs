using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Models.Response;
using NimbleSearch.Foundation.Abstractions.Pipelines.MapResult;
using Sitecore.Data;
using System.Collections.Generic;
using System.Linq;

namespace NimbleSearch.Foundation.Core.Pipelines.MapResult
{
    public class HandleView : MapResultProcessor
    {
        public override void Process(MapResultArgs args)
        {
            var requested = args.SearchParameters.View;

            var allowed = new List<ViewOption>();

            // Add View options from tab definition
            // Set active view by parameters
            var defined = args.TabItem.ViewOptions;
            if (defined != null)
            {
                foreach (var option in defined)
                {
                    allowed.Add(new ViewOption
                    {
                        Label = option.DisplayName,
                        Value = option.ID.Guid,
                        ClassName = option.CssClass,
                        IsSelected = option.ID.Guid == requested
                    });
                }
            }

            // Edge case: requested (default) is not in allowed list; add anyways
            if (!allowed.Any(x => x.Value == requested))
            {
                var optionItem = args.TabItem.Database.GetItem(new ID(requested));
                if (optionItem != null) {
                    var option = (ViewItem)optionItem;
                    allowed.Add(new ViewOption
                    {
                        Label = option.DisplayName,
                        Value = requested,
                        ClassName = option.CssClass,
                        IsSelected = true
                    });
                }
            }

            args.Response.ViewOptions = allowed;
        }
    }
}