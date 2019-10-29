using NimbleSearch.Foundation.Abstractions.Models.ComputedFields;
using Sitecore.ContentSearch;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using System.Linq;

namespace NimbleSearch.Foundation.Core.ComputedFields
{
    public class BestImage : CommonComputedFieldBase
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item scIndexable = indexable as SitecoreIndexableItem;
            if (ShouldSkip(scIndexable))
                return null;

            var media = GeFirstMediaItem(scIndexable, Configuration.Settings.Instance.BestImageFields?.ToArray());
            if (media == null)
                return null;

            var options = GetMediaOptions() ?? MediaUrlOptions.Empty;

            var imageUrl = MediaManager.GetMediaUrl(media, options);
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;
 
            // Address improper urls if present
            imageUrl = imageUrl.Replace("sitecore/shell/", string.Empty);

            imageUrl = HashingUtils.ProtectAssetUrl(imageUrl);
            return imageUrl;
        }

        public virtual MediaUrlOptions GetMediaOptions() {
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

        public virtual Item GeFirstMediaItem(Item item, params string[] fields) {
            if (item == null || fields == null || !fields.Any())
                return null;

            foreach (var fieldName in fields)
            {
                var imgField = ((ImageField)item.Fields[fieldName]);

                if (imgField?.MediaItem != null)
                    return imgField.MediaItem;
            }

            return null;
        }

    }
}