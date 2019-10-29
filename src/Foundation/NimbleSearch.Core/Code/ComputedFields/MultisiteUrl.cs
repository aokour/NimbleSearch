using Microsoft.Extensions.DependencyInjection;
using NimbleSearch.Foundation.Abstractions.Services;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.Sites;

namespace NimbleSearch.Foundation.Core.ComputedFields
{
    public class MultisiteUrl : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;
            if (item == null)
            {
                return null;
            }

            var siteService = ServiceLocator.ServiceProvider.GetService<ISiteContext>();
            var siteContext = siteService.GetSiteContext(item);
            if (siteContext == null)
            {
                Log.Warn($"NimbleUrl: Unable to determine Site context for {item.ID}, {item.Name}", this);
            }

            if (!item.Paths.IsMediaItem)
            {
                return this.BuildContentItemLink(item, siteContext);
            }
            return this.BuildMediaItemLink(item, siteContext);

        }


        private string BuildMediaItemLink(Item media, SiteContext siteContext)
        {
            string mediaUrl;

            var options = GetMediaOptions() ?? MediaUrlOptions.Empty;

            if (siteContext != null)
            {
                using (SiteContextSwitcher siteContextSwitcher = new SiteContextSwitcher(siteContext))
                {
                    mediaUrl = MediaManager.GetMediaUrl(media, options);
                }
            }
            else
            {
                mediaUrl = MediaManager.GetMediaUrl(media, options);
            }

            if (string.IsNullOrWhiteSpace(mediaUrl))
                return null;

            // Address improper urls if present
            mediaUrl = mediaUrl.Replace("sitecore/shell/", string.Empty);

            mediaUrl = HashingUtils.ProtectAssetUrl(mediaUrl);
            return mediaUrl;

        }

        private string BuildContentItemLink(Item item, SiteContext siteContext)
        {
            var urlOptions = new UrlOptions()
            {
                LanguageEmbedding = LanguageEmbedding.Never,
                // ShortenUrls = true,
                AlwaysIncludeServerUrl = false,
                SiteResolving = false
            };
            if (siteContext != null)
            {
                urlOptions.AlwaysIncludeServerUrl = true;
                urlOptions.SiteResolving = true;
                urlOptions.Site = siteContext;
            }

            var url = LinkManager.GetItemUrl(item, urlOptions);
            return EnsureProtocol(url, siteContext);
        }

        // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// If url does not start with http:// or http://, add it appropriately
        /// </summary>
        /// <param name="url"></param>
        /// <param name="siteContext"></param>
        /// <returns></returns>
        protected virtual string EnsureProtocol(string url, SiteContext siteContext)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            if (url.StartsWith("http"))
                return url;

            var protocol = siteContext?.Properties["scheme"];
            if (string.IsNullOrWhiteSpace(protocol))
                protocol = "http";

            return string.Format("{0}{1}", protocol, url);
        }

        public virtual MediaUrlOptions GetMediaOptions()
        {
            var options = new MediaUrlOptions
            {
                AlwaysIncludeServerUrl = false,
                AllowStretch = true
            };

            var imageSize = Sitecore.Configuration.Settings.GetIntSetting("nimble.MaxImageSize", 0);
            if (imageSize > 0)
            {

                options.MaxHeight = imageSize;
                options.MaxWidth = imageSize;
            }

            return options;
        }
    }
}