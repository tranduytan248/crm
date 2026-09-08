using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Core.Log.Caches;
using Core.Log.Models;
using Core.Sys.Caches.Sys;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.BaseApps;
using TSFramework.Libs.Processors;

namespace Core.Sys.BaseApp
{
    public class AppController : BaseController
    {
        private readonly SysModuleCache _moduleCache = new SysModuleCache();
        private readonly AccessHistoryCache _accessHistoryCache = new AccessHistoryCache();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var areaName = filterContext.Controller.ControllerContext.RouteData.DataTokens["area"] as string ??
                           filterContext.Controller.ControllerContext.RouteData.Values["area"] as string;
            var actionTypes = filterContext.ActionDescriptor
                .GetCustomAttributes(typeof(ActionTypeAttribute), false);
            var currentActionType = actionTypes.Length > 0 ? actionTypes[0] as ActionTypeAttribute : null;
            if (currentActionType != null)
            {
                var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var actionName = filterContext.ActionDescriptor.ActionName;

                //SaveAccessHistory(areaName, controllerName, actionName);
            }

            //var allModules = _moduleCache.GetByUserName(User.UserName);
            //var currentModule = _moduleCache.GetViaArea(areaName);
            //if (currentModule != null)
            //{
            //    ViewData["CurrentModule"] = currentModule.ModuleName;
            //}

            //ViewData["AppModules"] = allModules.Where(m => m.ModuleId != currentModule?.ModuleId).Select(m => new
            //{
            //    m.ModuleName, 
            //    m.DefaultAction,
            //    m.Icon
            //}).ToDictionary(x => x.ModuleName, x => $"{x.DefaultAction};{x.Icon}");
            
            base.OnActionExecuting(filterContext);
        }


        private bool SaveAccessHistory(string areaName, string controllerName, string actionName)
        {
            // Tạo một đối tượng AccessHistory từ thông tin cung cấp
            var accessHistory = new AccessHistoryModel()
            {
                Area = areaName,
                Controller = controllerName,
                Action = actionName,
                CreatedBy = User.UserName,
            };

            // Lưu lịch sử truy cập vào cơ sở dữ liệu hoặc cache
            return _accessHistoryCache.Save(accessHistory) > 0;
        }
    }
}