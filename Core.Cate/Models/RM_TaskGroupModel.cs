using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Models
{
    public class RM_TaskGroupModel : BaseModel
    {
        public int TaskGroupID { get; set; }

        [CustomRequired]
        [CustomDisplayName("TaskGroup_Code_Label")]
        public string TaskGroupCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("TaskGroup_Name_Label")]
        public string TaskGroupName { get; set; }

        public bool? IsDeleted { get; set; }
    }
}