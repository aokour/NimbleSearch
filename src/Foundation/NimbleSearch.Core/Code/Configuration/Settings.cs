using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NimbleSearch.Foundation.Core.Configuration
{
    public class Settings
    {
        public List<string> BestNameFields { get; private set; }
        public List<string> BestDateFields { get; private set; }
        public List<string> BestImageFields { get; private set; }
        public List<string> BestSummaryFields { get; private set; }

        public Settings()
        {
            this.BestNameFields = new List<string>();
            this.BestDateFields = new List<string>();
            this.BestImageFields = new List<string>();
            this.BestSummaryFields = new List<string>();
        }


        public static Settings Instance => (Sitecore.Configuration.Factory.CreateObject("nimble/settings", true) as Configuration.Settings);
    }
}