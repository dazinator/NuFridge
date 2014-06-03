using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Quartz;
using Quartz.Impl;

namespace NuFridge.Common
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
        
    }

}
