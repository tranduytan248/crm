using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using System.Web.Mvc;
using System.Web;


namespace Core.Cate.Models
{
    public class RM_TaskAssigneeModel : BaseModel
    {
        public int TaskAssigneeID { get; set; }
        public int TaskManagementID { get; set; }
        public int Employee_ID { get; set; }
        public string CreatedBy { get; set; }
        public string FullName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
