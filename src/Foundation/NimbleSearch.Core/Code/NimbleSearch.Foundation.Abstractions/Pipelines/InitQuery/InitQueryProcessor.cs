using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Abstractions.Pipelines.InitQuery
{
    public abstract class InitQueryProcessor
    {
        public abstract void Process(InitQueryArgs args);
    }
}