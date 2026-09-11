using System;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesStatusModel : BaseModel
    {
        public int StatusID { get; set; }
        [CustomRequired]
        [CustomDisplayName("DigitalSalesWorkflow_BusinessType_Label")]
        public byte BusinessType { get; set; }
        public string BusinessTypeName { get; set; }
        [CustomRequired]
        [CustomDisplayName("DigitalSalesWorkflow_StatusCode_Label")]
        public string StatusCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("DigitalSalesWorkflow_StatusName_Label")]
        public string StatusName { get; set; }

        [CustomDisplayName("DigitalSalesWorkflow_StatusDescription_Label")]
        public string Description { get; set; }

        [CustomDisplayName("DigitalSalesWorkflow_StatusSortOrder_Label")]
        public int SortOrder { get; set; }

        [CustomDisplayName("DigitalSalesWorkflow_IsDefault_Label")]
        public bool IsDefault { get; set; }

        [CustomDisplayName("DigitalSalesWorkflow_StatusIsActive_Label")]
        public bool IsActive { get; set; }
    }
}
