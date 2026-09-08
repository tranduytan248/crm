using System.Web.Mvc;

namespace CenIT.Solution.TOC.WebApp.Controllers
{
    public class EFormController : Controller
    {
        [AllowAnonymous]
        public ActionResult View(string key, string cc, string accountuser, string checksum)
        {
            return Redirect($"/Cate/EForm/View?key={key}&cc={cc}&accountuser={accountuser}&checksum={checksum}");
        }
    }
}