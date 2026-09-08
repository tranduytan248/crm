using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_TaskSearchModel : BaseSearchModel
    {
        public int? TaskGroupID { get; set; }
        public int? TaskTypeID { get; set; }
        public string Keyword { get; set; }

    }
}
