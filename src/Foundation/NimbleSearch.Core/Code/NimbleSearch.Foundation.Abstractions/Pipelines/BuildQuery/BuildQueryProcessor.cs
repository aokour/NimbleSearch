using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery
{
    public abstract class BuildQueryProcessor
    {
        public abstract void Process(BuildQueryArgs args);
    }
}