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
    public class RM_BusinessOpportunityModel : BaseModel
    {
        public int BusinessOpportunityID { get; set; }
        public int CustomerID { get; set; }
        [CustomDisplayName("Customer_CustomerName_Label")]
        public string CustomerName { get; set; }
        [CustomDisplayName("Opportunity_Code_Label")]
        public string CodeOpportunity { get; set; }
        public int SalesStageID { get; set; }
        [CustomDisplayName("ClosingProbability_Label")]
        public decimal ClosingProbability { get; set; }
        [CustomDisplayName("ExpectedValue_Label")]
        public decimal ExpectedValue { get; set; }
        public int StatusID { get; set; }
        [CustomDisplayName("Contact_Content")]
        [CustomRequired]
        public string Description { get; set; }
        [CustomDisplayName("FileUpload")]
        public string FileAttach { get; set; }
        [CustomDisplayName("ExpectedDate_Label")]
        public DateTime? ExpectedDate { get; set; }
        [CustomDisplayName("RMPS_Label_NameProduct")]
        public string ProductServiceIDs { get; set; }
        public List<Cate_ProductServiceModel> SPDichVus { get; set; }
        public List<string> lst_SP { get; set; }
        public string EmployeeIDs { get; set; }
        public string EmployeeNames { get; set; }
        public string ProductServiceNames { get; set; }
        public string StatusClass { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public List<RM_SalesTeamMembersModel> Members { get; set; }
        [CustomRequired]
        [CustomDisplayName("Opportunity_Name_Label")]
        public string OpportunityName { get; set; }
        [CustomRequired]
        [CustomDisplayName("ExchangeDate_Label")]
        public DateTime? ExchangeDate { get; set; }
        public string FullName { get; set; }
        public string ExchangeContent { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPosition { get; set; }
        public string UserCreated { get; set; }

        [CustomDisplayName("ContactPerson_Title")]
        [CustomRequired]
        public int ContactPerson_ID { get; set; }
        public List<SelectListItem> ListContactPerson { get; set; }
        public List<HttpPostedFileBase> DinhKemFile { get; set; }
        public List<RM_BusinessOpportunityFilePathModel> ExistingFiles { get; set; }
        public List<int> DeletedFileIds { get; set; }
    }

    public class RM_BusinessOpportunitySearchModel : BaseModel
    {
        public string Keyword { get; set; }
        public int BusinessOpportunityID { get; set; }
        public int CustomerID { get; set; }
        public int BoPhanID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNameTitle { get; set; }

        public int StatusID { get; set; }
        public List<RM_CustomerModel> Customers { get; set; }
        public List<RM_StatusModel> StatusList { get; set; }
        public List<MN_BoPhanModel> Departments { get; set; }
        public int EmployeeID { get; set; }
        public List<MN_EmployeeModel> Employees { get; set; }
        public int ProductServiceID { get; set; }
        public List<Cate_ProductServiceModel> ProductServices { get; set; }
        public string UserName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public decimal? SuccessRateFrom { get; set; }
        public decimal? SuccessRateTo { get; set; }
    }

    public class ConvertProjectViewModel
    {
        public string ProjectID { get; set; }
        [CustomRequired]
        [CustomDisplayName("Project_ProjectName_Label")]
        public string ProjectName { get; set; }
        public int CustomerID { get; set; }
        public int ContractID { get; set; }
        [CustomDisplayName("Project_StartDate_Label")]
        public DateTime? StartDate { get; set; }
        public int Status { get; set; }
        [CustomDisplayName("Project_Note_Label")]
        public string Note { get; set; }
        [CustomDisplayName("SuccessRate_Label")]
        public decimal SuccessRate { get; set; }
        public List<SelectListItem> ListContract { get; set; }
        public int BusinessOpportunityID { get; set; }

        public List<int> SelectedProductServiceIDs { get; set; } = new List<int>();
        public List<Cate_ProductServiceModel> ProductServices { get; set; } = new List<Cate_ProductServiceModel>();
        public List<RM_ProductProjectModel> ProductProjects { get; set; } = new List<RM_ProductProjectModel>();
    }
}
