using System;
using System.Collections.Generic;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_BusinessOpportunityWeeklyReportSearchModel : BaseModel
    {
        public string WeekDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int BoPhanID { get; set; }
        public string EmployeeIDs { get; set; }
        public List<MN_BoPhanModel> Departments { get; set; } = new List<MN_BoPhanModel>();
    }

    public class RM_BusinessOpportunityWeeklyReportModel
    {
        public int BoPhanID { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDisplayName { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int BusinessOpportunityID { get; set; }
        public string CodeOpportunity { get; set; }
        public string OpportunityName { get; set; }
        public decimal ClosingProbability { get; set; }
        public decimal ExpectedValue { get; set; }
        public string StageName { get; set; }
        public string CustomerName { get; set; }
        public string LatestExchangeInfo { get; set; }
        public string WeeklyPlan { get; set; }
    }

    public class RM_BusinessOpportunityWeeklyReportResultModel
    {
        public DateTime WeekFrom { get; set; }
        public DateTime WeekTo { get; set; }
        public string DepartmentFilterText { get; set; }
        public string EmployeeFilterText { get; set; }
        public List<RM_BusinessOpportunityWeeklyReportModel> Items { get; set; } = new List<RM_BusinessOpportunityWeeklyReportModel>();
    }
}
