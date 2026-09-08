using System;

namespace Core.Cate.Models
{
    public class ProjectExportRow
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public decimal SuccessRate { get; set; }
        public string CustomerName { get; set; }
        public string TaxCode { get; set; }
        public string CustomerTypeName { get; set; }
        public DateTime? StartDate { get; set; }
        public string StatusName { get; set; }
        public string ContractName { get; set; }
        public string Note { get; set; }
        public int TotalSanPham { get; set; }
        public int TotalThanhVien { get; set; }
        public decimal TotalDoanhThu { get; set; }
        public decimal TotalChiPhi { get; set; }
        public decimal LoiNhuan { get; set; }
    }

    public class ProductExportRow
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public int ProductProjectID { get; set; }
        public string NameProduct { get; set; }
        public decimal ExpectedRevenue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }
        public int TotalMember { get; set; }
    }

    public class MemberExportRow
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public int ProductProjectID { get; set; }
        public string NameProduct { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string RoleNames { get; set; }
    }

    public class TaskExportRow
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public int ProductProjectID { get; set; }
        public string NameProduct { get; set; }
        public string TaskName { get; set; }
        public string AssignedEmployeeNames { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string TaskStatus { get; set; }
        public int? CompletionPercentage { get; set; }
        public string Note { get; set; }
    }
}