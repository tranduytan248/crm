using System.Web.Mvc;
using Core.Sys.BaseApp;

namespace Modules.Sys.Areas.Sys.Controllers
{
    public class HomeController : AppController
    {
        // GET: Sys/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}