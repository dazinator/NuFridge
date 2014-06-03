using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuFridge.DataAccess.Entity
{
    public class ImportPackagesHistoryEntity : IEntityBase
    {
        public Guid Id { get; set; }
        public string Date { get; set; }
        public bool Result { get; set; }
        public int PackagesImported { get; set; }
        public int PackagesSkipped { get; set; }
        public string SourceFeedUrl { get; set; }
        public string FeedName { get; set; }
        public string Log { get; set; }
        public string TimeRunning { get; set; }
    }
}
