using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Models
{
    public class RM_RevenueReceivedModel : BaseModel
    {
        public int RevenueReceivedID { get; set; }
        [CustomRequired]
        [CustomDisplayName("ProductProject_Title")]
        public int ProductProjectID { get; set; }
        [CustomDisplayName("RevenueReceived_Amount_Label")]
        public decimal Amount { get; set; }
        [CustomDisplayName("RevenueReceived_Note_Label")]
        public string Note { get; set; }
        public string ProjectName { get; set; }
        public string NameProduct { get; set; }
        [CustomDisplayName("RevenueReceived_ReceivedDate_Label")]
        public DateTime? ReceivedDate { get; set; }
        [CustomDisplayName("RevenueReceived_ReceivedTime_Label")]
        public DateTime? ReceivedTime { get; set; }
        public List<SelectListItem> ListProductProject { get; set; }
    }
}
