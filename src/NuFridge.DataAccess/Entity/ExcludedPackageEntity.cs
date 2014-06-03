using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuFridge.DataAccess.Entity
{
    public class ExcludedPackageEntity : IEntityBase
    {
        public Guid Id { get; set; }
        public string PackageId { get; set; }
        public bool PartialMatch { get; set; }

        public ExcludedPackageEntity()
        {
            PackageId = null;
            PartialMatch = false;
        }
    }
}