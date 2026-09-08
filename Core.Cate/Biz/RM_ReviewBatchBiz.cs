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
    public class RM_ReviewBatchBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_ReviewBatch_Save = "RM_ReviewBatch_Save";
        private readonly string _RM_ReviewBatch_Get = "RM_ReviewBatch_Get";
        private readonly string _RM_ReviewBatch_Delete = "RM_ReviewBatch_Delete";
        private readonly string _RM_ReviewBatch_GetByID = "RM_ReviewBatch_GetByID";


        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_ReviewBatch</returns>
        public List<RM_ReviewBatchModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ReviewBatchModel>(_RM_ReviewBatch_Get,
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
        /// Lấy toàn bộ danh sách RM_ReviewBatch
        /// </summary>
        /// <returns>Danh sách RM_ReviewBatch</returns>
        public List<RM_ReviewBatchModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_ReviewBatch theo ID
        /// </summary>
        /// <returns>Danh sách RM_ReviewBatch</returns>
        public RM_ReviewBatchModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ReviewBatchModel>(_RM_ReviewBatch_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_ReviewBatch theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_ReviewBatchModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ReviewBatch_Delete, DATA_PROVIDER_NAME, model.ReviewBatchID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_ReviewBatch theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_ReviewBatchModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ReviewBatch_Save, DATA_PROVIDER_NAME
                , model.ReviewBatchID
                , model.BatchCode
                , model.BatchName
                , model.FromDate
                , model.ToDate
               , username);
            return result.GetValueOrDefault(0);
        }
    }
}
