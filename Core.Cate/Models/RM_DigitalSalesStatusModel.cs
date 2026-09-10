using System;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesStatusModel : BaseModel
    {
        public int StatusID { get; set; }
        public byte BusinessType { get; set; }
        public string BusinessTypeName { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}
