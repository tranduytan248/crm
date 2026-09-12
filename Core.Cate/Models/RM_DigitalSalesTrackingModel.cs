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
        [CustomRequired]
        [CustomDisplayName("DigitalSalesTracking_TaskName_Label")]
        public string TaskName { get; set; }
        public string ProgressName { get; set; }
        public int DefaultDurationDays { get; set; }
        [CustomDisplayName("DigitalSalesTracking_AssignedUser_Label")]
        public int? AssignedUserID { get; set; }
        public string AssignedUserName { get; set; }
        [CustomDisplayName("DigitalSalesTracking_StartDate_Label")]
        public DateTime StartDate { get; set; }

        [CustomDisplayName("DigitalSalesTracking_Deadline_Label")]
        public DateTime? Deadline { get; set; }
        public DateTime? CompletedDate { get; set; }
        [CustomDisplayName("DigitalSalesTracking_Status_Label")]
        public byte Status { get; set; } // 1: Chưa làm, 2: Đang làm, 3: Hoàn thành, 4: Quá hạn
        public string TaskStatusName { get; set; }
        public int IsOverdue { get; set; } // 1: Quá hạn, 0: Bình thường
        [CustomDisplayName("DigitalSalesTracking_ResultNote_Label")]
        public string ResultNote { get; set; }

        [CustomDisplayName("DigitalSalesTracking_AttachmentFile_Label")]
        public string AttachmentFile { get; set; }
        public bool IsCustomTask { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
