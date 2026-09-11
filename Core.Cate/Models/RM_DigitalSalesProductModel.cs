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
        [CustomDisplayName("DigitalSalesProduct_ProductService_Label")]
        public int ProductServiceID { get; set; }
        public string ProductServiceName { get; set; }
        public string ProductServiceCode { get; set; }

        [CustomDisplayName("DigitalSalesProduct_ExpectedRevenue_Label")]
        public decimal? ExpectedRevenue { get; set; }

        [CustomDisplayName("DigitalSalesProduct_ActualRevenue_Label")]
        public decimal? ActualRevenue { get; set; }

        [CustomDisplayName("DigitalSalesProduct_PackageName_Label")]
        public string PackageName { get; set; }

        [CustomDisplayName("DigitalSalesProduct_Quantity_Label")]
        public int Quantity { get; set; } = 1;

        [CustomDisplayName("DigitalSalesProduct_StartDate_Label")]
        public DateTime? StartDate { get; set; }

        [CustomDisplayName("DigitalSalesProduct_EndDate_Label")]
        public DateTime? EndDate { get; set; }

        [CustomDisplayName("DigitalSalesProduct_Note_Label")]
        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
