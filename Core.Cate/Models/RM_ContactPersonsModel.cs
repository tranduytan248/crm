using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_ContactPersonsModel : BaseModel
    {
        public int ContactPerson_ID { get; set; }
        [CustomDisplayName("ContactPersonal_FullName_Label")]
        public string CodePerson { get; set; }
        [CustomDisplayName("ContactPersonal_FullName_Label")]

        public string FullName { get; set; }
        [CustomDisplayName("ContactPersonal_Gender_Label")]
        public int? Gender { get; set; }
        [CustomDisplayName("ContactPersonal_Position_Label")]
        public string Position { get; set; }
        [CustomDisplayName("ContactPersonal_WorkUnit_Label")]
        public string WorkUnit { get; set; }
        [CustomDisplayName("ContactPersonal_Phone_Label")]
        public string Phone { get; set; }
        [CustomDisplayName("ContactPersonal_Mobile_Label")]
        public string Mobile { get; set; }
        [CustomDisplayName("ContactPersonal_Email_Label")]
        public string Email { get; set; }
        [CustomDisplayName("ContactPersonal_Zalo_Label")]
        public string Zalo { get; set; }
        [CustomDisplayName("ContactPersonal_Address_Label")]
        public string Address { get; set; }
        [CustomDisplayName("ContactPersonal_Birthday_Label")]
        public DateTime? Birthday { get; set; }
        [CustomDisplayName("ContactPersonal_Note_Label")]
        public string Note { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<SelectListItem> ListGender { get; set; }
        public string CustomerName { get; set; }
        public bool StatusBool
        {
            get => Status == 1;
            set => Status = value ? 1 : 0;
        }
    }
    public class RM_ContactPersonsSearchModel : BaseSearchModel
    {
        public string Keyword { get; set; }
        public int? Gender { get; set; }
        public int? Status { get; set; }
        public int? CustomerID { get; set; }
        public List<SelectListItem> ListGender { get; set; }
        public List<SelectListItem> ListStatus { get; set; }
        public List<SelectListItem> ListCustomer { get; set; }
    }
    public class RM_ContactPersonsDuplicateModel
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Reason { get; set; }
    }

    public class RM_ContactPersonsImportResultModel
    {
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public List<RM_ContactPersonsDuplicateModel> DuplicateRows { get; set; }
    }
}
