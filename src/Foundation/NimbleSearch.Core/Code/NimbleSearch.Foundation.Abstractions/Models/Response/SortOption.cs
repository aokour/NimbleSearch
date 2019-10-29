
using System;

namespace NimbleSearch.Foundation.Abstractions.Models.Response
{
    public class SortOption
    {
        public bool IsSelected{ get; set; }
        public string Label { get; set; }
        public Guid Value { get; set; }
        public bool IsDescending { get; set; }

    }
}