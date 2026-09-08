using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_CustomerTypeModel : BaseModel
    {
        [CustomRequired]
        public int CustomerTypeID { get; set; }
        [CustomRequired]
        [CustomDisplayName("CustomerType_Label_Code")]
        public string CustomerTypeCode { get; set; }
        [CustomRequired]
        [CustomDisplayName("CustomerType_Label_Name")]
        public string CustomerTypeName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
