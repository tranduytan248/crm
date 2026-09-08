using System;
using System.Data;
using System.Web;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Sys.Models.Sys
{
    public class SysSMSTemplateModel : BaseModel
    {
        public int SMSTemplateID { get; set; }

        [CustomRequired]
        [CustomDisplayName("SMSTemplate_Label_Code")]
        public string TemplateCode { get; set; }

        [CustomDisplayName("SMSTemplate_Label_Name")]
        public string TemplateName { get; set; }

        [CustomRequired]
        [CustomDisplayName("SMSTemplate_Label_Content")]
        public string TemplateContent { get; set; }

        [CustomDisplayName("SMSTemplate_Label_IsActive")]
        public bool IsActive { get; set; } = true;
    }

    public class SysSMSLogsModel
    {
        public int SMSLogID { get; set; }
        public string PhoneNumber { get; set; }
        public string TemplateCode { get; set; }
        public string Content { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}