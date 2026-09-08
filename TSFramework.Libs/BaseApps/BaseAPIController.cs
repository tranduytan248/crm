using System.IO;
using System.Web;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using TSFramework.Libs.Attributes;

namespace TSFramework.Libs.BaseApps
{
    [CustomApiAuthorize]
    [ApiActionFilter]
    public class BaseApiController : ApiController
    {
        //public static string RenderRazorViewToString(Controller controller, string viewName, object model)
        //{
        //    controller.ViewData.Model = model;
        //    controller.ViewBag.Model = model;
        //    using (var sw = new StringWriter())
        //    {
        //        var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
        //        var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData,
        //            controller.TempData, sw);
        //        viewResult.View.Render(viewContext, sw);
        //        viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
        //        return sw.GetStringBuilder().ToString();
        //    }
        //}

        //public static T CreateController<T>(RouteData routeData = null)
        //    where T : Controller, new()
        //{
        //    // create a disconnected controller instance
        //    var controller = new T();

        //    // get context wrapper from HttpContext if available
        //    HttpContextBase wrapper = null;
        //    if (HttpContext.Current != null)
        //        wrapper = new HttpContextWrapper(HttpContext.Current);
        //    else
        //        throw new InvalidOperationException(
        //            "Can't create Controller Context if no active HttpContext instance is available.");

        //    if (routeData == null)
        //        routeData = new RouteData();

        //    // add the controller routing if not existing
        //    if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
        //        routeData.Values.Add("controller", controller.GetType().Name
        //            .ToLower()
        //            .Replace("controller", ""));

        //    controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
        //    return controller;
        //}
    }
}