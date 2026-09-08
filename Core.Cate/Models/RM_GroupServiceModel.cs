using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_GroupServiceModel : BaseModel
    {
        public int GroupServiceID { get; set; }
        [CustomRequired]
        [CustomDisplayName("GroupService_Name_Label")]
        public string NameGroup { get; set; }
        public int? ParentGroupServiceID { get; set; }
        public string UserCreated { get; set; }
        public DateTime? CreatedDated { get; set; }
        public string UserUpdated { get; set; }
        public DateTime? UpdatedDated { get; set; }
        [CustomDisplayName("GroupService_Status_Label")]
        public bool IsActived { get; set; }
        public bool IsDeleted { get; set; }
        public int Level { get; set; }
    }
}
