using System;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesTrackingModel : BaseModel
    {
        public int TrackingID { get; set; }
        public int DigitalSalesID { get; set; }
        public int? ProcessID { get; set; }
        public string ProcessName { get; set; }
        public int? StatusID { get; set; }
        public string SalesStatusName { get; set; }
        public int? ProgressID { get; set; }
        public string TaskName { get; set; }
        public string ProgressName { get; set; }
        public int DefaultDurationDays { get; set; }
        public int? AssignedUserID { get; set; }
        public string AssignedUserName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CompletedDate { get; set; }
        public byte Status { get; set; } // 1: Chưa làm, 2: Đang làm, 3: Hoàn thành, 4: Quá hạn
        public string TaskStatusName { get; set; }
        public int IsOverdue { get; set; } // 1: Quá hạn, 0: Bình thường
        public string ResultNote { get; set; }
        public string AttachmentFile { get; set; }
        public bool IsCustomTask { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
