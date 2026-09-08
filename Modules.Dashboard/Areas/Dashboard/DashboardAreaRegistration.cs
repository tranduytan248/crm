using System.Web.Mvc;

namespace Modules.Dashboard.Areas.Dashboard
{
    public class DashboardAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Dashboard";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Dashboard_default",
                url: "Dashboard/{controller}/{action}/{id}",
                defaults: new { controller = "Manager", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Modules.Dashboard.Areas.Dashboard.Controllers" }
            );
        }
    }
}