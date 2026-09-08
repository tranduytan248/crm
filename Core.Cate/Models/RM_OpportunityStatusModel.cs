using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Models
{
    public class RM_OpportunityStatusModel : BaseModel
    {
        public int OpportunityStatusID { get; set; }
        [CustomRequired]
        [CustomDisplayName("OpportunityStatus_Code_Label")]
        public string StatusCode { get; set; }
        [CustomRequired]
        [CustomDisplayName("OpportunityStatus_Name_Label")]
        public string StatusName { get; set; }
        [CustomDisplayName("OpportunityStatus_Class_Label")]
        public string StatusClass { get; set; }
    }
}
