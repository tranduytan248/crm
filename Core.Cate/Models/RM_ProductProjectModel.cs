using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Models
{
    public class RM_ProductProjectModel : BaseModel
    {
        public int ProductProjectID { get; set; }
        [CustomRequired]
        [CustomDisplayName("Project_Title")]
        public int ProjectID { get; set; }
        [CustomRequired]
        [CustomDisplayName("RMPS_Label_NameProduct")]
        public int ProductServiceID { get; set; }
        [CustomDisplayName("ProductProject_ExpectedRevenue_Label")]
        public decimal ExpectedRevenue { get; set; }
        [CustomDisplayName("ProductProject_StartDate_Label")]
        public DateTime? StartDate { get; set; }
        [CustomDisplayName("ProductProject_End_Label")]
        public DateTime? EndDate { get; set; }
        public string ProjectName { get; set; }
        public List<SelectListItem> ListProject { get; set; }
        public string NameProduct { get; set; }
        public string CustomerName { get; set; }
        public string Note { get; set; }
        public List<Cate_ProductServiceModel> ListProductService { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public int TotalMember { get; set; }
        public string Members { get; set; }
    }
}
