using System;
using System.Web;
using NuFridge.Common.Manager;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;
using NuFridge.Common;

namespace NuFridge.Website
{
    public class Global : HttpApplication
    {
      

        void Application_Start(object sender, EventArgs e)
        {
      
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }
    }
}
