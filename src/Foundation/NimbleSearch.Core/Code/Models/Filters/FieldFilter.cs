using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Core.Models
{
    public class FieldFilter
    {
        public string FieldName { get; set; }
        public FilterOperator Operator { get; set; }
        public string FiledValue { get; set; }

    }

    public enum FilterOperator
    {
        Equal = 0,
        NotEqual = 1,
        Contains = 2,
        NotContains = 3,
        StartWith = 4,
        GreaterThan = 5,
        LessThan = 6
    }

    public class FieldFilterGroup
    {
        public List<FieldFilter> FieldFilters { get; set; }
        public GroupOperator Operator { get; set; }
    }

    public enum GroupOperator
    {
        And=0,
        Or=1
    }
}