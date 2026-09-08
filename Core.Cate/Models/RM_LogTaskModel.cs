using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_LogTaskModel : BaseModel
    {
        public int LogTaskID { get; set; }
        [CustomRequired]
        [CustomDisplayName("LogTask_Employee_Label")]
        public int Employee_ID { get; set; }

        public string FullName { get; set; }

        [CustomRequired]
        [CustomDisplayName("LogTask_LogDate_Label")]
        public DateTime? LogDate { get; set; }
        [CustomRequired]
        [CustomDisplayName("LogTask_Description_Label")]
        public string Description { get; set; }
        [CustomRequired]
        [CustomDisplayName("LogTask_CompletionPercentage_Label")]
        public int TaskManagementID{ get; set; }

        public bool? IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        [CustomDisplayName("LogTask_StartTime_Label")]
        public TimeSpan? StartTime { get; set; }
        [CustomDisplayName("LogTask_EndTime_Label")]
        public TimeSpan? EndTime { get; set; }

        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string FileAttach { get; set; }
        public List<HttpPostedFileBase> DinhKemFile { get; set; }
        public List<RM_LogTaskFilePathModel> LogTaskFilePaths { get; set; }
        public List<SelectListItem> ListEmployee { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }
        public List<RM_LogTaskFilePathModel> FilePaths { get; set; }

    }

    public class RM_LogTaskFilePathModel : BaseModel
    {
        public int FilePathID { get; set; }
        public int LogTaskID { get; set; }
        public int TaskManagementID { get; set; }
        public int CommentID { get; set; }
        public string FilePath { get; set; }
        public bool? IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class RM_LogTaskFilePathSearchModel : BaseModel
    {
        public int LogTaskID { get; set; }
        public int TaskManagementID { get; set; }  
        public int CommentID { get; set; }
    }

    public class RM_LogTaskFormModel
    {
        public RM_LogTaskModel RM_LogTask { get; set; }
        public List<RM_LogTaskModel> RM_LogTasks { get; set; }
        public bool IsAssigned { get; set; }
    }

    public class TaskTimelineModel
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; } // COMMENT | LOG | SYSTEM
        public string TypeAction { get; set; } // COMMENT | LOG | SYSTEM

        public string UserName { get; set; }
        public string Description { get; set; }

        public string Extra { get; set; } // log time range / json / etc
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public List<RM_LogTaskFilePathModel> FilePaths { get; set; } = new List<RM_LogTaskFilePathModel>();
    }
}