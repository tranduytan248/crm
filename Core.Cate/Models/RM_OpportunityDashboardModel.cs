using System;
using System.Collections.Generic;
namespace Core.Cate.Models
{
    public class RM_OpportunityDashboardRowModel
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public string EmployeeCode { get; set; }
        public int Total { get; set; }
        public int Open { get; set; }
        public int Closed { get; set; }
        public int Won { get; set; }
        public decimal? WinRate { get; set; }
    }

    public class RM_OpportunityChartRowModel
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public string EmployeeCode { get; set; }
        public int SalesStageID { get; set; }
        public string StageName { get; set; }
        public string StatusClass { get; set; }
        public int Count { get; set; }
    }

    public class ProjectMemberDashboardRow
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string TenBoPhan { get; set; }
        public int ProjectCount { get; set; }
        public string ProjectNames { get; set; }
        public decimal AllocatedRevenue { get; set; }
    }

    public class RM_OpportunityDashboardViewModel
    {
        public DateTime? EmpFromDate { get; set; }
        public DateTime? EmpToDate { get; set; }
        public int TotalOpportunity { get; set; }
        public int TotalOpen { get; set; }
        public int TotalClosed { get; set; }
        public int TotalWon { get; set; }
        public decimal WinRate { get; set; }
        public List<RM_OpportunityDashboardRowModel> Rows { get; set; }
            = new List<RM_OpportunityDashboardRowModel>();
        public List<RM_OpportunityChartRowModel> ChartRows { get; set; }
            = new List<RM_OpportunityChartRowModel>();
        public List<ProjectMemberDashboardRow> ProjectMembers { get; set; }
            = new List<ProjectMemberDashboardRow>();
    }
}