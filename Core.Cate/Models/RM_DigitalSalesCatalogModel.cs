using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesStatusModel : BaseModel
    {
        public int StatusID { get; set; }

        [Range(1, 2, ErrorMessage = "Vui lòng chọn loại hình.")]
        [CustomDisplayName("DigitalSales_BusinessType")]
        public int BusinessType { get; set; }

        public string BusinessTypeName { get; set; }

        [CustomRequired]
        [StringLength(50)]
        [CustomDisplayName("DigitalSales_StatusCode")]
        public string StatusCode { get; set; }

        [CustomRequired]
        [StringLength(250)]
        [CustomDisplayName("DigitalSales_StatusName")]
        public string StatusName { get; set; }

        [StringLength(1000)]
        [CustomDisplayName("DigitalSales_Description")]
        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Thứ tự hiển thị không được âm.")]
        [CustomDisplayName("DigitalSales_SortOrder")]
        public int SortOrder { get; set; }

        [CustomDisplayName("DigitalSales_IsDefault")]
        public bool IsDefault { get; set; }

        [CustomDisplayName("DigitalSales_IsActive")]
        public bool IsActive { get; set; } = true;
    }

    public class RM_DigitalSalesStatusSearchModel : BaseSearchModel
    {
        public int? BusinessType { get; set; }
    }

    public class RM_DigitalSalesProcessModel : BaseModel
    {
        public int ProcessID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn trạng thái.")]
        [CustomDisplayName("DigitalSales_Status")]
        public int StatusID { get; set; }

        public int BusinessType { get; set; }
        public string BusinessTypeName { get; set; }
        public string StatusName { get; set; }

        [CustomRequired]
        [StringLength(50)]
        [CustomDisplayName("DigitalSales_ProcessCode")]
        public string ProcessCode { get; set; }

        [CustomRequired]
        [StringLength(250)]
        [CustomDisplayName("DigitalSales_ProcessName")]
        public string ProcessName { get; set; }

        [StringLength(1000)]
        [CustomDisplayName("DigitalSales_Description")]
        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Thứ tự hiển thị không được âm.")]
        [CustomDisplayName("DigitalSales_SortOrder")]
        public int SortOrder { get; set; }

        [CustomDisplayName("DigitalSales_IsActive")]
        public bool IsActive { get; set; } = true;

        public int ProgressCount { get; set; }
        public List<RM_DigitalSalesStatusModel> Statuses { get; set; }
    }

    public class RM_DigitalSalesProcessSearchModel : BaseSearchModel
    {
        public int? BusinessType { get; set; }
        public int? StatusID { get; set; }
    }

    public class RM_DigitalSalesProgressModel : BaseModel
    {
        public int ProgressID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quy trình không hợp lệ.")]
        public int ProcessID { get; set; }

        [CustomRequired]
        [StringLength(50)]
        [CustomDisplayName("DigitalSales_ProgressCode")]
        public string ProgressCode { get; set; }

        [CustomRequired]
        [StringLength(250)]
        [CustomDisplayName("DigitalSales_ProgressName")]
        public string ProgressName { get; set; }

        [StringLength(1000)]
        [CustomDisplayName("DigitalSales_Description")]
        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Thứ tự hiển thị không được âm.")]
        [CustomDisplayName("DigitalSales_SortOrder")]
        public int SortOrder { get; set; }

        [CustomDisplayName("DigitalSales_IsActive")]
        public bool IsActive { get; set; } = true;
    }

    public class RM_DigitalSalesProgressPageModel
    {
        public RM_DigitalSalesProcessModel Process { get; set; }
        public RM_DigitalSalesProgressModel Editor { get; set; }
        public List<RM_DigitalSalesProgressModel> Items { get; set; }
    }
}
