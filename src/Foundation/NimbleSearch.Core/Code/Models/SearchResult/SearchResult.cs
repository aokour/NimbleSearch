namespace NimbleSearch.Foundation.Core.Models
{
    using System;
    using Sitecore.Data.Items;
    using Sitecore.Links;
    using Sitecore.Resources.Media;

    public class SearchResult : ISearchResult
    {
        private Uri _url;

        public SearchResult(Item item)
        {
            this.Item = item;
        }

        public Item Item { get; }
        public MediaItem Media { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string Description { get; set; }

        public Uri Url
        {
            get
            {
                return this._url ?? new Uri(GenerateUrl(this.Item), UriKind.Relative);
            }
            set
            {
                this._url = value;
            }
        }

        public string ViewName { get; set; }

        private string GenerateUrl(Item item, UrlOptions options = null)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (options != null)
                return LinkManager.GetItemUrl(item, options);
            return !item.Paths.IsMediaItem ? LinkManager.GetItemUrl(item) : MediaManager.GetMediaUrl(item);
        }
    }
}