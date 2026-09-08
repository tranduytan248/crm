using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Helpers;
using TSFramework.Libs.Processors;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace TSFramework.Libs.Attributes
{
    public class ApiActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var allowAnonymousController = actionContext.ControllerContext
                .ControllerDescriptor
                .GetCustomAttributes<AllowAnyPermissionAttribute>();
            var allowAnonymousAction = actionContext.ActionDescriptor
                .GetCustomAttributes<AllowAnyPermissionAttribute>();
            if (allowAnonymousController.Count == 0 && allowAnonymousAction.Count == 0)
            {
                var apiPath = actionContext.Request.RequestUri.AbsolutePath.Replace("/api/", string.Empty).Split('/');
                var controllerName = apiPath[0];
                var actionTypes = actionContext.ActionDescriptor
                    .GetCustomAttributes<ActionTypeAttribute>(false);
                var currentActionType = actionTypes.Count > 0 ? actionTypes[0] : null;
                if (currentActionType != null)
                {
                    var actionTypeName = EnumHelper.GetDescription(currentActionType.Type);
                    var userName = HttpContext.Current.User.Identity.Name;

                    // Kiểm tra quyền
                    if (!AppProcessor.Author.IsAllow(userName, "Api", controllerName, actionTypeName))
                        actionContext.Response = actionContext.Request.CreateResponse(new
                        {
                            status = EnumReponseStatus.AccessDenied,
                            data = string.Empty,
                            message = AppProcessor.Messagor.GetMessage("Authorize_AccessDenied")
                        });
                }
            }

            base.OnActionExecuting(actionContext);
        }

        private static bool AllowAnonymous(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ActionDescriptor.ControllerDescriptor
                       .GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}