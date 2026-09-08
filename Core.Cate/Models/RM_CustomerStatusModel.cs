using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_CustomerStatusModel : BaseModel
    {
        public int CustomerStatusID { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string StatusClass { get; set; }
    }
}