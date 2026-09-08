using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_TaskActivityModel : BaseModel
    {
        public int ActivityID { get; set; }
        public int TaskManagementID { get; set; }
        public int Employee_ID { get; set; }
        public string EmployeeName { get; set; }
        public string ActivityType { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
    public class TaskActivityChange
    {
        public string ActivityType { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Description { get; set; }
    }
}
