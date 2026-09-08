using System.Web.Mvc;
using System.Web.Routing;

namespace CenIT.Solution.TOC.WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "LoginRoute",
                "Login",
                new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                new[] { "CenIT.Solution.TOC.WebApp.Controllers" }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "CenIT.Solution.TOC.WebApp.Controllers" }
            );
        }
    }
}
