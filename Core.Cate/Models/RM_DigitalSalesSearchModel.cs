using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesSearchModel : BaseModel
    {
        public string Keyword { get; set; }
        public byte BusinessType { get; set; } // 0: Tất cả, 1: Cơ hội, 2: Dự án
        public int StatusID { get; set; }
        public int CustomerID { get; set; }
        public int ProductServiceID { get; set; }
        public int DepartmentID { get; set; }
        public int EmployeeID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string UserName { get; set; }

        public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> StatusList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ProductServices { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Employees { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ListCustomer { get => Customers; set => Customers = value; }
        public List<SelectListItem> ListStatus { get => StatusList; set => StatusList = value; }
        public List<SelectListItem> ListProductService { get => ProductServices; set => ProductServices = value; }
        public List<SelectListItem> ListDepartment { get => Departments; set => Departments = value; }
        public List<SelectListItem> ListEmployee { get => Employees; set => Employees = value; }
    }
}
