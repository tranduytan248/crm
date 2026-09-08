using System;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Attributes;

namespace Core.Cate.Models
{
    public class RM_TaskModel : BaseModel
    {
        public int TaskID { get; set; }
        [CustomDisplayName("Task_ParentsTask_Label")]
        public int? TaskParentsID { get; set; }
        public string ParentsTask { get; set; }
        [CustomRequired]
        [CustomDisplayName("Task_Group_Label")]
        public int? TaskGroupID { get; set; }

        public string TaskGroupName { get; set; }       

        [CustomRequired]
        [CustomDisplayName("Task_Code_Label")]
        public string TaskCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("Task_Name_Label")]
        public string TaskName { get; set; }

        [CustomDisplayName("Task_Type_Label")]
        public int? TaskTypeID { get; set; }
        public string TaskTypeName { get; set; }

        public int? SortOrder { get; set; }

        [CustomDisplayName("Task_Note_Label")]
        public string Note { get; set; }
        public bool? IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }

}
