using NimbleSearch.Foundation.Api.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Api.CalculatedFields
{
    public class DisplayNameCalculatedField : ICalculatedField
    {
        public string CalculateFieldValue(CalculatedFieldArgs args)
        {
            string value = string.Empty;
            if (args.Item !=null)
            {
                if (!string.IsNullOrEmpty(args.Parameters))
                {
                    foreach(string param in args.Parameters.Split('|'))
                    {
                        if (string.IsNullOrWhiteSpace(param))
                            continue;
                        if(!string.IsNullOrWhiteSpace(args.Item[param]))
                        {
                            value = args.Item[param];
                            break;
                        }
                    }
                }
                
                if(string.IsNullOrWhiteSpace(value))
                {
                    if (!string.IsNullOrWhiteSpace(args.Item.DisplayName))
                        value = args.Item.DisplayName;
                    else
                        value = args.Item.Name;
                }
            }
            return value;
        }
    }
}