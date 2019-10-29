using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Abstractions.Pipelines.ApplyFacet
{
    public abstract class ApplyFacetProcessor
    {
        public abstract void Process(ApplyFacetArgs args);
    }
}