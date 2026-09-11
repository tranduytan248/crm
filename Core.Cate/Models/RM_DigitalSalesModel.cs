using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesModel : BaseModel
    {
        public int DigitalSalesID { get; set; }

        [CustomDisplayName("Mã số")]
        public string Code { get; set; }

        [CustomRequired]
        [CustomDisplayName("Tên Cơ hội / Dự án")]
        public string Title { get; set; }

        [CustomRequired]
        [CustomDisplayName("Loại hình")]
        public byte BusinessType { get; set; } // 1: Cơ hội, 2: Dự án
        public string BusinessTypeName { get; set; }

        [CustomRequired]
        [CustomDisplayName("Trạng thái")]
        public int StatusID { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }

        [CustomRequired]
        [CustomDisplayName("Khách hàng")]
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }

        [CustomDisplayName("Người liên hệ")]
        public int? ContactPerson_ID { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPhone { get; set; }
        public string ContactPersonEmail { get; set; }

        [CustomDisplayName("Tổng doanh thu dự kiến (VNĐ)")]
        public decimal? TotalExpectedRevenue { get; set; }

        [CustomDisplayName("Tổng doanh thu thực tế (sau ký HĐ) (VNĐ)")]
        public decimal? TotalActualRevenue { get; set; }

        [CustomDisplayName("Xác suất thành công (%)")]
        public decimal? ClosingProbability { get; set; }

        [CustomDisplayName("Ngày dự kiến hoàn thành")]
        public DateTime? ExpectedDate { get; set; }

        [CustomDisplayName("Ngày bắt đầu")]
        public DateTime? StartDate { get; set; }

        [CustomDisplayName("Ngày hoàn thành")]
        public DateTime? EndDate { get; set; }

        [CustomDisplayName("Hợp đồng liên quan")]
        public int? ContractID { get; set; }

        [CustomDisplayName("Số hợp đồng")]
        public string ContractNo { get; set; }

        [CustomDisplayName("Giá trị hợp đồng")]
        public decimal? ContractValue { get; set; }

        [CustomDisplayName("Ngày ký hợp đồng")]
        public DateTime? ContractSignDate { get; set; }

        [CustomDisplayName("Nhân sự phụ trách")]
        public int? AssignedEmployeeID { get; set; }
        public string AssignedEmployeeName { get; set; }

        [CustomDisplayName("Bộ phận phụ trách")]
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        [CustomDisplayName("Ghi chú")]
        public string Note { get; set; }

        [CustomDisplayName("File đính kèm")]
        public string FileAttach { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }

        // Computed & Joined fields
        public string ProductServiceNames { get; set; }
        public int ProductCount { get; set; }
        public int ProgressPercentage { get; set; }
        public int HasOverdueTasks { get; set; }
        public int? TotalCount { get; set; }

        // Danh sách sản phẩm chi tiết
        public List<RM_DigitalSalesProductModel> Products { get; set; } = new List<RM_DigitalSalesProductModel>();

        // Danh sách tiến trình thực thi
        public List<RM_DigitalSalesTrackingModel> TrackingTasks { get; set; } = new List<RM_DigitalSalesTrackingModel>();

        // Danh sách thành viên tham gia
        public List<RM_DigitalSalesMemberModel> Members { get; set; } = new List<RM_DigitalSalesMemberModel>();

        // Lịch sử chuyển đổi trạng thái (Timeline)
        public List<RM_DigitalSalesTimelineModel> Timelines { get; set; } = new List<RM_DigitalSalesTimelineModel>();

        // Dropdown sources for UI
        public List<SelectListItem> ListCustomer { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListProductService { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListStatus { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListContactPerson { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListEmployee { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListDepartment { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListContract { get; set; } = new List<SelectListItem>();
    }

    public class RM_DigitalSalesChangeStatusViewModel
    {
        public int DigitalSalesID { get; set; }
        public string Title { get; set; }
        public string CurrentStatusName { get; set; }
        public byte CurrentBusinessType { get; set; }
        public string CurrentBusinessTypeName { get; set; }

        [CustomRequired]
        [CustomDisplayName("Trạng thái mới")]
        public int NewStatusID { get; set; }

        [CustomDisplayName("Ghi chú / Lý do chuyển")]
        public string Note { get; set; }

        [CustomDisplayName("File biên bản / Hợp đồng đính kèm")]
        public string AttachmentPath { get; set; }

        public List<SelectListItem> AvailableStatuses { get; set; } = new List<SelectListItem>();
    }
}
