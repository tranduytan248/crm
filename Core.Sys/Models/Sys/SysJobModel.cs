using System;
using System.Web;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Sys.Models.Sys
{
    public class SysJobModel : BaseModel
    {
        public Guid JobId { get; set; }

        [CustomDisplayName("Job_Label_JobLibrary")]
        public HttpPostedFileBase FileLibrary { get; set; }

        [CustomRequired]
        [CustomDisplayName("Job_Label_JobName")]
        public string JobName { get; set; }

        [CustomDisplayName("Job_Label_JobDescription")]
        public string JobDescription { get; set; }

        [CustomRequired]
        [CustomDisplayName("Job_Label_CronExpression")]
        public string CronExpression { get; set; }

        [CustomDisplayName("Job_Label_JobLibrary")]
        public string JobLibrary { get; set; }

        [CustomDisplayName("Job_Label_IsActive")]
        public bool IsActive { get; set; }

        [CustomDisplayName("Job_Label_JobParrams")]
        public string JobParrams { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }
        public string SavedBy { get; set; }

        public new int? TotalRow { get; set; } = 0;
    }
}