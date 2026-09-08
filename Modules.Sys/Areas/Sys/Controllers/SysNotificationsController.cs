
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Sys.Areas.Sys.Controllers
{
    [AllowAnonymous]
    public class SysNotificationsController : AppController
    {
        private readonly SysNotificationsCache _sys_NoticationCache;
        private readonly string _noticationTitle = AppProcessor.Messagor.GetMessage("Notication_Title");

        public SysNotificationsController()
        {
            _sys_NoticationCache = new SysNotificationsCache();
        }

        ///// <summary>
        ///// Lấy danh sách thông báo của user
        ///// </summary>    
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpGet]
        public ActionResult GetNotifyByUserName()
        {
            var data = _sys_NoticationCache.GetByUser(User.UserName, out var total, null);
            return PartialView("_NotifyByUserName", data);
        }

        ///// <summary>
        ///// Đánh dấu đã đọc thông báo
        ///// </summary>    
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        public ActionResult MarkAsRead(int id)
        {
            var result = _sys_NoticationCache.MarkAsRead(id, User.UserName);
            return Json(new { status = result > 0 ? true : false });
        }
    }
}