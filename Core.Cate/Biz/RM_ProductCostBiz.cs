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
    public class RM_ProductCostBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_ProductCost_Save = "RM_ProductCost_Save";
        private readonly string _RM_ProductCost_Get = "RM_ProductCost_Get";
        private readonly string _RM_ProductCost_Delete = "RM_ProductCost_Delete";
        private readonly string _RM_ProductCost_GetByID = "RM_ProductCost_GetByID";
        private readonly string _RM_ProductCost_GetByProductProjectID = "RM_ProductCost_GetByProductProjectID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_ProductCost</returns>
        public List<RM_ProductCostModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProductCostModel>(_RM_ProductCost_Get,
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
        /// Lấy toàn bộ danh sách RM_ProductCost
        /// </summary>
        /// <returns>Danh sách RM_ProductCost</returns>
        public List<RM_ProductCostModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_ProductCost theo ID
        /// </summary>
        /// <returns>Danh sách RM_ProductCost</returns>
        public RM_ProductCostModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ProductCostModel>(_RM_ProductCost_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Lấy danh sách RM_ProductProject theo ProjectID
        /// </summary>
        /// <returns>Danh sách RM_ProductProject</returns>
        public List<RM_ProductCostModel> GetByProductProjectID(int ID)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProductCostModel>(_RM_ProductCost_GetByProductProjectID, DATA_PROVIDER_NAME, ID);
            return list;
        }

        /// <summary>
        /// Xóa danh sách RM_ProductCost theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_ProductCostModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ProductCost_Delete, DATA_PROVIDER_NAME, model.ProductCostID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_ProductCost theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_ProductCostModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ProductCost_Save, DATA_PROVIDER_NAME
                , model.ProductCostID
                , model.ProductProjectID
                , model.CostTypeID
                , model.Amount
                , model.PaymentDate
                , model.Note
               , username);
            return result.GetValueOrDefault(0);
        }
    }
}
