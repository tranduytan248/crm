using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_Employee_BusinessPlanModel : BaseModel
    {
        public int EmployeeBusinessPlanID { get; set; }
        [CustomDisplayName("BusinessPlan_Title")]
        [CustomRequired]
        public int BusinessPlanID { get; set; }
        [CustomDisplayName("Employee_Title")]
        public int EmployeeID { get; set; }
        [CustomDisplayName("BusinessPlan_RevenueTarget_Label")]
        public int RevenueTarget { get; set; }
        [CustomDisplayName("BusinessPlan_ResponsibilityRevenue_Label")]
        public int ResponsibilityRevenue { get; set; }
        [CustomDisplayName("Task_Note_Label")]
        public string Note { get; set; }
        public string FullName { get; set; }
        public List<SelectListItem> ListEmployee { get; set; }
        public List<SelectListItem> ListBusinessPlan { get; set; }
        public List<MN_BoPhanModel> ListBoPhan { get; set; }
        [CustomDisplayName("BoPhan_Title")]
        [CustomRequired]
        public int BoPhan_ID { get; set; }
        public int BoPhanCha_ID { get; set; }
        public string TenBoPhan { get; set; }
    }

    public class Import_Employee_BusinessPlanViewModel
    {
        public int STT { get; set; }
        public string MaBoPhan { get; set; }
        public string TenBoPhan { get; set; }
        public string MaNV { get; set; }
        public string FullName { get; set; }
        public string RevenueTarget { get; set; }
        public string ResponsibilityRevenue { get; set; }
        public string Note { get; set; }
        public string Message { get; set; } = "";
    }

    public class Import_Employee_BusinessPlanModel
    {
        public string UserName { get; set; }
        [CustomDisplayName("BusinessPlan_Title")]
        [CustomRequired]
        public int BusinessPlanID { get; set; }
        public List<SelectListItem> ListBusinessPlan { get; set; }
        public string Key { get; set; }
        public HttpPostedFileBase AttachDocumentFile { get; set; }
    }

    public class Employee_BusinessPlanSearchModel
    {
        [CustomDisplayName("Year_Label")]
        public int Year { get; set; } = DateTime.Now.Year;
        [CustomDisplayName("BoPhan_Title")]
        public int? BoPhanID { get; set; }
        public List<MN_BoPhanModel> ListBoPhan { get; set; }
    }
}
