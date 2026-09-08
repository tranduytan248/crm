using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages.Razor.Configuration;
using RazorEngine.Templating;

namespace TSFramework.Libs.Providers
{
    public static class RenderTemplateHtmlProvider
    {
        public static T CreateController<T>(RouteData routeData = null)
            where T : Controller, new()
        {
            // create a disconnected controller instance
            var controller = new T();

            // get context wrapper from HttpContext if available
            HttpContextBase wrapper;
            if (HttpContext.Current != null)
                wrapper = new HttpContextWrapper(HttpContext.Current);
            else
                throw new InvalidOperationException(
                    "Can't create Controller Context if no active HttpContext instance is available.");

            if (routeData == null)
                routeData = new RouteData();

            // add the controller routing if not existing
            if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
                routeData.Values.Add("controller", controller.GetType().Name
                    .ToLower()
                    .Replace("controller", ""));

            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
            return controller;
        }

        public static string RenderViewToHtml(Controller controller, string viewName, object model)
        {
            var context = controller.ControllerContext;
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public static string RenderViewToHtml(ControllerContext context, string viewPath, object model = null,
            bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            viewEngineResult = partial
                ? ViewEngines.Engines.FindPartialView(context, viewPath)
                : ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        public static string RenderPartialToHtml(string fullPathTemplate, object model)
        {
            //fullPathTemplate = HostingEnvironment.MapPath(fullPathTemplate);
            if (!File.Exists(fullPathTemplate)) return string.Empty;
            var contentHtmlTemplate = File.ReadAllText(fullPathTemplate);
            var templateService = new TemplateService();
            // Add the default namespaces that will be automatically imported in all template classes
            AddDefaultNamespacesFromWebConfig(templateService);
            var emailHtmlBody = templateService.Parse(contentHtmlTemplate, model, null, null);
            return emailHtmlBody;
        }

        public static string RenderHtml(string contentPartial, object model)
        {
            if (!string.IsNullOrEmpty(contentPartial)) return string.Empty;
            var templateService = new TemplateService();
            // Add the default namespaces that will be automatically imported in all template classes
            AddDefaultNamespacesFromWebConfig(templateService);
            var emailHtmlBody = templateService.Parse(contentPartial, model, null, null);
            return emailHtmlBody;
        }

        /// <summary>
        ///     Add the namespaces found in the ASP.NET MVC configuration section of the Web.config file to the provided
        ///     TemplateService instance.
        /// </summary>
        private static void AddDefaultNamespacesFromWebConfig(ITemplateService templateService)
        {
            var webConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.config");
            if (!File.Exists(webConfigPath))
                return;

            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = webConfigPath };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var razorConfig = configuration.GetSection("system.web.webPages.razor/pages") as RazorPagesSection;

            if (razorConfig == null)
                return;

            foreach (NamespaceInfo namespaceInfo in razorConfig.Namespaces)
                templateService.AddNamespace(namespaceInfo.Namespace);
        }
    }
}