using System;

namespace Core.Cate.Models
{
    /// <summary>
    /// Model dùng để render template email kế hoạch cơ hội kinh doanh.
    /// </summary>
    public class RM_OpportunityPlanMailModel
    {
        #region Thông tin người nhận

        /// <summary>Họ tên người nhận</summary>
        public string FullName { get; set; }

        /// <summary>Email người nhận</summary>
        public string ToEmail { get; set; }

        #endregion

        #region Thông tin kế hoạch

        /// <summary>Tên kế hoạch</summary>
        public string PlanName { get; set; }

        /// <summary>Nội dung kế hoạch</summary>
        public string Content { get; set; }

        /// <summary>Thời gian diễn ra</summary>
        public DateTime? WorkingDate { get; set; }

        /// <summary>Thời gian format sẵn (dd/MM/yyyy HH:mm)</summary>
        public string WorkingDateText => WorkingDate?.ToString("dd/MM/yyyy HH:mm");

        /// <summary>Địa điểm</summary>
        public string AddressMeeting { get; set; }

        /// <summary>Người tạo kế hoạch</summary>
        public string CreatedByFullName { get; set; }

        #endregion

        #region Thông tin email

        /// <summary>
        /// Loại email:
        /// Create / AddUser / Reminder_1Day / Reminder_30Min
        /// </summary>
        public string MailType { get; set; }

        /// <summary>Nội dung chính (render động)</summary>
        public string NoiDung { get; set; }

        /// <summary>Link vào hệ thống (detail plan)</summary>
        public string DetailUrl { get; set; }

        /// <summary>Host URL (logo, link tĩnh...)</summary>
        public string HostUrl { get; set; }

        #endregion
    }
}
