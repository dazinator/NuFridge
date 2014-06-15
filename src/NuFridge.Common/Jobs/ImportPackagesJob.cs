using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuFridge.DataAccess.Entity;
using NuFridge.DataAccess.Model;
using NuFridge.DataAccess.Repositories;
using NuFridge.Common.Manager;
using NuGet;
using Quartz;

namespace NuFridge.Common.Jobs
{

    public class ImportPackagesJob : IJob
    {
        public const string FeedNameKey = "FeedNameKey";
        public const string SourceFeedUrlKey = "SourceFeedUrlKey";
        public const string ApiKey = "FeedApiKey";
        internal readonly static string UserAgent = "NuFridge Import";

        private IRepository<Feed> _repository { get; set; } 

        public ImportPackagesJob(IRepository<Feed> repository)
        {
            _repository = repository;
        }

        public void Execute(IJobExecutionContext context)
        {

            try
            {

                var feedName = (string)context.JobDetail.JobDataMap[FeedNameKey];
                var sourceFeedUrl = (string)context.JobDetail.JobDataMap[SourceFeedUrlKey];
                var apiKey = (string)context.JobDetail.JobDataMap[ApiKey];

                var sourceFeedUri = new Uri(sourceFeedUrl);

                var targetFeed = _repository.GetAll().Single(fd => fd.Name == feedName);

                var historyRepository = new MongoDbRepository<ImportPackagesHistoryEntity>();
                var history = new ImportPackagesHistoryEntity
                {
                    Date = DateTime.Now.ToString(),
                    FeedName = targetFeed.Name,
                    SourceFeedUrl = sourceFeedUrl
                };

                var watch = Stopwatch.StartNew();

                int packagesImported = 0;
                int packagesSkipped = 0;

                try
                {

                    var sourceRepos = new DataServicePackageRepository(sourceFeedUri);
                    var targetFeedUri = new Uri(targetFeed.FeedURL);
                    var targetRepos = new DataServicePackageRepository(targetFeedUri);

                    var packagesInfo = sourceRepos.GetPackages().OrderBy(a => a.Id).ThenBy(a => a.Version).ToList();

                    //    Select(p => new { p.Id, p.Title, p.Version }).ToList();
                    //packagesInfo = packagesInfo.OrderBy(a => a.Id).ThenBy(a => a.Version).

                    var packageServer = new PackageServer(targetFeed.FeedURL, UserAgent);

                    foreach (var packageInfo in packagesInfo)
                    {
                        var packageExists = targetRepos.Exists(packageInfo.Id, packageInfo.Version);
                        if (packageExists)
                        {
                            packagesSkipped++;
                            // Don't bother uploading the package as it allready exists.
                            continue;
                        }

                       // var package = sourceRepos.FindPackage(packageInfo.Id, packageInfo.Version);

                        var stream = packageInfo.GetStream();
                        var size = stream.Length;
                        packageServer.PushPackage(apiKey, packageInfo, size, 50000, false);

                        //   targetRepos.AddPackage(package);
                        packagesImported++;
                    }

                    history.Result = true;

                }
                catch (Exception e)
                {
                    history.Result = false;
                    history.Log = e.Message;
                }
                finally
                {
                    history.PackagesImported = packagesImported;
                    history.PackagesSkipped = packagesSkipped;
                    watch.Stop();
                    var timeRunning = Convert.ToInt32(TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds);
                    history.TimeRunning = timeRunning + " second" + (timeRunning == 1 ? "" : "s");
                    historyRepository.Insert(history);
                }

            }
            catch (Exception e)
            {
                throw new JobExecutionException(e);

            }

        }

    }
}
