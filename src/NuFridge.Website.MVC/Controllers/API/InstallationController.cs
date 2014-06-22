using MongoDB.Driver;
using NuFridge.Common.Helpers;
using NuFridge.Common.Managers.IIS;
using NuFridge.DataAccess.Connection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NuFridge.Website.MVC.Controllers.API
{
    public class InstallationController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage IsInstallationValid()
        {
            try
            {
                string message;
                string feedWebsiteName;

                if (!ConfigHelper.GetFeedWebsiteName(out message, out feedWebsiteName))
                    throw new Exception(message);

                var mongoRead = new MongoRead(false);
                var canConnectToMongoDB = mongoRead.Connect();

                var hasValidFeedWebsite = new WebsiteManager(feedWebsiteName).WebsiteExists();

                if (canConnectToMongoDB && hasValidFeedWebsite)
                {
                    return Request.CreateResponse<bool>(HttpStatusCode.OK, true);
                }
            }
            catch (Exception ex)
            {

            }

            return Request.CreateResponse<bool>(HttpStatusCode.OK, false);
        }
    }
}