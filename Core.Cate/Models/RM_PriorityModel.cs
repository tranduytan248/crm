using System;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_PriorityModel : BaseModel
    {
        public int PriorityID { get; set; }
        public string PriorityName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public new string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
