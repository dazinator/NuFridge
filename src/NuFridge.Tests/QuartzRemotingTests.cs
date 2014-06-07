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
    [Category("Integration Test")]
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
                                              @"NuFridge.WindowsService\bin\Debug\NuFridge.WindowsService.exe");

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
