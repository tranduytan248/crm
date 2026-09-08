using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_StatusModel : BaseModel
    {
        public int ID { get; set; }
        [CustomRequired]
        [CustomDisplayName("Status_Label_Key")]
        public string StatusKey { get; set; }
        [CustomRequired]
        [CustomDisplayName("Status_Label_Code")]
        public string StatusCode { get; set; }
        [CustomRequired]
        [CustomDisplayName("Status_Label_Name")]
        public string StatusName { get; set; }
        [CustomRequired]
        [CustomDisplayName("Status_Label_SortOrder")]
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        [CustomDisplayName("Status_Label_StatusClass")]
        public string StatusClass { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<SelectListItem> ListStatusKey { get; set; }
        [CustomRequired]
        [CustomDisplayName("Project_SuccessRate_Label")]
        public int Project_SuccessRate { get; set; }
    };

    public class RM_StatusSearchModel
    {
        public string Keyword { get; set; }
        public string SearchKey { get; set; }
        public List<SelectListItem> ListStatusKey { get; set; }

    };
}
