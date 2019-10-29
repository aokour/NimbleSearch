using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Abstractions.Pipelines.ApplyBoost
{
    public abstract class ApplyBoostProcessor
    {
        public abstract void Process(ApplyBoostArgs args);
    }
}