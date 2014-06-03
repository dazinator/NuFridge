using System;
using NuFridge.Common.Manager;
using Quartz;
using NuFridge.DataAccess.Repositories;
using NuFridge.DataAccess.Entity;
using System.Diagnostics;

namespace NuFridge.Common.Jobs
{
    [DisallowConcurrentExecution]
    public class ApplyRetentionPoliciesJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var policies = RetentionPolicyManager.GetRetentionPolicys();

            foreach (var retentionPolicy in policies)
            {
                if (retentionPolicy.Enabled && retentionPolicy.DaysToKeepPackages > 0)
                {
                    string message;
                    bool success;
                    int packagesDeleted;
                    long fileLengthDeleted;

                    var repository = new MongoDbRepository<RetentionPolicyHistoryEntity>();

                    var history = new RetentionPolicyHistoryEntity
                        {
                            Date = DateTime.Now.ToString(),
                            FeedName = retentionPolicy.FeedName
                        };

                    var watch = Stopwatch.StartNew();
                   

                    try
                    {
                        success = RetentionPolicyManager.ApplyRetentionPolicy(retentionPolicy, out message, out fileLengthDeleted, out packagesDeleted);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        message = "Exception: " + ex.Message;
                        packagesDeleted = 0;
                        fileLengthDeleted = 0;
                    }

                    watch.Stop();

                    var timeRunning = Convert.ToInt32(TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds);

                    history.TimeRunning = timeRunning + " second" + (timeRunning == 1 ? "" : "s");
                    history.Result = success;
                    history.Log = message;
                    history.PackagesDeleted = packagesDeleted;

                    history.DiskSpaceDeleted = packagesDeleted == 0 ? "0MB" : GetBytesReadable(fileLengthDeleted);
                    
                    repository.Insert(history);
                }
            }
        }

        public string GetBytesReadable(long i)
        {
            string sign = (i < 0 ? "-" : "");
            double readable = (i < 0 ? -i : i);
            string suffix;
            if (i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (double)(i >> 50);
            }
            else if (i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (double)(i >> 40);
            }
            else if (i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (double)(i >> 30);
            }
            else if (i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (double)(i >> 20);
            }
            else if (i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (double)(i >> 10);
            }
            else if (i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = (double)i;
            }
            else
            {
                return i.ToString(sign + "0 B"); // Byte
            }
            readable /= 1024;

            return sign + readable.ToString("0.### ") + suffix;
        }
    }
}
