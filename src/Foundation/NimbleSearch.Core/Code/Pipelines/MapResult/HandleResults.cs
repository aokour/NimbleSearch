using NimbleSearch.Foundation.Abstractions.Models.Definitions;
using NimbleSearch.Foundation.Abstractions.Models.Response;
using NimbleSearch.Foundation.Abstractions.Models.Search;
using NimbleSearch.Foundation.Abstractions.Pipelines.MapResult;
using System.Collections.Generic;
using System.Linq;

namespace NimbleSearch.Foundation.Core.Pipelines.MapResult
{
    public class HandleResults : MapResultProcessor
    {
        public override void Process(MapResultArgs args)
        {
            var results = new List<ItemResult>();
            var fields = args.TabItem?.ResultFields?.Distinct().ToList();

            foreach (var doc in args.Hits.Select(x => x.Document)) {


                var result = MapResultModel(doc, fields, args.TabItem);

                results.Add(result);
                    
            }

            args.Response.Items = results;
        }

        public virtual ItemResult MapResultModel(NimbleSearchResultItem doc, List<string> additionalFields, TabItem tabItem) {
            var model = new ItemResult
            {
                Title = doc.Title,
                Url = string.IsNullOrWhiteSpace(doc.Url)? doc.ClickUrl : doc.Url,
                ItemID = doc.ItemId.Guid,
                Type = doc.TemplateName,
                ImageUrl = doc.ImageUrl, 
                Summary = doc.Summary
            };

            var hasFields = additionalFields != null && additionalFields.Any();
            if (hasFields)
            {
                var fieldValues = new Dictionary<string, object>();

                foreach (var field in additionalFields)
                {
                    fieldValues.Add(field, doc.Fields.ContainsKey(field)? doc[field] : string.Empty);
                }
                model.Custom = fieldValues;
            }

            return model;
        }
    }
}