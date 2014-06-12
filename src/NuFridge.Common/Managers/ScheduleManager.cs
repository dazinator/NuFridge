using System;
using Quartz;

namespace NuFridge.Common.Managers
{

    public static class ScheduleManager
    {

        // Grab the Scheduler instance from the Factory 


        public static void Schedule<T>(IJobDetail jobDetail, ITrigger trigger) where T : IJob
        {
            Scheduler.Instance.QuartzScheduler.ScheduleJob(jobDetail, trigger);
        }

        public static TriggerBuilder RunOnceImmediatelyTrigger<T>() where T : IJob
        {
            TriggerBuilder trigger = TriggerBuilder.Create()
              .WithIdentity(typeof(T).Name, "OneTimeJobs")
              .StartAt(new DateTimeOffset(DateTime.UtcNow.AddSeconds(10)))
              .WithSimpleSchedule(x => x
                  .WithRepeatCount(0));
            return trigger;

        }

    }
}