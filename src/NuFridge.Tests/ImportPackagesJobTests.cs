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

        internal readonly static string UserAgent = "IntegrationTest";

        [Test]
        [TestCase("http://vm-dnn7-2012:81/nuget/GlobalDependencies")]
        [TestCase("https://www.nuget.org/api/v2/")]
        public void CanQueryFeed(string feedUrl)
        {
            var testProgetUri = new Uri(feedUrl);
            var repos = new DataServicePackageRepository(testProgetUri);
            var package = repos.GetPackages().FirstOrDefault();
            Assert.That(package, Is.Not.Null);
            Assert.That(package.Id, Is.Not.Empty);
        }




        [Test]
        [TestCase("http://vm-dnn7-2012:81/nuget/GlobalDependencies", "http://vm-dnn7-2012:82/Feeds/Deployment/api/packages")]
        public void CanImportPackage(string feedUrl, string targetfeedurl)
        {
            var testProgetUri = new Uri(feedUrl);
            var repos = new DataServicePackageRepository(testProgetUri);
            var package = repos.GetPackages().FirstOrDefault();
            Assert.That(package, Is.Not.Null);
            Assert.That(package.Id, Is.Not.Empty);
         
            // var targetFeed = FeedManager.FindFeed(feedname);
            var targetFeedUri = new Uri(targetfeedurl);
            var targetRepos = new DataServicePackageRepository(targetFeedUri);

            //  var package = repos.GetPackages().First();

            // var package = sourceRepos.FindPackage(packageInfo.Id, packageInfo.Version);
       //  targetRepos.AddPackage(package);
            // packagesImported++;

            PackageServer packageServer = new PackageServer(targetfeedurl, ImportPackagesJobTests.UserAgent);
            packageServer.SendingRequest += (object sender, WebRequestEventArgs e) =>
                {
                    object[] method = new object[2];
                    method[0] = e.Request.Method;
                    method[1] = e.Request.RequestUri;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("{0} {1}", method);
                };

            var stream = package.GetStream();
            var size = stream.Length;

            packageServer.PushPackage("TODO", package, size, 10000, false);
            Console.WriteLine("package pushed.");


        }

      


    }

}
