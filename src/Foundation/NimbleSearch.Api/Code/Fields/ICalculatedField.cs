using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NimbleSearch.Foundation.Api.Models.Configurations;

namespace NimbleSearch.Foundation.Api.Fields
{
    public interface ICalculatedField
    {
        string CalculateFieldValue(CalculatedFieldArgs args);
    }

    public class CalculatedFieldArgs
    {
        public CalculatedFieldArgs(Item Item, Tab Tab, string Parameters)
        {
            this.Item = Item;
            this.Tab = Tab;
            this.Parameters = Parameters;
        }
        public Item Item { get; internal set; }
        public Tab Tab { get; internal set; }
        public string Parameters { get; internal set; }
    }
}