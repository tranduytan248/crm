using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_TaskStatusModel : BaseModel
    {
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public string StatusClass { get; set; } 
    }
}