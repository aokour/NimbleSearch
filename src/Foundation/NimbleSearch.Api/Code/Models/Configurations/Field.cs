using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearchTemplates = NimbleSearch.Foundation.Api.Templates;

namespace NimbleSearch.Foundation.Api.Models.Configurations
{
    public class Field
    {
        public Field(Item fieldItem)
        {
            FieldName = fieldItem[SearchTemplates.Field.Fields.FieldName];
            ReferencedItemFieldName = fieldItem[SearchTemplates.Field.Fields.ReferencedItemFieldName];
            Parameters = fieldItem[SearchTemplates.Field.Fields.Parameters];
        }
        public string FieldName { get; private set; }
        public string ReferencedItemFieldName { get; private set; }
        public string Parameters { get; private set; }
    }

}