using Core.API.Models;
using Core.Cate.Biz;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Modules.API.Models.KhachHang;
using Modules.API.Models.NewsKhachHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TSFramework.Libs.Processors;

namespace Core.API.Biz.KHACHHANG
{
    public class PaymentHistoryBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _API_KhachHang_GetPaymentHistory = "API_KhachHang_GetPaymentHistory";
        /// <summary>
        /// Lấy lịch sử thanh toán theo khoảng thời gian
        /// </summary>
        /// <param name="contractCode">Mã hợp đồng</param>
        /// <param name="fromDate">Từ ngày (string format yyyy-MM-dd)</param>
        /// <param name="toDate">Đến ngày (string format yyyy-MM-dd)</param>
        /// <returns>Danh sách lịch sử thanh toán</returns>
        public List<PaymentHistoryModel> GetPaymentHistory(string contractCode, string fromDate = null, string toDate = null)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<PaymentHistoryModel>(
                _API_KhachHang_GetPaymentHistory,DATA_PROVIDER_NAME, contractCode, fromDate, toDate);
        }
        /// <summary>
        /// Lấy lịch sử thanh toán 12 tháng gần nhất
        /// </summary>
        /// <param name="contractId">Mã hợp đồng</param>
        /// <returns>Danh sách lịch sử thanh toán</returns>
        /// 
        public List<PaymentHistoryModel> GetPaymentHistory(string contractId)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<PaymentHistoryModel>(_API_KhachHang_GetPaymentHistory,DATA_PROVIDER_NAME, contractId);
        }
    }
}
