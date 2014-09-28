using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NuFridge.Website.MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(name: "Durandal App Views", url: "App/views/{viewName}.html", defaults: new { controller = "DurandalView", action = "Get" });
        }

    }
}