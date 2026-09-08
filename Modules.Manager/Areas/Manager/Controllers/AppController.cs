using System.Linq;
using System.Web.Mvc;
using Core.Sys.Caches.Sys;
using TSFramework.Libs.BaseApps;

namespace Modules.Manager.Areas.Manager.Controllers
{
    public class AppController : BaseController
    {
        private readonly SysModuleCache _moduleCache = new SysModuleCache();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var areaName = filterContext.Controller.ControllerContext.RouteData.DataTokens["area"] as string ?? filterContext.Controller.ControllerContext.RouteData.Values["area"] as string;

            var allModules = _moduleCache.GetAll();
            var module = _moduleCache.GetViaArea(areaName);

            ViewData["App-Modules"] = allModules.Select(m => new {m.ModuleName, m.DefaultAction}).ToDictionary(x => x.ModuleName, x => x.DefaultAction);

            base.OnActionExecuting(filterContext);
        }
    }
}