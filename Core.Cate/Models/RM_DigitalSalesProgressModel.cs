using System;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesProgressModel : BaseModel
    {
        public int ProgressID { get; set; }

        [CustomRequired]
        [CustomDisplayName("Quy trình")]
        public int ProcessID { get; set; }

        public string ProcessName { get; set; }

        [CustomRequired]
        [CustomDisplayName("Mã tiến trình")]
        public string ProgressCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("Tên tiến trình")]
        public string ProgressName { get; set; }

        [CustomDisplayName("Mô tả")]
        public string Description { get; set; }

        [CustomDisplayName("Thời hạn (ngày)")]
        public int DefaultDurationDays { get; set; } = 3;

        [CustomDisplayName("Thứ tự")]
        public int SortOrder { get; set; }

        [CustomDisplayName("Kích hoạt")]
        public bool IsActive { get; set; } = true;
    }
}
