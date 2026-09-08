using System;
using System.Collections.Generic;
using Core.Cate.Biz;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using System.Web.Mvc;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    /// <summary>
    /// Xử lý hiển thị và cập nhật trạng thái thông báo trên thanh header.
    /// Chuông thông báo hiển thị trên mọi màn hình nên không ràng buộc phân quyền chức năng;
    /// dữ liệu luôn được lọc theo người dùng đang đăng nhập.
    /// </summary>
    [AllowAnonymous]
    public class NotificationController : AppController
    {
        private const int HeaderNotificationTop = 20;

        private readonly RM_NotificationBiz _notificationBiz;

        /// <summary>
        /// Khởi tạo xử lý nghiệp vụ thông báo.
        /// </summary>
        public NotificationController()
        {
            _notificationBiz = new RM_NotificationBiz();
        }

        /// <summary>
        /// Hiển thị danh sách thông báo của người dùng hiện tại trên thanh header.
        /// </summary>
        /// <returns>Partial view chuông thông báo.</returns>
        [HttpGet]
        public ActionResult HeaderNotifications()
        {
            int unreadCount;
            List<RM_NotificationModel> data;

            try
            {
                data = _notificationBiz.GetByUser(User.UserName, out unreadCount, HeaderNotificationTop);
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
                unreadCount = 0;
                data = new List<RM_NotificationModel>();
            }

            ViewBag.UnreadCount = unreadCount;

            return PartialView("_HeaderNotifications", data);
        }

        /// <summary>
        /// Đánh dấu một thông báo là đã đọc.
        /// </summary>
        /// <param name="id">Mã thông báo.</param>
        /// <returns>Kết quả cập nhật dạng JSON.</returns>
        [HttpPost]
        public JsonResult MarkAsRead(int id)
        {
            try
            {
                var affectedRows = _notificationBiz.MarkAsRead(id, User.UserName);
                return Json(new { success = true, affectedRows });
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
                return Json(new { success = false, affectedRows = 0 });
            }
        }

        /// <summary>
        /// Đánh dấu toàn bộ thông báo của người dùng hiện tại là đã đọc.
        /// </summary>
        /// <returns>Kết quả cập nhật dạng JSON.</returns>
        [HttpPost]
        public JsonResult MarkAllAsRead()
        {
            try
            {
                var affectedRows = _notificationBiz.MarkAllAsRead(User.UserName);
                return Json(new { success = true, affectedRows });
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
                return Json(new { success = false, affectedRows = 0 });
            }
        }
    }
}
