using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_StatusBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_Status_Get = "RM_Status_Get";
        private readonly string _RM_Status_GetByID = "RM_Status_GetByID";
        private readonly string _RM_Status_Save = "RM_Status_Save";
        private readonly string _RM_Status_Delete = "RM_Status_Delete";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_Status</returns>
        /// 
        public List<RM_StatusModel> LoadList(out int total, BaseSearchModel search, string statusKey)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_StatusModel>(_RM_Status_Get,
                DATA_PROVIDER_NAME,
                statusKey,
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
        /// Lấy toàn bộ danh sách RM_Status
        /// </summary>
        /// <returns>Danh sách RM_Status</returns>
        public List<RM_StatusModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null, "");
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_Status theo ID
        /// </summary>
        /// <returns>Danh sách RM_Status</returns>
        public RM_StatusModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_StatusModel>(_RM_Status_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_Status theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_StatusModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Status_Delete, DATA_PROVIDER_NAME, model.ID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_Status theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_StatusModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Status_Save, DATA_PROVIDER_NAME,
                model.ID, 
                model.StatusKey, 
                model.StatusCode, 
                model.StatusName, 
                model.StatusClass,
                model.SortOrder,
                model.Project_SuccessRate,
                username);
            return result.GetValueOrDefault(0);
        }
    }
}
