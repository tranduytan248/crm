using Core.API.Models;
using Core.Cate.Biz;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Modules.API.Models.KhachHang;
using Modules.API.Models.NewsKhachHang;
using Modules.API.Models.NotificationModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TSFramework.Libs.Processors;

namespace Core.API.Biz.KHACHHANG
{
    public class NotificationBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _API_KhachHang_GetNotifications = "API_KhachHang_GetNotifications";
        private readonly string API_KhachHang_GetNotificationsCategories = "API_KhachHang_GetNotificationsCategories";

        public List<NotificationModel> GetNotifications(int userId)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<NotificationModel>(
                _API_KhachHang_GetNotifications,DATA_PROVIDER_NAME,userId);
        }
        public List<NotificationCategoryModel> GetNotificationCategory()
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<NotificationCategoryModel>(
                API_KhachHang_GetNotificationsCategories, DATA_PROVIDER_NAME);
        }
    }
}
