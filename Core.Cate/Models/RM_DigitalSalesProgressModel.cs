using System;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesProgressModel : BaseModel
    {
        public int ProgressID { get; set; }

        [CustomRequired]
        [CustomDisplayName("DigitalSalesWorkflow_Process_Label")]
        public int ProcessID { get; set; }

        public string ProcessName { get; set; }

        [CustomRequired]
        [CustomDisplayName("DigitalSalesWorkflow_ProgressCode_Label")]
        public string ProgressCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("DigitalSalesWorkflow_ProgressName_Label")]
        public string ProgressName { get; set; }

        [CustomDisplayName("DigitalSalesWorkflow_ProgressDescription_Label")]
        public string Description { get; set; }

        [CustomDisplayName("DigitalSalesWorkflow_DefaultDurationDays_Label")]
        public int DefaultDurationDays { get; set; } = 3;

        [CustomDisplayName("DigitalSalesWorkflow_ProgressSortOrder_Label")]
        public int SortOrder { get; set; }

        [CustomDisplayName("DigitalSalesWorkflow_ProgressIsActive_Label")]
        public bool IsActive { get; set; } = true;
    }
}
