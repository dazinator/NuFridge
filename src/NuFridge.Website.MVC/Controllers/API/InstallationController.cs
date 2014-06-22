using NuFridge.Common.Helpers;
using NuFridge.Common.Managers.IIS;
using NuFridge.DataAccess.Connection;
using System;
using System.Collections.Generic;
using System.Configuration;
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

                var canConnectToMongoDB = MongoRead.Instance.CanConnect;
                var hasValidFeedWebsite = new WebsiteManager(feedWebsiteName).WebsiteExists();

                if (canConnectToMongoDB && hasValidFeedWebsite)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new HttpError(ex, true));
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);

        }

    }
}
