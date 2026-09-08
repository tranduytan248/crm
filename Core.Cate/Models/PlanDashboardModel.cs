using System;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    /// <summary>
    /// Model hiển thị kế hoạch kinh doanh trên Dashboard
    /// </summary>
    public class PlanDashboardModel : BaseModel
    {
        public int Id { get; set; }
        public int BusinessOpportunityID { get; set; }
        public string Username { get; set; }
        public string PlanName { get; set; }
        public string Content { get; set; }
        public DateTime? WorkingDate { get; set; }
        public string AddressMeeting { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Thông tin join
        public string OpportunityName { get; set; }
        public string CodeOpportunity { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeFullName { get; set; }
        public string EmployeeUserName { get; set; }
        public string RelatedPersonNames { get; set; }
        public string RelatedPersonUsernames { get; set; }

        /// <summary>
        /// Cho phép sửa/xóa: WorkingDate >= ngày hiện tại
        /// </summary>
        public bool CanEdit => WorkingDate.HasValue && WorkingDate.Value.Date >= DateTime.Today;
    }
}
