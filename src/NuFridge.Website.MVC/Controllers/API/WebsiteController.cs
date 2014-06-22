using NuFridge.Common.Helpers;
using NuFridge.Common.Managers.IIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NuFridge.Website.MVC.Controllers.API
{
    public class WebsiteController : ApiController
    {
        [System.Web.Mvc.HttpPost]
        [ActionName("TestWebsiteExists")]
        public HttpResponseMessage PostTestWebsiteExists(string name)
        {
            WebsiteManager websiteManager = new WebsiteManager(name);
            var result = websiteManager.WebsiteExists();

            return Request.CreateResponse<bool>(HttpStatusCode.OK, result);
        }

        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetFeedWebsiteName()
        {
            try
            {
                string message;
                string websiteName = string.Empty;
                ConfigHelper.GetFeedWebsiteName(out message, out websiteName);

                return Request.CreateResponse<string>(HttpStatusCode.OK, websiteName);
            }
            catch (Exception ex)
            {

            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetFeedWebsitePortNumber()
        {
            try
            {
                return Request.CreateResponse<int>(HttpStatusCode.OK, ConfigHelper.GetFeedWebsitePortNumber());
            }
            catch (Exception ex)
            {

            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetFeedWebsitePhysicalPath()
        {
            try
            {
                return Request.CreateResponse<string>(HttpStatusCode.OK, ConfigHelper.GetFeedWebsitePhysicalPath());
            }
            catch (Exception ex)
            {

            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}
