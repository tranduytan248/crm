using System;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesProcessModel : BaseModel
    {
        public int ProcessID { get; set; }

        [CustomRequired]
        [CustomDisplayName("DigitalSalesWorkflow_Status_Label")]
        public int StatusID { get; set; }

        public string StatusName { get; set; }

        public byte BusinessType { get; set; }

        public string BusinessTypeName { get; set; }

        [CustomRequired]
        [CustomDisplayName("DigitalSalesWorkflow_ProcessCode_Label")]
        public string ProcessCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("DigitalSalesWorkflow_ProcessName_Label")]
        public string ProcessName { get; set; }

        [CustomDisplayName("DigitalSalesWorkflow_ProcessDescription_Label")]
        public string Description { get; set; }

        [CustomDisplayName("DigitalSalesWorkflow_ProcessSortOrder_Label")]
        public int SortOrder { get; set; }

        [CustomDisplayName("DigitalSalesWorkflow_ProcessIsActive_Label")]
        public bool IsActive { get; set; } = true;

        public int ProgressCount { get; set; }
    }

    public class RM_DigitalSalesProcessSearchModel : BaseSearchModel
    {
        public byte? BusinessType { get; set; }
        public int? StatusID { get; set; }
    }
}
