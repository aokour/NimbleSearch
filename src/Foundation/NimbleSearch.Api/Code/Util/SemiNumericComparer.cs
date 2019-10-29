using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Api.Util
{
    public class SemiNumericComparer : IComparer<string>
    {
        public int Compare(string s1, string s2)
        {
            try
            {
                if (IsNumeric(s1) && IsNumeric(s2))
                {
                    if (ConvertToNumber(s1).Value > ConvertToNumber(s2).Value) return 1;
                    if (ConvertToNumber(s1).Value < ConvertToNumber(s2).Value) return -1;
                    if (ConvertToNumber(s1).Value == ConvertToNumber(s2).Value) return 0;
                }

                if (IsNumeric(s1) && !IsNumeric(s2))
                    return -1;

                if (!IsNumeric(s1) && IsNumeric(s2))
                    return 1;

                return string.Compare(s1, s2, true);
            }
            catch(Exception ex)
            {
                Sitecore.Diagnostics.Log.Warn("Could not sort Facet value : " + s1 + " : " + s2, ex, this);
                return string.Compare(s1, s2, true);
            }
        }

        public static bool IsNumeric(object value)
        {
            var numberValue = ConvertToNumber(value);
            return numberValue.HasValue;
        }

        public static Int32? ConvertToNumber(object value)
        {
            Int32? number = null;
            Int32 i;
            if( Int32.TryParse(value.ToString(), NumberStyles.AllowThousands |
                                                 NumberStyles.AllowCurrencySymbol | 
                                                 NumberStyles.AllowDecimalPoint |
                                                 NumberStyles.AllowLeadingSign|
                                                 NumberStyles.AllowLeadingWhite |
                                                 NumberStyles.AllowParentheses |
                                                 NumberStyles.AllowTrailingSign |
                                                 NumberStyles.AllowTrailingWhite |
                                                 NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out i ))
            {
                number = i;
            }
            else
            {
                string firstPart = string.Empty;
                if (value.ToString().Contains("-"))
                {
                    firstPart = value.ToString().Split('-')[0];
                }
                else if (value.ToString().Contains("+"))
                {
                    firstPart = value.ToString().Split('+')[0];
                }
                if (!string.IsNullOrEmpty(firstPart))
                {
                    if (Int32.TryParse(firstPart, NumberStyles.AllowThousands |
                                                 NumberStyles.AllowCurrencySymbol |
                                                 NumberStyles.AllowDecimalPoint |
                                                 NumberStyles.AllowLeadingSign |
                                                 NumberStyles.AllowLeadingWhite |
                                                 NumberStyles.AllowParentheses |
                                                 NumberStyles.AllowTrailingSign |
                                                 NumberStyles.AllowTrailingWhite |
                                                 NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out i))
                    {
                        number = i;
                    }
                }
            }
            return number;
        }
    }
}