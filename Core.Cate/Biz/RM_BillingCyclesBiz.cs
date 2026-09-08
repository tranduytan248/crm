using System.Collections.Generic;
using System.Linq;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_BillingCyclesBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_BillingCycles_Save = "RM_BillingCycles_Save";
        private readonly string _RM_BillingCycles_Get = "RM_BillingCycles_Get";
        private readonly string _RM_BillingCycles_Delete = "RM_BillingCycles_Delete";
        private readonly string _RM_BillingCycles_GetByID = "RM_BillingCycles_GetByID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_BillingCycles</returns>
        public List<RM_BillingCyclesModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_BillingCyclesModel>(_RM_BillingCycles_Get,
                DATA_PROVIDER_NAME, search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_BillingCycles
        /// </summary>
        /// <returns>Danh sách RM_BillingCycles</returns>
        public List<RM_BillingCyclesModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_BillingCycles theo ID
        /// </summary>
        /// <returns>Danh sách RM_BillingCycles</returns>
        public RM_BillingCyclesModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_BillingCyclesModel>(_RM_BillingCycles_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_BillingCycles theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_BillingCyclesModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_BillingCycles_Delete, DATA_PROVIDER_NAME, model.BillingCycleID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_BillingCycles theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_BillingCyclesModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_BillingCycles_Save, DATA_PROVIDER_NAME
               , model.BillingCycleID
               , model.CycleName
               , model.Islimited
               , model.CycleType
               , model.Quantity
               , username);
            return result.GetValueOrDefault(0);
        }
    }
}
