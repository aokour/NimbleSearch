using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearchTemplates = NimbleSearch.Foundation.Api.Templates;

namespace NimbleSearch.Foundation.Api.Models.Configurations
{
    public class CalculatedField
    {
        public CalculatedField(Item calculatedField)
        {
            ClassName = calculatedField[SearchTemplates.CalculatedField.Fields.ClassName];
            AssemblyName = calculatedField[SearchTemplates.CalculatedField.Fields.AssemblyName];
            Name = calculatedField[SearchTemplates.CalculatedField.Fields.Name];
            Parameters = calculatedField[SearchTemplates.CalculatedField.Fields.Parameters];
        }
        public string ClassName { get; private set; }
        public string AssemblyName { get; private set; }
        public string Name { get; private set; }
        public string Parameters { get; private set; }
    }
}