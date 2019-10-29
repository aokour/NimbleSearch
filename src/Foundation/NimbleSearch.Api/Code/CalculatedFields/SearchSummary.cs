using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using NimbleSearch.Foundation.Api.Fields;

namespace NimbleSearch.Foundation.Api.CalculatedFields
{
    public class SearchSummary : ICalculatedField
    {
        public string CalculateFieldValue(CalculatedFieldArgs args)
        {
            try
            {
                if (args.Item == null)
                    return string.Empty;

                int defaultLength = 250;
                string returnedValue = string.Empty;

                var _params = args.Parameters.Split('|');

                if (_params.Length > 0 && !string.IsNullOrEmpty(_params[0])) // Length
                {
                    int.TryParse(_params[0], out defaultLength);
                }


                if (args.Item.TemplateID == Templates.PublicationDetailPage.ID)
                {
                    if (args.Item.Fields[Templates.PublicationDetailPage.Fields.Abstract] != null)
                    {
                        returnedValue = args.Item.Fields[Templates.PublicationDetailPage.Fields.Abstract].Value;
                    }
                }
                else
                {
                    if (args.Item.Fields[Templates._SearchSummary.Fields.SearchSummary] != null)
                    {
                        returnedValue = args.Item.Fields[Templates._SearchSummary.Fields.SearchSummary].Value;
                    }
                }

                if (returnedValue.Length > defaultLength)
                {
                    returnedValue = returnedValue.Substring(0, defaultLength - 1) + "...";
                }

                return returnedValue;
            }
            catch(Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(ex.Message, ex, this);
                return string.Empty;
            }
        }
    }
}