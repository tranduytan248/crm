using System;
using System.Collections.Generic;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_ProjectWeeklyTaskReportSearchModel : BaseModel
    {
        public string WeekDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string keyword { get; set; }
    }

    public class RM_ProjectWeeklyTaskReportModel : BaseModel
    {
        public int ProjectID { get; set; }

        public string ProjectName { get; set; }

        public string AMNames { get; set; }

        /// <summary>
        /// Danh sách công việc phát sinh trong tuần
        /// </summary>
        public string WeeklyTasks { get; set; }

        /// <summary>
        /// Công việc được trao đổi gần nhất
        /// </summary>
        public string LastTaskName { get; set; }

        /// <summary>
        /// JSON trả về từ SP
        /// </summary>
        public string LastComment { get; set; }

        /// <summary>
        /// Danh sách comment đã deserialize từ LastComment
        /// </summary>
        public List<ProjectCommentDto> CommentItems { get; set; }
            = new List<ProjectCommentDto>();

        /// <summary>
        /// Thời gian trao đổi gần nhất
        /// </summary>
        public DateTime? LastTaskDate { get; set; }

        /// <summary>
        /// Người trao đổi gần nhất
        /// </summary>
        public string LastCommentBy { get; set; }

        /// <summary>
        /// Ngày trao đổi gần nhất
        /// </summary>
        public DateTime? LastCommentDate { get; set; }
    }

    public class ProjectCommentDto
    {
        public int TaskManagementID { get; set; }

        public string TaskName { get; set; }

        public string Content { get; set; }

        public string EmployeeName { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class RM_ProjectWeeklyTaskReportModelResultModel
    {
        public DateTime WeekFrom { get; set; }

        public DateTime WeekTo { get; set; }

        public List<RM_ProjectWeeklyTaskReportModel> Items { get; set; }
            = new List<RM_ProjectWeeklyTaskReportModel>();
    }
}