using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuFridge.Website.MVC.Controllers
{
    public class DurandalViewController : Controller
    {
        //
        // GET: /App/views/{viewName}.html
        [HttpGet]
        public ActionResult Get(string viewName)
        {
            return View("~/App/views/" + viewName + ".cshtml");
        }
    }
}