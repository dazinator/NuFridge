using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuFridge.Common.Jobs;
using Moq;
using NUnit.Framework;
using Quartz;
using Quartz.Impl;

namespace NuFridge.Tests
{
    [TestFixture]
    public class QuartzRemotingTests
    {

        private Process _Process = null;
        private ISchedulerFactory _SchedulerFactory = null;



        [TestFixtureSetUp]
        public void SetUp()
        {
            var directory = Directory.GetCurrentDirectory();
            var dirInfo = System.IO.Directory.GetParent(directory);
            dirInfo = dirInfo.Parent;
            dirInfo = dirInfo.Parent;

            var path = System.IO.Path.Combine(dirInfo.FullName,
                                              @"FeedManagerService\bin\Debug\FeedManagerWindowsService.exe");

            var startInfo = new ProcessStartInfo(path);
            _Process = Process.Start(startInfo);


            NameValueCollection properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = "RemoteClient";

            // set thread pool info
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "5";
            properties["quartz.threadPool.threadPriority"] = "Normal";

            // set remoting expoter
            properties["quartz.scheduler.proxy"] = "true";
            properties["quartz.scheduler.proxy.address"] = "tcp://localhost:5656/QuartzScheduler";
            // First we must get a reference to a scheduler
            _SchedulerFactory = new StdSchedulerFactory(properties);

        }

        [Test]
        public void CanConnect()
        {
            IScheduler sched = _SchedulerFactory.GetScheduler();
            Assert.That(sched, Is.Not.Null);
        }

        [Test]
        public void CanCreateJob()
        {

            var mock = new Mock<IJob>();

            int executeCount = 0;

            mock.Setup(framework => framework.Execute(It.IsAny<IJobExecutionContext>()));

            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            IJobDetail job = JobBuilder.Create<ImportPackagesJob>()
                .WithIdentity("importpackages", "group1") // name "myJob", group "group1"
                .UsingJobData("nugetfeedurl", "http://somepackage/nuget")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("importpackagestrigger", "group1")
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithRepeatCount(0))
              .Build();

            sched.ScheduleJob(job, trigger);

            // give job time to run.
            Thread.Sleep(5000);

            mock.Verify(x => x.Execute(It.IsAny<IJobExecutionContext>()), Times.AtMost(1));





        }


        [TestFixtureTearDown]
        public void TearDown()
        {
            if (_Process != null)
            {
                _Process.Kill();
                _Process.WaitForExit(9000);
                _Process.Dispose();
                _Process = null;
            }
        }
    }
}
