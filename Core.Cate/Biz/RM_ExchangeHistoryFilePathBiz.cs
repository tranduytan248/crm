using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_ExchangeHistoryFilePathBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _rm_exchangehistoryfilepath_Save = "RM_ExchangeHistoryFilePath_Save";
        private readonly string _rm_exchangehistoryfilepath_Get = "RM_ExchangeHistoryFilePath_Get";
        private readonly string _rm_exchangehistoryfilepath_Delete = "RM_ExchangeHistoryFilePath_Delete";
        private readonly string _rm_exchangehistoryfilepath_GetByID = "RM_ExchangeHistoryFilePath_GetByID";
        private readonly string _rm_exchangehistoryfilepath_GetByEHID = "RM_ExchangeHistoryFilePath_GetByEHID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_ExchangeHistoryFilePath</returns>
        public List<RM_ExchangeHistoryFilePathModel> LoadList(RM_ExchangeHistoryFilePathSearchModel model, out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "1", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ExchangeHistoryFilePathModel>(_rm_exchangehistoryfilepath_Get,
                DATA_PROVIDER_NAME, model.BusinessOpportunityID, model.ExchangeHistoryID, search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ExchangeHistoryFilePath
        /// </summary>
        /// <returns>Danh sách RM_ExchangeHistoryFilePath</returns>
        public List<RM_ExchangeHistoryFilePathModel> GetAll()
        {
            int total;
            var model = new RM_ExchangeHistoryFilePathSearchModel();
            var list = LoadList(model, out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_ExchangeHistoryFilePath theo ID
        /// </summary>
        /// <returns>Danh sách RM_ExchangeHistoryFilePath</returns>
        public RM_ExchangeHistoryFilePathModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ExchangeHistoryFilePathModel>(_rm_exchangehistoryfilepath_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_ExchangeHistoryFilePath theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(int ID, string UserName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_exchangehistoryfilepath_Delete, DATA_PROVIDER_NAME, ID, UserName);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_ExchangeHistoryFilePath theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_ExchangeHistoryFilePathModel model, string savedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_exchangehistoryfilepath_Save, DATA_PROVIDER_NAME
               , model.FilePathID
               , model.ExchangeHistoryID
               , model.FilePath
               , model.IsDeleted
            , savedBy);
            return result.GetValueOrDefault(0);
        }
        public List<RM_ExchangeHistoryFilePathModel> GetByEHID(int ID)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ExchangeHistoryFilePathModel>(_rm_exchangehistoryfilepath_GetByEHID, DATA_PROVIDER_NAME, ID);
            return list;
        }
    }
}
