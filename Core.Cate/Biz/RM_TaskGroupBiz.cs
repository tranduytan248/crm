using System.Collections.Generic;
using System.Linq;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_TaskGroupBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_TaskGroup_Save = "RM_TaskGroup_Save";
        private readonly string _RM_TaskGroup_Get = "RM_TaskGroup_Get";
        private readonly string _RM_TaskGroup_Delete = "RM_TaskGroup_Delete";
        private readonly string _RM_TaskGroup_GetByID = "RM_TaskGroup_GetByID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_TaskGroup</returns>
        public List<RM_TaskGroupModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_TaskGroupModel>(_RM_TaskGroup_Get,
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
        /// Lấy toàn bộ danh sách RM_TaskGroup
        /// </summary>
        /// <returns>Danh sách RM_TaskGroup</returns>
        public List<RM_TaskGroupModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy RM_TaskGroup theo ID
        /// </summary>
        /// <returns>RM_TaskGroup</returns>
        public RM_TaskGroupModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_TaskGroupModel>(_RM_TaskGroup_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa RM_TaskGroup theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        public int Delete(RM_TaskGroupModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_TaskGroup_Delete, DATA_PROVIDER_NAME, model.TaskGroupID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật RM_TaskGroup theo dữ liệu đầu vào
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        public int Save(RM_TaskGroupModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_TaskGroup_Save, DATA_PROVIDER_NAME
                , model.TaskGroupID
                , model.TaskGroupCode
                , model.TaskGroupName
                , username);
            return result.GetValueOrDefault(0);
        }
    }
}