using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_ContractsBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_Contracts_Get = "RM_Contracts_Get";
        private readonly string _RM_Contracts_GetById = "RM_Contracts_GetById";
        private readonly string _RM_Contracts_GetByCustomerId = "RM_Contracts_GetByCustomerId";
        private readonly string _RM_Contracts_Delete = "RM_Contracts_Delete";
        private readonly string _RM_Contracts_Save = "RM_Contracts_Save";
        private readonly string _RM_Contracts_GetByProjectID = "RM_Contracts_GetByProjectID";
        private readonly string _RM_Contracts_GetByProductProjectID = "RM_Contracts_GetByProductProjectID";

        private readonly string _RM_ContractFilePath_Get = "RM_ContractFilePath_Get";
        private readonly string _RM_ContractFilePath_GetByID = "RM_ContractFilePath_GetByID";
        private readonly string _RM_ContractFilePath_Save = "RM_ContractFilePath_Save";
        private readonly string _RM_ContractFilePath_Delete = "RM_ContractFilePath_Delete";

        public List<RM_ContractsModel> LoadList(out int total, RM_ContractsSearchModel model, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "0", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ContractsModel>(_RM_Contracts_Get,
                DATA_PROVIDER_NAME,
                string.IsNullOrEmpty(model?.Keyword) ? (object)DBNull.Value : model.Keyword,
                model?.StatusID == 0 ? (object)DBNull.Value : model.StatusID,
                search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        public List<RM_ContractsModel> GetAll()
        {
            int total;
            return LoadList(out total, new RM_ContractsSearchModel(), null);
        }

        public RM_ContractsModel LoadDetail(int ID)
        {
            return AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ContractsModel>(_RM_Contracts_GetById, DATA_PROVIDER_NAME, ID);
        }

        /// <summary>
        /// Lấy danh sách RM_ContractsModel theo CustomerID
        /// </summary>
        /// <returns>Danh sách RM_ContractsModel</returns>
        public List<RM_ContractsModel> GetByCustomerID(int ID)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ContractsModel>(_RM_Contracts_GetByCustomerId, DATA_PROVIDER_NAME, ID);
            return list;
        }
        public int Delete(RM_ContractsModel model, string username)
        {
            return AppProcessor.ProcedureProvider.Execute(_RM_Contracts_Delete, DATA_PROVIDER_NAME,
                model.ContractID, username).GetValueOrDefault(0);
        }

        public int Save(RM_ContractsModel model,DataTable ngayNhacs, string username)
        {
            return AppProcessor.ProcedureProvider.Execute(_RM_Contracts_Save, DATA_PROVIDER_NAME,
                model.ContractID,
                model.ProductProjectID,
                model.ContractCode,
                model.ContractName,
                model.BillingCycleID,
                model.ReminderType,
                model.ReminderDayOfMonth,
                model.SignDate,
                model.StartDate,
                model.EndDate,
                model.ContractValue,
                model.VAT,
                model.TotalAmount,
                model.StatusContract,
                (object)DBNull.Value,
                username,
                ngayNhacs).GetValueOrDefault(0);
        }

        public List<RM_ContractFilePathModel> GetFilePaths(int contractID)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ContractFilePathModel>(
                _RM_ContractFilePath_Get, DATA_PROVIDER_NAME, contractID) ?? new List<RM_ContractFilePathModel>();
        }

        public RM_ContractFilePathModel GetFilePathById(int id)
        {
            return AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ContractFilePathModel>(
                _RM_ContractFilePath_GetByID, DATA_PROVIDER_NAME, id);
        }

        public int SaveFilePath(RM_ContractFilePathModel model, string username)
        {
            return AppProcessor.ProcedureProvider.Execute(_RM_ContractFilePath_Save, DATA_PROVIDER_NAME,
                model.FilePathID, model.ContractID, model.FilePath, username).GetValueOrDefault(0);
        }

        public int DeleteFilePath(int filePathID, string username)
        {
            return AppProcessor.ProcedureProvider.Execute(_RM_ContractFilePath_Delete, DATA_PROVIDER_NAME,
                filePathID, username).GetValueOrDefault(0);
        }

        /// <summary>
        /// Lấy danh sách RM_Contracts theo ProjectID
        /// </summary>
        /// <returns>Danh sách RM_Contracts</returns>
        public List<RM_ContractsModel> GetByProjectID(int ID)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ContractsModel>(_RM_Contracts_GetByProjectID, DATA_PROVIDER_NAME, ID);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_Contracts theo ProductProjectID
        /// </summary>
        /// <returns>Danh sách RM_Contracts</returns>
        public List<RM_ContractsModel> GetByProductProjectID(int ID)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ContractsModel>(_RM_Contracts_GetByProductProjectID, DATA_PROVIDER_NAME, ID);
            return list;
        }
    }
}