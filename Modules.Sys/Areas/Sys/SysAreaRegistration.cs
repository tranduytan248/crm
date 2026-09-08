using System.Web.Mvc;

namespace Modules.Sys.Areas.Sys
{
    public class SysAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Sys";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Sys_default",
                "Sys/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = "Sys", id = UrlParameter.Optional },
                new[]{ "Modules.Sys.Areas.Sys.Controllers" }
            );
        }
    }
}