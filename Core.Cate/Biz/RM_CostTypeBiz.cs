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
    public class RM_CostTypeBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_CostType_Save = "RM_CostType_Save";
        private readonly string _RM_CostType_Get = "RM_CostType_Get";
        private readonly string _RM_CostType_Delete = "RM_CostType_Delete";
        private readonly string _RM_CostType_GetByID = "RM_CostType_GetByID";


        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_CostType</returns>
        public List<RM_CostTypeModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_CostTypeModel>(_RM_CostType_Get,
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
        /// Lấy toàn bộ danh sách RM_CostType
        /// </summary>
        /// <returns>Danh sách RM_CostType</returns>
        public List<RM_CostTypeModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_CostType theo ID
        /// </summary>
        /// <returns>Danh sách RM_CostType</returns>
        public RM_CostTypeModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_CostTypeModel>(_RM_CostType_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_CostType theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_CostTypeModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_CostType_Delete, DATA_PROVIDER_NAME, model.CostTypeID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_CostType theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_CostTypeModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_CostType_Save, DATA_PROVIDER_NAME
                , model.CostTypeID
                , model.CostTypeCode
                , model.CostTypeName
               , username);
            return result.GetValueOrDefault(0);
        }
    }
}
