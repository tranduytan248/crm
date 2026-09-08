using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Models
{
    public class RM_ProductCostModel : BaseModel
    {
        public int ProductCostID { get; set; }
        [CustomRequired]
        [CustomDisplayName("ProductProject_Title")]
        public int ProductProjectID { get; set; }
        [CustomRequired]
        [CustomDisplayName("CostType_Title")]
        public int CostTypeID { get; set; }
        [CustomDisplayName("ProductCost_Amount_Label")]
        public decimal Amount { get; set; }
        [CustomDisplayName("ProductCost_Note_Label")]
        public string Note { get; set; }
        public string ProjectName { get; set; }
        public string NameProduct { get; set; }
        public string CostTypeName { get; set; }
        [CustomDisplayName("ProductCost_PaymentDate_Label")]
        public DateTime? PaymentDate { get; set; }
        public List<SelectListItem> ListProductProject { get; set; }
        public List<SelectListItem> ListCostType { get; set; }
    }
}
