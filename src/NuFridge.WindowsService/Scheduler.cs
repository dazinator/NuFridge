using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Quartz;
using Quartz.Impl;
using NuFridge.Common.Jobs;
using NuFridge.Common.Jobs.CCNet;

namespace NuFridge.WindowsService
{

    public sealed class Scheduler
    {
        private static volatile Scheduler instance;
        private static object syncRoot = new Object();

        public IScheduler QuartzScheduler { get; set; }

        private Scheduler()
        {
            QuartzScheduler = StdSchedulerFactory.GetDefaultScheduler();
        }

        public static Scheduler Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Scheduler();
                    }
                }

                return instance;
            }
        }

        public void StopScheduler()
        {
            if (QuartzScheduler.IsStarted)
            {
                QuartzScheduler.Shutdown();
            }
        }

        public void StartScheduler()
        {
            // and start it off
            QuartzScheduler.Start();


            ITrigger retentionPolicyTrigger = TriggerBuilder.Create()
                                             .WithIdentity("RetentionPolicyJob", "Maintenance")
                                             .WithDailyTimeIntervalSchedule(
                                                 x =>
                                                 x.WithIntervalInHours(24)
                                                  .OnMondayThroughFriday()
                                                  .StartingDailyAt(new TimeOfDay(23, 0, 0)))
                                             .Build();

            AddJob<ApplyRetentionPoliciesJob>("Maintenance", retentionPolicyTrigger);



            ITrigger CCNetTrigger = TriggerBuilder.Create()
                                             .WithIdentity("CCNetJob", "Integration")
                                             .WithSimpleSchedule(s => s.RepeatForever().WithIntervalInSeconds(13).WithMisfireHandlingInstructionIgnoreMisfires()).Build();

            AddJob<CCNetJob>("Integration", CCNetTrigger);

        }

        private void AddJob<T>(string group, ITrigger trigger) where T : IJob
        {
            IJobDetail job = JobBuilder.Create<T>()
    .WithIdentity(typeof(T).Name, group)
    .Build();

            QuartzScheduler.ScheduleJob(job, trigger);
        }

    }

}