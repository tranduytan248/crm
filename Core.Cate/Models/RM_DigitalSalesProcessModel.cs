using System;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesProcessModel : BaseModel
    {
        public int ProcessID { get; set; }

        [CustomRequired]
        [CustomDisplayName("Trạng thái")]
        public int StatusID { get; set; }

        public string StatusName { get; set; }

        public byte BusinessType { get; set; }

        public string BusinessTypeName { get; set; }

        [CustomRequired]
        [CustomDisplayName("Mã quy trình")]
        public string ProcessCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("Tên quy trình")]
        public string ProcessName { get; set; }

        [CustomDisplayName("Mô tả")]
        public string Description { get; set; }

        [CustomDisplayName("Thứ tự")]
        public int SortOrder { get; set; }

        [CustomDisplayName("Kích hoạt")]
        public bool IsActive { get; set; } = true;

        public int ProgressCount { get; set; }
    }

    public class RM_DigitalSalesProcessSearchModel : BaseSearchModel
    {
        public byte? BusinessType { get; set; }
        public int? StatusID { get; set; }
    }
}
