using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Core.API.Models;
using Core.Cate.Models;
using Core.Log.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class InvBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _TransactionNoPaid_Get = "CSKH_GetListInvByContractAndPeriod";
        private readonly string _Contract_GetByAccountUser = "CSKH_Contract_GetByAccountUser";
        private readonly string _ClockRecord_GetByContract = "CSKH_GetClockRecordByContractAndPeriod";
        private readonly string _Transactions_GetByContractAndPeriod = "CSKH_GetInfoContractByContractAndPeriod";

        /// <summary>
        /// Lấy toàn bộ danh sách giao dịch theo hợp đồng
        /// </summary>
        /// <returns>Danh sách Cate_NewsCategories</returns>
        public List<ARTransactionHistoryModel> Transactions_Get(string contractCode)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<ARTransactionHistoryModel>(_TransactionNoPaid_Get,
                DATA_PROVIDER_NAME, contractCode);
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách giao dịch theo hợp đồng
        /// </summary>
        /// <returns>Danh sách Cate_NewsCategories</returns>
        public ARTransactionHistoryModel Transactions_GetByContractAndPeriod(string contractCode, DateTime Period)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<ARTransactionHistoryModel>(_Transactions_GetByContractAndPeriod,
                DATA_PROVIDER_NAME, contractCode);
            return data.FirstOrDefault();
        }

        /// <summary>
        /// Danh sách hợp đồng theo tài khoản
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        public List<CSKH_KH_CONTRACT_Model> Contract_GetByAccountUser(string accountUser, string typeAccount)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<CSKH_KH_CONTRACT_Model>(_Contract_GetByAccountUser,
                DATA_PROVIDER_NAME, accountUser, typeAccount);
            return data;
        }

        /// <summary>
        /// Lấy danh sách tiêu thụ nước theo tài khoản
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        public List<ClockRecordPeriodHistoryModel> ClockRecord_GetByContract(string contractCode)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<ClockRecordPeriodHistoryModel>(_ClockRecord_GetByContract,
                DATA_PROVIDER_NAME, contractCode);
            return data;
        }
    }
}
