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
    }
}
