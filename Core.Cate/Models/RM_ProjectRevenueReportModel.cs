using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_ProjectRevenueReportSearchModel : BaseModel
    {
        public string ReportType { get; set; }
        public int? Month { get; set; }
        public int? Quarter { get; set; }
        public int? Year { get; set; }
        public List<SelectListItem> ReportTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Months { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Quarters { get; set; } = new List<SelectListItem>();
    }

    public class RM_ProjectRevenueReportModel
    {
        public int ProductProjectID { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public string ServiceName { get; set; }
        public DateTime RevenueFromDate { get; set; }
        public DateTime RevenueToDate { get; set; }
        public string RevenueDateText { get; set; }
        public string DisplayOrderText { get; set; }
        public string DisplayProjectName { get; set; }
        public string DisplayCustomerName { get; set; }
        public string DisplayRevenueDateText { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RM_ProjectRevenueReportResultModel
    {
        public bool IsValid { get; set; } = true;
        public string ValidationMessage { get; set; }
        public string PeriodDisplayText { get; set; }
        public decimal GrandTotalRevenue { get; set; }
        public List<RM_ProjectRevenueReportModel> Items { get; set; } = new List<RM_ProjectRevenueReportModel>();
    }
}
