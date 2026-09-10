using System;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesTimelineModel : BaseModel
    {
        public int TimelineID { get; set; }
        public int DigitalSalesID { get; set; }
        public int? FromStatusID { get; set; }
        public string FromStatusName { get; set; }
        public string FromStatusCode { get; set; }
        public int ToStatusID { get; set; }
        public string ToStatusName { get; set; }
        public string ToStatusCode { get; set; }
        public byte? FromBusinessType { get; set; }
        public string FromBusinessTypeName { get; set; }
        public byte ToBusinessType { get; set; }
        public string ToBusinessTypeName { get; set; }
        public DateTime ActionDate { get; set; }
        public string ActionBy { get; set; }
        public string ActionByName { get; set; }
        public string Note { get; set; }
        public string AttachmentPath { get; set; }
    }
}
