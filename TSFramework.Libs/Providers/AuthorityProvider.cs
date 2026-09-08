using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Helpers;
using TSFramework.Libs.Interfaces;

namespace TSFramework.Libs.Providers
{
    public class AuthorityProvider
    {
        private readonly IAuthority _authenticator;

        public AuthorityProvider(IStoreProcedure storeProceduror)
        {
            _authenticator = LoadAuthoriryLibs();
            _authenticator.ProcedureProvider = storeProceduror;
        }

        public static AuthorityProvider Instance(IStoreProcedure storeProceduror)
        {
            return new AuthorityProvider(storeProceduror);
        }

        private static IAuthority LoadAuthoriryLibs()
        {
            var authType = ConfigurationManager.AppSettings["App_AuthorityType"];
            var libsAuthor =
                LibraryProvider<IAuthority>.LoadLibrary(
                    ConfigurationManager.AppSettings["App_AuthorityLibPath"] ?? "Authority");
            return libsAuthor.FirstOrDefault(lib => lib.GetType().Name == authType);
        }

        public bool IsAllow(string userName, string areaName, string controllerName, string actionName)
        {
            return _authenticator.IsAllow(userName, areaName, controllerName, actionName);
        }

        public bool IsAllow(HttpContextBase context, string userName, string areaName, string controllerName, string actionName)
        {
            var tempRequestContext =
                new RequestContext(context, new RouteData());

            var namespaces = RouteTable.Routes.OfType<Route>()
                .Where(d => d.DataTokens != null && d.DataTokens.ContainsKey("area") &&
                            (areaName == null || d.DataTokens["area"].Equals(areaName)))
                .Select(r => r.DataTokens).ToArray()[0]["Namespaces"];

            var lstNamespaces = new List<string>((string[])namespaces);

            tempRequestContext.RouteData.DataTokens["Area"] = areaName;
            tempRequestContext.RouteData.DataTokens["Namespaces"] = lstNamespaces;

            tempRequestContext.RouteData.Values["Area"] = areaName;
            tempRequestContext.RouteData.Values["Namespaces"] = lstNamespaces;

            var factory = ControllerBuilder.Current.GetControllerFactory();
            var controller = factory.CreateController(tempRequestContext, controllerName) as ControllerBase;

            var controllerContext = new ControllerContext(tempRequestContext, controller);
            var controllerDescriptor = new ReflectedControllerDescriptor(controller?.GetType());
            var actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);

            var actionAllowAnyPermission =
                actionDescriptor.GetCustomAttributes(typeof(AllowAnyPermissionAttribute), false);
            var controllerAllowAnyPermission =
            controllerDescriptor.GetCustomAttributes(
            typeof(AllowAnyPermissionAttribute), false);

            if (!actionAllowAnyPermission.Any() && !controllerAllowAnyPermission.Any()) return true;

            areaName = areaName ?? controllerContext.RouteData.DataTokens["area"] as string;
            var actionTypes = actionDescriptor
                .GetCustomAttributes(typeof(ActionTypeAttribute), false);
            var currentActionType = actionTypes.Length > 0 ? actionTypes[0] as ActionTypeAttribute : null;
            if (currentActionType == null) return true;
            var actionTypeName = EnumHelper.GetDescription(currentActionType.Type);

            return _authenticator.IsAllow(userName, areaName, controllerName, actionTypeName);
        }

        public bool IsValidUser(string userName, string passWord)
        {
            return _authenticator.IsValidUser(userName, passWord);
        }

        public bool SaveLogin(string userName, bool isValid, string senderIp, string senderHeader)
        {
            return _authenticator.SaveLogin(userName, isValid, senderIp, senderHeader);
        }

        public AuthorizeData GetUserInfo(string userName)
        {
            return _authenticator.GetUserInfo(userName);
        }

        public AuthorizeData Login(string userName, string passWord)
        {
            return _authenticator.Login(userName, passWord);
        }

        public AuthorizeData Token(string userName, string passWord)
        {
            return _authenticator.Token(userName, passWord);
        }
    }
}