using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DashboardModel
    {
        public DateTime FromDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime ToDate { get; set; } = DateTime.Now;
        public DateTime EmpFromDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime EmpToDate { get; set; } = DateTime.Now;
        public DateTime PlanFromDate { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);
        public DateTime PlanToDate   { get; set; } = DateTime.Now;
        public RM_OverviewDashboardModel OverviewDashboardModel { get; set; } = new RM_OverviewDashboardModel();
        public RM_OpportunityDashboardViewModel OpportunityDashboardModel { get; set; } = new RM_OpportunityDashboardViewModel();
    }

    /// <summary>
    /// Cơ hội hoặc dự án đã từ ba ngày trở lên chưa phát sinh cập nhật.
    /// </summary>
    public class RM_StaleUpdateModel
    {
        public int Type { get; set; }
        public int ObjectID { get; set; }
        public string ObjectCode { get; set; }
        public string ObjectName { get; set; }
        public string CustomerName { get; set; }
        public string AMNames { get; set; }
        public string StatusName { get; set; }
        public string StatusClass { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int DaysWithoutUpdate { get; set; }
        public string TenBoPhan { get; set; }
    }

    public class RM_OverviewDashboardModel
    {
        public DashboardSummaryModel Summary { get; set; } = new DashboardSummaryModel();
        public DashboardSummaryModel FeasibilitySummary { get; set; } = new DashboardSummaryModel();
        public List<OpportunityByStatusModel> OpportunityByStatus { get; set; } = new List<OpportunityByStatusModel>();
        public List<OpportunityByServiceModel> OpportunityByService { get; set; } = new List<OpportunityByServiceModel>();
        public List<ProjectByStatusModel> ProjectByStatus { get; set; } = new List<ProjectByStatusModel>();
        public List<ProjectDashboardModel> Projects { get; set; } = new List<ProjectDashboardModel>();
        public List<OpportunityDashboardModel> Opportunitys { get; set; } = new List<OpportunityDashboardModel>();
        public List<PlanDashboardModel> Plans { get; set; } = new List<PlanDashboardModel>();
        public List<RM_StaleUpdateModel> StaleUpdates { get; set; } = new List<RM_StaleUpdateModel>();
    }

    public class DashboardSearchModel
    {
        public DateTime FromDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime ToDate { get; set; } = DateTime.Now;
        public DateTime EmpFromDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime EmpToDate { get; set; } = DateTime.Now;
        public string EmployeeIds { get; set; }
        public int Type { get; set; } = 0;
        public decimal SuccessRate { get; set; }
        public bool IsGreaterOrEqual { get; set; }
        public string Username { get; set; }

        /// <summary>
        /// Mã nhóm dịch vụ dùng cho chart và popup chi tiết theo nhóm dịch vụ.
        /// </summary>
        public int GroupServiceID { get; set; }
    }

    public class DashboardSummaryModel
    {
        public int TotalProjects { get; set; }
        public int TotalOpportunities { get; set; }
        public decimal TotalprojRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpectedValue { get; set; }
    }

    public class OpportunityByStatusModel
    {
        public string StatusName { get; set; }
        public string StatusClass { get; set; }
        public int Total { get; set; }
    }

    public class OpportunityByServiceModel
    {
        public string NameProduct { get; set; }
        public int Total { get; set; }
    }

    public class ProjectByStatusModel
    {
        public string StatusName { get; set; }
        public string StatusClass { get; set; }
        public int Total { get; set; }
    }

    public class ProjectDashboardModel : BaseModel
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int CustomerID { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public string CustomerName { get; set; }
        public DateTime? StartDate { get; set; }
        public string StatusClass { get; set; }
        public string StatusName { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public string Members { get; set; }
        public string TenBoPhan { get; set; }
    }

    public class OpportunityDashboardModel : BaseModel
    {
        public int BusinessOpportunityID { get; set; }
        public string CodeOpportunity { get; set; }
        public string CustomerName { get; set; }
        public int SalesStageID { get; set; }
        public decimal ClosingProbability { get; set; }
        public decimal ExpectedValue { get; set; }
        public int StatusID { get; set; }
        public string Description { get; set; }
        public string FileAttach { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string ProductServices { get; set; }
        public string StatusClass { get; set; }
        public string StatusName { get; set; }
        public string OpportunityName { get; set; }
        public string Members { get; set; }
        public DateTime? ExchangeDate { get; set; }
        public string FullName { get; set; }
        public string ExchangeContent { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPosition { get; set; }
        public string TenBoPhan { get; set; }
    }

    public class DataForPieChartModel
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Một lát dữ liệu của chart theo nhóm dịch vụ, kèm mã nhóm để mở popup chi tiết.
    /// </summary>
    public class GroupServiceChartModel
    {
        public int GroupServiceID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
