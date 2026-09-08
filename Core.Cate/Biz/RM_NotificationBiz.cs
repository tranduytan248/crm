using System;
using System.Collections.Generic;
using System.Linq;
using Core.Cate.Models;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    /// <summary>
    /// Xử lý truy vấn và cập nhật dữ liệu thông báo hiển thị trên thanh header.
    /// </summary>
    public class RM_NotificationBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private const string SP_SAVE = "RM_Notification_Save";
        private const string SP_GET_BY_USER = "RM_Notification_GetByUser";
        private const string SP_MARK_AS_READ = "RM_Notification_MarkAsRead";
        private const string SP_MARK_ALL_AS_READ = "RM_Notification_MarkAllAsRead";

        /// <summary>
        /// Tạo một thông báo và gán cho danh sách người nhận.
        /// </summary>
        /// <param name="model">Dữ liệu thông báo cần tạo.</param>
        /// <returns>Mã thông báo vừa tạo, trả về 0 nếu không tạo được.</returns>
        public int Save(RM_NotificationSaveModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Usernames))
            {
                return 0;
            }

            var result = AppProcessor.ProcedureProvider.ExecuteScalarObject<NotificationIdResult>(
                SP_SAVE, DATA_PROVIDER_NAME,
                model.Title,
                model.Content,
                model.NotificationType,
                model.SourceType,
                model.SourceID,
                model.DetailUrl,
                model.IconClass,
                model.IconColor,
                model.CreatedBy,
                model.CreatedByFullName,
                model.Usernames);

            return result?.NotificationID ?? 0;
        }

        /// <summary>
        /// Lấy danh sách thông báo của một người dùng.
        /// </summary>
        /// <param name="username">Username của người nhận.</param>
        /// <param name="unreadCount">Số thông báo chưa đọc.</param>
        /// <param name="top">Số thông báo tối đa cần lấy.</param>
        /// <param name="onlyUnread">Chỉ lấy thông báo chưa đọc hay không.</param>
        /// <returns>Danh sách thông báo sắp xếp mới nhất trước.</returns>
        public List<RM_NotificationModel> GetByUser(string username, out int unreadCount, int top = 20, bool onlyUnread = false)
        {
            unreadCount = 0;
            if (string.IsNullOrWhiteSpace(username))
            {
                return new List<RM_NotificationModel>();
            }

            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_NotificationModel>(
                SP_GET_BY_USER, DATA_PROVIDER_NAME,
                username, top, onlyUnread)
                ?? new List<RM_NotificationModel>();

            if (data.Count > 0)
            {
                unreadCount = data[0].UnreadCount;
            }

            return data;
        }

        /// <summary>
        /// Đánh dấu một thông báo là đã đọc.
        /// </summary>
        /// <param name="notificationId">Mã thông báo.</param>
        /// <param name="username">Username của người nhận.</param>
        /// <returns>Số bản ghi được cập nhật.</returns>
        public int MarkAsRead(int notificationId, string username)
        {
            if (notificationId <= 0 || string.IsNullOrWhiteSpace(username))
            {
                return 0;
            }

            var result = AppProcessor.ProcedureProvider.ExecuteScalarObject<AffectedRowsResult>(
                SP_MARK_AS_READ, DATA_PROVIDER_NAME, notificationId, username);

            return result?.AffectedRows ?? 0;
        }

        /// <summary>
        /// Đánh dấu toàn bộ thông báo của người dùng là đã đọc.
        /// </summary>
        /// <param name="username">Username của người nhận.</param>
        /// <returns>Số bản ghi được cập nhật.</returns>
        public int MarkAllAsRead(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return 0;
            }

            var result = AppProcessor.ProcedureProvider.ExecuteScalarObject<AffectedRowsResult>(
                SP_MARK_ALL_AS_READ, DATA_PROVIDER_NAME, username);

            return result?.AffectedRows ?? 0;
        }

        /// <summary>
        /// Kết quả trả về mã thông báo vừa tạo.
        /// </summary>
        private class NotificationIdResult
        {
            /// <summary>
            /// Mã thông báo vừa tạo.
            /// </summary>
            public int NotificationID { get; set; }
        }

        /// <summary>
        /// Kết quả trả về số bản ghi được cập nhật.
        /// </summary>
        private class AffectedRowsResult
        {
            /// <summary>
            /// Số bản ghi được cập nhật.
            /// </summary>
            public int AffectedRows { get; set; }
        }
    }
}
