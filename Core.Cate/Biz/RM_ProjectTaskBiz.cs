using System.Collections.Generic;
using System.Linq;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_ProjectTaskBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_ProjectTask_Save = "RM_ProjectTask_Save";
        private readonly string _RM_ProjectTask_Get = "RM_ProjectTask_Get";
        private readonly string _RM_ProjectTask_Delete = "RM_ProjectTask_Delete";
        private readonly string _RM_ProjectTask_GetByID = "RM_ProjectTask_GetByID";
        private readonly string _RM_ProjectTask_GetByProductProjectID = "RM_ProjectTask_GetByProductProjectID";
        private readonly string _RM_ProjectTask_GetByProjectID = "RM_ProjectTask_GetByProjectID";

        /// <summary>
        /// Lấy danh sách có phân trang + search
        /// </summary>
        public List<RM_ProjectTaskModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProjectTaskModel>(_RM_ProjectTask_Get,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy tất cả theo ProductProjectID
        /// </summary>
        public List<RM_ProjectTaskModel> GetByProductProjectID(int productProjectID)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProjectTaskModel>(
                _RM_ProjectTask_GetByProductProjectID, DATA_PROVIDER_NAME, productProjectID);
        }
        public List<RM_ProjectTaskModel> GetByProjectID(int projectID)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProjectTaskModel>(
                _RM_ProjectTask_GetByProjectID, DATA_PROVIDER_NAME, projectID);
        }


        /// <summary>
        /// Lấy theo ID
        /// </summary>
        public RM_ProjectTaskModel LoadDetail(int ID)
        {
            return AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ProjectTaskModel>(
                _RM_ProjectTask_GetByID, DATA_PROVIDER_NAME, ID);
        }

        /// <summary>
        /// Xóa mềm
        /// </summary>
        public int Delete(RM_ProjectTaskModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ProjectTask_Delete,
                DATA_PROVIDER_NAME, model.ProjectTaskID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Thêm/Cập nhật
        /// </summary>
        public int Save(RM_ProjectTaskModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ProjectTask_Save, DATA_PROVIDER_NAME,
                model.ProjectTaskID,
                model.ProductProjectID,
                model.TaskID,
                model.AssignedEmployeeIDs,
                model.StartDate,
                model.CompletedDate,
                model.Status,
                model.CompletionPercentage,
                model.Note,
                username);
            return result.GetValueOrDefault(0);
        }
    }
}