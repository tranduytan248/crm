using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Models
{
    public class RM_CustomerModel : BaseModel
    {
        public int CustomerID { get; set; }
        [CustomRequired]
        [CustomDisplayName("CustomerType_Title")]
        public byte CustomerTypeID { get; set; }
        public string CustomerTypeName { get; set; }

        [CustomRequired]
        [CustomDisplayName("Customer_CustomerName_Label")]
        public string CustomerName { get; set; }

        [CustomRequired]
        [CustomDisplayName("Customer_ShortName_Label")]
        public string ShortName { get; set; }

        [CustomRequired]
        [CustomDisplayName("Customer_TaxCode_Label")]
        public string TaxCode { get; set; }

        [CustomDisplayName("Customer_CompanyType_Label")]
        public string CompanyType { get; set; }

        [CustomDisplayName("Customer_EstablishmentDate_Label")]
        public DateTime? EstablishmentDate { get; set; }
        [CustomDisplayName("Customer_Status_Label")]
        public byte CustomerStatusID { get; set; }
        public string StatusName { get; set; }
        public string StatusClass { get; set; }

        [CustomDisplayName("Customer_Website_Label")]
        public string Website { get; set; }

        [CustomDisplayName("Customer_CharterCapital_Label")]
        public decimal? CharterCapital { get; set; }

        [CustomRequired]
        [CustomDisplayName("Customer_AddressCus_Label")]
        public string AddressCus { get; set; }

        [CustomDisplayName("Customer_Province_Label")]
        public string Province { get; set; }

        [CustomDisplayName("DCustomer_Ward_LabelaChi")]
        public string Ward { get; set; }

        [CustomDisplayName("Customer_Email_Label")]
        public string Email { get; set; }

        [CustomDisplayName("Customer_Phone_Label")]
        public string Phone { get; set; }

        [CustomDisplayName("Customer_Fax_Label")]
        public string Fax { get; set; }

        public List<SelectListItem> ListCustomerType { get; set; }
        public List<SelectListItem> ListStatus { get; set; }
        public List<SelectListItem> ListGender { get; set; }
    }
    public class RM_CustomerDuplicateModel
    {
        public string CustomerName { get; set; }
        public string TaxCode { get; set; }
        public string Reason { get; set; }
    }
    public class RM_CustomerImportResultModel
    {
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public List<RM_CustomerDuplicateModel> DuplicateRows { get; set; }
    }
}