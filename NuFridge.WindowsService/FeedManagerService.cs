using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace NuFridge.WindowsService
{
    public partial class FeedManagerService : ServiceBase
    {
        public FeedManagerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Scheduler.Instance.StartScheduler();
        }

        protected override void OnStop()
        {
            Scheduler.Instance.StopScheduler();
        }

        public void Start()
        {
            OnStart(null);
        }

        public void Stop()
        {
            OnStop();
        }
    }
}
