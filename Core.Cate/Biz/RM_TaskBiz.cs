using System.Collections.Generic;
using System.Linq;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_TaskBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_Task_Save = "RM_Task_Save";
        private readonly string _RM_Task_Get = "RM_Task_Get";
        private readonly string _RM_Task_Delete = "RM_Task_Delete";
        private readonly string _RM_Task_GetByID = "RM_Task_GetByID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        public List<RM_TaskModel> LoadList(out int total, RM_TaskSearchModel model, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_TaskModel>(_RM_Task_Get,
                  DATA_PROVIDER_NAME,
                  model?.Keyword,     
                  model?.TaskGroupID,
                  model?.TaskTypeID,
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
        /// Lấy toàn bộ danh sách RM_Task
        /// </summary>
        public List<RM_TaskModel> GetAll()
        {
            int total;
            return LoadList(out total, null, null);
        }

        /// <summary>
        /// Lấy RM_Task theo ID
        /// </summary>
        public RM_TaskModel LoadDetail(int ID)
        {
            return AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_TaskModel>(_RM_Task_GetByID, DATA_PROVIDER_NAME, ID);
        }

        /// <summary>
        /// Xóa RM_Task theo ID
        /// </summary>
        public int Delete(RM_TaskModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Task_Delete, DATA_PROVIDER_NAME, model.TaskID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Thêm/Cập nhật RM_Task
        /// </summary>
        public int Save(RM_TaskModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Task_Save, DATA_PROVIDER_NAME,
                model.TaskID,
                model.TaskParentsID,
                model.TaskCode,
                model.TaskName,
                model.Note,
                model.TaskGroupID,
                username);
            return result.GetValueOrDefault(0);
        }
    }
}
