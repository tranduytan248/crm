using System;
using System.Collections.Generic;
using System.Web;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_CommentModel : BaseModel
    {
        public int CommentID { get; set; }
        public int TaskManagementID { get; set; }
        public int ParentCommentID { get; set; }
        public int Employee_ID { get; set; }
        public string Content { get; set; }
        public string EmployeeName { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }
        public List<RM_LogTaskFilePathModel> FilePaths { get; set; }

    }

    public class RM_CommentViewByProjectModel : RM_CommentModel
    {
        public string TaskName { get; set; }
        public string PriorityName { get; set; }
        public string AssigneeNames { get; set; }
        public string ListFilePaths { get; set; } = "";
    }
    public class RM_CommentByTaskModel
    {
        public int TaskManagementID { get; set; }
        public string TaskName { get; set; }
        public string PriorityName { get; set; }
        public string AssigneeNames { get; set; }
        public List<RM_CommentViewByProjectModel> Comments { get; set; }
    }
    public class RM_CommentByProjectModel
    {
        public string Ngay { get; set; }
        public List<RM_CommentByTaskModel> CommentsByTask { get; set; }
    }
}
