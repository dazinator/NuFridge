using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace NuFridge.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var service = new FeedManagerService();

            log4net.LogManager.GetLogger(typeof (Program)).Info("Starting program");

            if (Environment.UserInteractive)
            {
                service.Start();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }
    }
}
