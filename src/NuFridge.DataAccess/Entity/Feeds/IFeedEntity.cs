using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuFridge.DataAccess.Entity.Feeds
{
    public interface IFeedEntity
    {
        string Name { get; set; }
        string BaseUrl { get; set; }
        string BrowsePackagesUrl { get; }
        string PublishPackagesUrl { get; }
        string DownloadPackagesUrl { get; }
        string SignalRUrl { get; }
        string SymbolServerUrl { get; }
    }
}
