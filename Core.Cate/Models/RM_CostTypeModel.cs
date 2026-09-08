using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Models
{
    public class RM_CostTypeModel : BaseModel
    {
        public int CostTypeID { get; set; }
        [CustomRequired]
        [CustomDisplayName("CostType_CostTypeCode_Label")]
        public string CostTypeCode { get; set; }
        [CustomRequired]
        [CustomDisplayName("CostType_CostTypeName_Label")]
        public string CostTypeName { get; set; }
    }
}
