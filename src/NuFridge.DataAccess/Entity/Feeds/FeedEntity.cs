using System;

namespace NuFridge.DataAccess.Entity.Feeds
{
    [Serializable]
    public class FeedEntity : IEntityBase, IFeedEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string BrowsePackagesUrl
        {
            get { return BaseUrl; }
        }
        public string PublishPackagesUrl
        {
            get { return BaseUrl + "/api/packages"; }
        }
        public string DownloadPackagesUrl
        {
            get { return BaseUrl + "/api/odata"; }
        }

        public string SignalRUrl
        {
            get { return BaseUrl + "/api/signalr"; }
        }

        public string SymbolServerUrl
        {
            get { return BaseUrl + "/api/symbols"; }
        }

       

        public FeedEntity(string name, string url)
        {
            Name = name;
            BaseUrl = url;
        }

        public FeedEntity()
        {

        }
    }
}