
using System;

namespace NimbleSearch.Foundation.Abstractions.Models.Response
{
    public class ViewOption
    {
        public bool IsSelected{ get; set; }
        public string Label { get; set; }
        public string ClassName { get; set; }
        public Guid Value { get; set; }
    }
}