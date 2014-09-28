using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NuFridge.Common.Manager;
using NUnit.Framework;
using NuGet;

namespace NuFridge.Tests
{



    [TestFixture]
    public class ImportPackagesJobTests
    {
      
        [Test]
        [TestCase("https://www.nuget.org/api/v2/")]
        public void CanQueryFeed(string feedUrl)
        {
            var testProgetUri = new Uri(feedUrl);
            var repos = new DataServicePackageRepository(testProgetUri);
            var package = repos.GetPackages().FirstOrDefault();
            Assert.That(package, Is.Not.Null);
            Assert.That(package.Id, Is.Not.Empty);
        }

    }

}
