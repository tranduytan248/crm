using System;
using System.Collections.Generic;
using TSFramework.Libs.Models.Base;
using System.Web.Mvc;
using System.Web;
using TSFramework.Libs.Attributes;


namespace Core.Cate.Models
{
    public class RM_TaskManagementModel : BaseModel
    {
        public int TaskManagementID { get; set; }
        public int ProductProjectID { get; set; }
        public int ProjectID { get; set; }
        public int? BusinessOpportunityID { get; set; }
        [CustomDisplayName("Task_ParentTask_Label")]
        public int? ParentTaskID { get; set; }
        [CustomRequired]
        [CustomDisplayName("Task_Name_Label")]
        public string TaskName { get; set; }
        public string ParentTaskName { get; set; }
        public int TaskID { get; set; }
        public string TaskCode { get; set; }
        public string TaskTypeName { get; set; }
        [CustomRequired]
        [CustomDisplayName("Task_Status_Label")]
        public int? StatusID { get; set; }
        public string StatusName { get; set; }
        public string StatusClass { get; set; }
        [CustomDisplayName("Priority_Label")]
        public int? PriorityID { get; set; }
        public string PriorityName { get; set; }
        public string PriorityClass { get; set; }
        [CustomRequired]
        [CustomDisplayName("Task_Type_Label")]
        public int? TaskTypeID { get; set; }
        [CustomDisplayName("Task_StartDate_Label")]
        public DateTime? StartDate { get; set; }
        [CustomDisplayName("Task_EndDate_Label")]
        public DateTime? EndDate { get; set; }
        public string AssigneeIDs { get; set; }
        [CustomDisplayName("Task_Assignee_Label")]
        public List<int> AssigneeIDList { get; set; } = new List<int>();
        public string AssigneeNames { get; set; }
        public string AssigneeAvatars { get; set; }
        [CustomDisplayName("TaskManagement_EstimatedHours_Label")]
        public decimal? EstimatedHours { get; set; }
        [CustomDisplayName("TaskManagement_ActualHours_Label")]
        public decimal? ActualHours { get; set; }
        [CustomDisplayName("TaskManagement_CompletionPercentage_Label")]
        public int? CompletionPercentage { get; set; }
        [CustomDisplayName("TaskManagement_Description_Label")]
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public new string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool IsEdit { get; set; }
        public bool HasComment { get; set; }
        public string ProjectName { get; set; }
        public List<RM_TaskTypeModel> TaskTypes { get; set; }
        public List<RM_PriorityModel> Priorities { get; set; }
        public List<SelectListItem> ListEmployee { get; set; }
        public List<RM_StatusModel> ListTaskStatus { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }
        public List<RM_LogTaskFilePathModel> FilePaths { get; set; }
    }
    public class RM_TaskManagementSearchModel
    {
        public string Keyword { get; set; }
        public int ProjectID { get; set; }
        // public int? StatusID { get; set; }
        // public int? PriorityID { get; set; }
        // public int? TaskTypeID { get; set; }
        // public int? ProductProjectID { get; set; }
        // public int? BusinessOpportunityID { get; set; }
        // public int? EmployeeID { get; set; }
        // public DateTime? FromDate { get; set; }
        // public DateTime? ToDate { get; set; }
    }
}
