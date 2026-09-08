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
    public class RM_RevenueReceivedBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_RevenueReceived_Save = "RM_RevenueReceived_Save";
        private readonly string _RM_RevenueReceived_Get = "RM_RevenueReceived_Get";
        private readonly string _RM_RevenueReceived_Delete = "RM_RevenueReceived_Delete";
        private readonly string _RM_RevenueReceived_GetByID = "RM_RevenueReceived_GetByID";
        private readonly string _RM_RevenueReceived_GetByProductProjectID = "RM_RevenueReceived_GetByProductProjectID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_RevenueReceived</returns>
        public List<RM_RevenueReceivedModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_RevenueReceivedModel>(_RM_RevenueReceived_Get,
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
        /// Lấy toàn bộ danh sách RM_RevenueReceived
        /// </summary>
        /// <returns>Danh sách RM_RevenueReceived</returns>
        public List<RM_RevenueReceivedModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_RevenueReceived theo ID
        /// </summary>
        /// <returns>Danh sách RM_RevenueReceived</returns>
        public RM_RevenueReceivedModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_RevenueReceivedModel>(_RM_RevenueReceived_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Lấy danh sách RM_ProductProject theo ProjectID
        /// </summary>
        /// <returns>Danh sách RM_ProductProject</returns>
        public List<RM_RevenueReceivedModel> GetByProductProjectID(int ID)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_RevenueReceivedModel>(_RM_RevenueReceived_GetByProductProjectID, DATA_PROVIDER_NAME, ID);
            return list;
        }

        /// <summary>
        /// Xóa danh sách RM_RevenueReceived theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_RevenueReceivedModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_RevenueReceived_Delete, DATA_PROVIDER_NAME, model.RevenueReceivedID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_RevenueReceived theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_RevenueReceivedModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_RevenueReceived_Save, DATA_PROVIDER_NAME
                , model.RevenueReceivedID
                , model.ProductProjectID
                , model.Amount
                , model.ReceivedDate
                , model.ReceivedTime
                , model.Note
               , username);
            return result.GetValueOrDefault(0);
        }
    }
}
