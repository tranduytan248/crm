using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web.Helpers;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_CustomerStatusBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_CustomerStatus_Save = "RM_CustomerStatus_Save";
        private readonly string _RM_CustomerStatus_Get = "RM_CustomerStatus_Get";
        private readonly string _RM_CustomerStatus_Delete = "RM_CustomerStatus_Delete";
        private readonly string _RM_CustomerStatus_GetByID = "RM_CustomerStatus_GetByID";


        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_CustomerStatus</returns>
        public List<RM_CustomerStatusModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_CustomerStatusModel>(_RM_CustomerStatus_Get,
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
        /// Lấy toàn bộ danh sách RM_CustomerStatus
        /// </summary>
        /// <returns>Danh sách RM_CustomerStatus</returns>
        public List<RM_CustomerStatusModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_CustomerStatus theo ID
        /// </summary>
        /// <returns>Danh sách RM_CustomerStatus</returns>
        public RM_CustomerStatusModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_CustomerStatusModel>(_RM_CustomerStatus_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_CustomerStatus theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_CustomerStatusModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_CustomerStatus_Delete, DATA_PROVIDER_NAME, model.CustomerStatusID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_CustomerStatus theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_CustomerStatusModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_CustomerStatus_Save, DATA_PROVIDER_NAME
                , model.CustomerStatusID
                , model.StatusCode
                , model.StatusName
                , model.StatusClass
               , username);
            return result.GetValueOrDefault(0);
        }
    }
}
