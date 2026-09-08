using System.Web.Mvc;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;

namespace CenIT.Solution.TOC.WebApp.Areas.Manager.Controllers
{
    public class HomeController : AppController
    {
        private readonly SysModuleCache _moduleCache = new SysModuleCache();

        // GET: Manager/Home
        public ActionResult Index()
        {
            var lstModules = _moduleCache.GetByUserName(User.UserName);

            if (lstModules.Count == 1)
            {
                var defaultAction = lstModules[0].DefaultAction;
                return RedirectPermanent(defaultAction);
            }

            return View(lstModules);
        }
    }
}