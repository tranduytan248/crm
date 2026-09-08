using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Models
{
    public class RM_ReviewBatchModel : BaseModel
    {
        public int ReviewBatchID { get; set; }
        [CustomRequired]
        [CustomDisplayName("ReviewBatch_BatchCode_Label")]
        public string BatchCode { get; set; }
        [CustomRequired]
        [CustomDisplayName("ReviewBatch_BatchName_Label")]
        public string BatchName { get; set; }
        public int ReviewLevel { get; set; }
        [CustomRequired]
        [CustomDisplayName("ReviewBatch_FromDate_Label")]
        public DateTime? FromDate { get; set; }
        [CustomRequired]
        [CustomDisplayName("ReviewBatch_ToDate_Label")]
        public DateTime? ToDate { get; set; }
    }
}
