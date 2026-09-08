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
    public class RM_OpportunityStatusBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_OpportunityStatus_Save = "RM_OpportunityStatus_Save";
        private readonly string _RM_OpportunityStatus_Get = "RM_OpportunityStatus_Get";
        private readonly string _RM_OpportunityStatus_Delete = "RM_OpportunityStatus_Delete";
        private readonly string _RM_OpportunityStatus_GetByID = "RM_OpportunityStatus_GetByID";


        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_OpportunityStatus</returns>
        public List<RM_OpportunityStatusModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_OpportunityStatusModel>(_RM_OpportunityStatus_Get,
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
        /// Lấy toàn bộ danh sách RM_OpportunityStatus
        /// </summary>
        /// <returns>Danh sách RM_OpportunityStatus</returns>
        public List<RM_OpportunityStatusModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_OpportunityStatus theo ID
        /// </summary>
        /// <returns>Danh sách RM_OpportunityStatus</returns>
        public RM_OpportunityStatusModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_OpportunityStatusModel>(_RM_OpportunityStatus_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_OpportunityStatus theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_OpportunityStatusModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_OpportunityStatus_Delete, DATA_PROVIDER_NAME, model.OpportunityStatusID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_OpportunityStatus theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_OpportunityStatusModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_OpportunityStatus_Save, DATA_PROVIDER_NAME
                , model.OpportunityStatusID
                , model.StatusCode
                , model.StatusName
                , model.StatusClass
               , username);
            return result.GetValueOrDefault(0);
        }
    }
}
