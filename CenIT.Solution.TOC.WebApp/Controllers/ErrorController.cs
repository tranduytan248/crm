using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.BaseApps;

namespace CenIT.Solution.TOC.WebApp.Controllers
{
    [AllowAnonymous]
    public class ErrorController : BaseController
    {
        [AllowAnyPermission]
        public ActionResult AccessDenied()
        {
            GenDefaultPage();
            Response.StatusCode = 403;
            return View();
        }

        [AllowAnyPermission]
        public ActionResult NotFound()
        {
            GenDefaultPage();
            Response.StatusCode = 404;
            return View();
        }

        [AllowAnyPermission]
        public ActionResult Error()
        {
            GenDefaultPage();
            Response.StatusCode = 500;
            return View();
        }

        private void GenDefaultPage()
        {
            if (Request.UrlReferrer == null) return;

            var fullUrl = Request.UrlReferrer.ToString();
            var questionMarkIndex = fullUrl.IndexOf('?');
            string queryString = null;
            string url = fullUrl;

            if (questionMarkIndex != -1)
            {
                url = fullUrl.Substring(0, questionMarkIndex);
                queryString = fullUrl.Substring(questionMarkIndex + 1);
            }

            var request = new HttpRequest(null, url, queryString);
            var response = new HttpResponse(new StringWriter());
            var httpContext = new HttpContext(request, response);
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            if (routeData == null) return;

            var areaName = routeData.DataTokens["area"];
            ViewData["DefaultPage"] = $"{areaName}/Home";
        }
    }
}