using System;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Attributes;

namespace Core.Cate.Models
{
    public class RM_TaskTypeModel : BaseModel
    {
        public int TaskTypeID { get; set; }
        public string TaskTypeName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public new string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
