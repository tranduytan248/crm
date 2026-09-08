using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_ProjectTaskModel : BaseModel
    {
        public int ProjectTaskID { get; set; }

        [CustomRequired]
        [CustomDisplayName("ProjectTask_ProductProject_Label")]
        public int ProductProjectID { get; set; }
        public int ProjectID { get; set; }

        [CustomRequired]
        [CustomDisplayName("ProjectTask_Task_Label")]
        public int TaskID { get; set; }
        public string TaskName { get; set; }

        [CustomDisplayName("ProjectTask_AssignedEmployee_Label")]
        public List<int> AssignedEmployeeIDs { get; set; }
        public string AssignedEmployeeNames { get; set; }

        [CustomDisplayName("ProjectTask_StartDate_Label")]
        public DateTime? StartDate { get; set; }

        [CustomDisplayName("ProjectTask_CompletedDate_Label")]
        public DateTime? CompletedDate { get; set; }

        [CustomDisplayName("ProjectTask_Status_Label")]
        public byte? Status { get; set; }
        public string StatusName { get; set; }

        [CustomDisplayName("ProjectTask_CompletionPercentage_Label")]
        public int? CompletionPercentage { get; set; }

        [CustomDisplayName("ProjectTask_Note_Label")]
        public string Note { get; set; }
        public bool IsTaskManagementSource { get; set; }
        public int? TaskManagementID { get; set; }

        public bool? IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public List<SelectListItem> ListTask { get; set; }
        public List<SelectListItem> ListEmployee { get; set; }
        public List<RM_StatusModel> ListTaskStatus { get; set; }
    }

    public class RM_ProjectTaskBulkAddModel : BaseModel
    {
        public int ProjectID { get; set; }
        public int ProductProjectID { get; set; }

        [CustomRequired]
        [CustomDisplayName("Task_Group_Label")]
        public int? TaskGroupID { get; set; }

        public string Keyword { get; set; }
        public int? TaskTypeID { get; set; }
        public byte? Status { get; set; }

        public List<RM_TaskGroupModel> TaskGroups { get; set; }
        public List<RM_TaskTypeModel> TaskTypes { get; set; }
        public List<RM_PriorityModel> Priorities { get; set; }
        public List<SelectListItem> ListEmployee { get; set; }
        public List<RM_StatusModel> ListTaskStatus { get; set; }
        public List<RM_ProjectTaskBulkAddItemModel> Items { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }
    }

    public class RM_ProjectTaskBulkAddItemModel : BaseModel
    {
        public bool IsSelected { get; set; }
        public bool IsExistingProjectTask { get; set; }

        public int TaskID { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public int? TaskParentsID { get; set; }
        public string ParentsTask { get; set; }
        public int? TaskGroupID { get; set; }
        public string TaskGroupName { get; set; }
        public int? TaskTypeID { get; set; }
        public string TaskTypeName { get; set; }
        public int? SortOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public byte? StatusID { get; set; }
        public int? PriorityID { get; set; }
        public decimal? EstimatedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public int? CompletionPercentage { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }

        public string AssigneeIDs { get; set; }
        public List<int> AssigneeIDList { get; set; } = new List<int>();
        public string FileIndexes { get; set; }

    }
}
