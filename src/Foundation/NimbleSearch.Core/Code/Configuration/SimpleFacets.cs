using Sitecore.Data;
using Sitecore.Diagnostics;
using System.Collections.Generic;

namespace NimbleSearch.Foundation.Core.Configuration
{
    public class SimpleFacets
    {
        public List<ID> TemplateIds { get; set; }

        public SimpleFacets ()
        {
            this.TemplateIds = new List<ID>();
        }
        
        public void AddTemplate(string key, System.Xml.XmlNode node)
        {
            AddTemplate(node);
        }
        public void AddTemplate(System.Xml.XmlNode node)
        {
            var value = node.InnerText;
            Log.Info($"NIMBLE SETTINGS: Adding SimpleFacet {value}", this);
            ID templateId;
            if(ID.TryParse(value, out templateId))
                this.TemplateIds.Add(templateId);
            else
                Log.Warn($"NIMBLE SETTINGS: Failed to add {value} as template ID", this);
        }
    }
}