﻿using NimbleSearch.Foundation.Abstractions.Pipelines.MapResult;

namespace NimbleSearch.Foundation.Core.Pipelines.MapResult
{
    public class InitResponse : MapResultProcessor
    {
        public override void Process(MapResultArgs args)
        {
            args.Response = new Abstractions.Models.Response.SearchResponse();
        }
    }
}