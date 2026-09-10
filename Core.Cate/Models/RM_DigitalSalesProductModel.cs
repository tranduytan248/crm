using System;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesProductModel : BaseModel
    {
        public int SalesProductID { get; set; }
        public int DigitalSalesID { get; set; }

        [CustomRequired]
        [CustomDisplayName("Sản phẩm / Dịch vụ số")]
        public int ProductServiceID { get; set; }
        public string ProductServiceName { get; set; }
        public string ProductServiceCode { get; set; }

        [CustomDisplayName("Doanh thu dự kiến (VNĐ)")]
        public decimal? ExpectedRevenue { get; set; }

        [CustomDisplayName("Doanh thu thực tế (sau ký HĐ) (VNĐ)")]
        public decimal? ActualRevenue { get; set; }

        [CustomDisplayName("Gói cước / Quy mô")]
        public string PackageName { get; set; }

        [CustomDisplayName("Số lượng")]
        public int Quantity { get; set; } = 1;

        [CustomDisplayName("Thời hạn bắt đầu")]
        public DateTime? StartDate { get; set; }

        [CustomDisplayName("Thời hạn kết thúc")]
        public DateTime? EndDate { get; set; }

        [CustomDisplayName("Ghi chú")]
        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
