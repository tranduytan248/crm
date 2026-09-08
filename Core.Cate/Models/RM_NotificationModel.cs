using System;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    /// <summary>
    /// Thông báo hiển thị trên chuông thông báo của thanh header.
    /// </summary>
    public class RM_NotificationModel : BaseModel
    {
        /// <summary>
        /// Mã thông báo.
        /// </summary>
        public int NotificationID { get; set; }

        /// <summary>
        /// Tiêu đề thông báo.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Nội dung tóm tắt của thông báo.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Mã loại nghiệp vụ sinh ra thông báo.
        /// </summary>
        public string NotificationType { get; set; }

        /// <summary>
        /// Loại đối tượng liên kết: Opportunity, Project hoặc Task.
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// Mã đối tượng liên kết tương ứng với SourceType.
        /// </summary>
        public int? SourceID { get; set; }

        /// <summary>
        /// Đường dẫn tương đối tới màn hình chi tiết của đối tượng liên kết.
        /// </summary>
        public string DetailUrl { get; set; }

        /// <summary>
        /// Class icon hiển thị kèm thông báo.
        /// </summary>
        public string IconClass { get; set; }

        /// <summary>
        /// Class màu của icon hiển thị kèm thông báo.
        /// </summary>
        public string IconColor { get; set; }

        /// <summary>
        /// Username của người tạo thông báo.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Họ tên của người tạo thông báo.
        /// </summary>
        public string CreatedByFullName { get; set; }

        /// <summary>
        /// Thời điểm tạo thông báo.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Người nhận đã đọc thông báo hay chưa.
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Thời điểm người nhận đọc thông báo.
        /// </summary>
        public DateTime? ReadDate { get; set; }

        /// <summary>
        /// Tổng số thông báo chưa đọc của người nhận.
        /// </summary>
        public int UnreadCount { get; set; }
    }

    /// <summary>
    /// Dữ liệu đầu vào để tạo một thông báo mới.
    /// </summary>
    public class RM_NotificationSaveModel
    {
        /// <summary>
        /// Tiêu đề thông báo.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Nội dung tóm tắt của thông báo.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Mã loại nghiệp vụ sinh ra thông báo.
        /// </summary>
        public string NotificationType { get; set; }

        /// <summary>
        /// Loại đối tượng liên kết: Opportunity, Project hoặc Task.
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// Mã đối tượng liên kết tương ứng với SourceType.
        /// </summary>
        public int? SourceID { get; set; }

        /// <summary>
        /// Đường dẫn tương đối tới màn hình chi tiết của đối tượng liên kết.
        /// </summary>
        public string DetailUrl { get; set; }

        /// <summary>
        /// Class icon hiển thị kèm thông báo.
        /// </summary>
        public string IconClass { get; set; }

        /// <summary>
        /// Class màu của icon hiển thị kèm thông báo.
        /// </summary>
        public string IconColor { get; set; }

        /// <summary>
        /// Username của người tạo thông báo.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Họ tên của người tạo thông báo.
        /// </summary>
        public string CreatedByFullName { get; set; }

        /// <summary>
        /// Danh sách username người nhận, phân cách bằng dấu phẩy.
        /// </summary>
        public string Usernames { get; set; }
    }
}
