using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Models
{
    public class RM_ProjectModel : BaseModel
    {
        public int ProjectID { get; set; }
        [CustomRequired]
        [CustomDisplayName("Project_ProjectName_Label")]
        public string ProjectName { get; set; }
        [CustomRequired]
        [CustomDisplayName("Customer_Title")]
        public int CustomerID { get; set; }
        [CustomRequired]
        [CustomDisplayName("Project_Status_Label")]
        public int Status { get; set; }
        [CustomDisplayName("Project_Note_Label")]
        public string Note { get; set; }
        public string CustomerName { get; set; }
        [CustomDisplayName("Project_StartDate_Label")]
        public DateTime? StartDate { get; set; }
        public List<SelectListItem> ListCustomer { get; set; }
        public int ContractID { get; set; }
        public List<SelectListItem> ListContract { get; set; }
        public string StatusClass { get; set; }
        public string StatusName { get; set; }
        public int BusinessOpportunityID { get; set; }
        public string OpportunityName { get; set; }
        public List<RM_StatusModel> ListStatus { get; set; }
        public string UserCreated { get; set; }
        [CustomDisplayName("SuccessRate_Label")]
        public decimal SuccessRate { get; set; }
    }

    public class RM_ProjectSearchModel : BaseModel
    {
        public int ProjectID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Keyword { get; set; }
        public int BoPhanID { get; set; }
        public int CustomerTypeID { get; set; }
        public int StatusID { get; set; }
        public string StatusIDs { get; set; }
        public int? Year { get; set; }
        public decimal? SuccessRateFrom { get; set; }
        public decimal? SuccessRateTo { get; set; }
        public List<RM_CustomerModel> Customers { get; set; }
        public List<RM_StatusModel> Status { get; set; }
        public List<RM_CustomerTypeModel> CustomerTypes { get; set; }
        public List<MN_BoPhanModel> Departments { get; set; }
        public int EmployeeID { get; set; }
        public string UserName { get; set; }
        public List<int> ExcludedStatusIDs { get; set; }
    }
}
