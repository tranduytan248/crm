using System.Collections.Generic;
using System.Linq;
using Core.Cate.Models;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_CommentBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_Comment_GetByTaskManagementID = "RM_Comment_GetByTaskManagementID";
        private readonly string _RM_Comment_Save = "RM_Comment_Save";
        private readonly string _RM_Comment_Delete = "RM_Comment_Delete";
        private readonly string _RM_Comment_GetByID = "RM_Comment_GetByID";
        private readonly string _RM_Comment_GetByProjectID = "RM_Comment_GetByProjectID";

        public List<RM_CommentModel> LoadListByTaskManagementID(int taskManagementID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_CommentModel>(
                _RM_Comment_GetByTaskManagementID,
                DATA_PROVIDER_NAME,
                taskManagementID // @TaskManagementID
            );

            return data;
        }

        public List<RM_CommentViewByProjectModel> LoadListByProjectID(int ProjectID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_CommentViewByProjectModel>(
                _RM_Comment_GetByProjectID,
                DATA_PROVIDER_NAME,
                ProjectID // @TaskManagementID
            );
            return data;
        }

        public RM_CommentModel GetByID(int commentID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_CommentModel>(
                _RM_Comment_GetByID,
                DATA_PROVIDER_NAME,
                commentID // @CommentID
            );

            return data?.FirstOrDefault();
        }

        public int Save(RM_CommentModel model, string userName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_Comment_Save,
                DATA_PROVIDER_NAME,
                model.CommentID,       // @CommentID
                model.TaskManagementID, // @TaskManagementID
                model.ParentCommentID,  // @ParentCommentID
                model.Content,          // @Content
                userName,        // @CreatedBy
                model.Employee_ID
            );

            return result.GetValueOrDefault(0);
        }

        public int Delete(RM_CommentModel model, string username)
        {
            return AppProcessor.ProcedureProvider.Execute(
                _RM_Comment_Delete,
                DATA_PROVIDER_NAME,
                model.CommentID, // @CommentID
                username
            ).GetValueOrDefault(0);
        }
    }
}
