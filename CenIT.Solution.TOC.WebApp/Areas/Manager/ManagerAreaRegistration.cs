using System.Web.Mvc;

namespace CenIT.Solution.TOC.WebApp.Areas.Manager
{
    public class ManagerAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Manager";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Manager_default",
                "Manager/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = "Manager", id = UrlParameter.Optional }
                //new[] { "CenIT.Solution.TOC.WebApp.Areas.Manager.Controllers" }
            );
        }
    }
}