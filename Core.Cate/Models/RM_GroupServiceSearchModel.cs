using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_GroupServiceSearchModel : BaseSearchModel
    {
        public string Keyword { get; set; }
        public int? GroupServiceID { get; set; }
        [CustomDisplayName("GroupService_Status_Label")]
        public int? IsActive { get; set; }
    }
}
