using System.Web.Mvc;
using System.Web.Routing;

namespace MvcCodeFlowClientManual
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            //routes.MapRoute(
            //     name: "ErrorHandler",
            //     url: "{controller}/{action}/{id}",
            //     defaults: new { controller = "App", action = "Unauthorised", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                "ErrorHandler",
                "Error/{action}/{errMsg}",
                new { controller = "Error", action = "Index", errMsg = UrlParameter.Optional }
            );
        }
    }
}
