using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuFridge.DataAccess.Entity
{
    public class RetentionPolicyHistoryEntity : IEntityBase
    {
        public Guid Id { get; set; }
        public string Date { get; set; }
        public bool Result { get; set; }
        public int PackagesDeleted { get; set; }
        public string DiskSpaceDeleted { get; set; }
        public string FeedName { get; set; }
        public string Log { get; set; }
        public string TimeRunning { get; set; }
    }
}
