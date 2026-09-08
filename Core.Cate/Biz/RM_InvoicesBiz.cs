using Core.Cate.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_InvoicesBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_Invoices_Get = "RM_Invoices_Get";
        private readonly string _RM_Invoices_GetByID = "RM_Invoices_GetByID";
        private readonly string _RM_Invoices_Save = "RM_Invoices_Save";
        private readonly string _RM_Invoices_Delete = "RM_Invoices_Delete";
        private readonly string _RM_InvoicesFilePath_Get = "RM_InvoicesFilePath_Get";
        private readonly string _RM_InvoicesFilePath_Delete = "RM_InvoicesFilePath_Delete";
        private readonly string _RM_InvoicesFilePath_GetByID = "RM_InvoicesFilePath_GetByID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_Invoices</returns>
        /// 
        public List<RM_InvoicesModel> LoadList(out int total, int ContractID, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_InvoicesModel>(_RM_Invoices_Get,
                DATA_PROVIDER_NAME,
                ContractID,
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
        /// Lấy toàn bộ danh sách RM_Invoices
        /// </summary>
        /// <returns>Danh sách RM_Invoices</returns>
        public List<RM_InvoicesModel> GetAll(int ContractID)
        {
            int total;
            var list = LoadList(out total, ContractID, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_Invoices theo ID
        /// </summary>
        /// <returns>Danh sách RM_Invoices</returns>
        public RM_InvoicesModel LoadDetail(string ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_InvoicesModel>(_RM_Invoices_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_Invoices theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_InvoicesModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Invoices_Delete, DATA_PROVIDER_NAME, model.InvoiceID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_Invoices theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_InvoicesModel model, DataTable dt, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Invoices_Save, DATA_PROVIDER_NAME,
                model.InvoiceID,
                model.ContractID,
                model.Serial,
                model.Number,
                model.Buyer,
                model.TaxCode,
                model.VAT_Rate,
                model.TotalAmount,
                model.VAT_Amount,
                model.TotalPayment,
                username,
                dt
                );
            return result.GetValueOrDefault(0);
        }
        #region File hóa đơn

        public List<RM_InvoicesFilePathModel> GetFilePaths(string invoice_id)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_InvoicesFilePathModel>(_RM_InvoicesFilePath_Get,
                DATA_PROVIDER_NAME,
                invoice_id);
            return data;
        }

        public int DeleteFilePath(RM_InvoicesFilePathModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_InvoicesFilePath_Delete, DATA_PROVIDER_NAME, model.FileID);
            return result.GetValueOrDefault(0);
        }
        public RM_InvoicesFilePathModel GetFilePathByID(string ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_InvoicesFilePathModel>(_RM_InvoicesFilePath_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }
        #endregion
    }
}
