using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Util;
using CenIT.Libs.VNPTMoney.Payment.Major.Model;
using Libs.VNPTMoney.Payment.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Providers;

namespace CenIT.Libs.VNPTMoney.Payment.Major.Biz
{
    public class QrCodeTransactionBiz
    {
        //private const string DATA_PROVIDER_NAME = "UrencoPOProvider";

        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";

        private readonly string _poQrCodeTransactionsAdd = "PO_QrCode_Transactions_Add";
        private readonly string _poQrCodeTransactionsAddResult = "PO_QrCode_Transactions_AddResult";
        private readonly string _poQrCodeTransactionsGet = "PO_QrCode_Transactions_Get";
        private readonly string _poQrCodeTransactionsGetByBillNumber = "PO_QrCode_Transactions_GetByBillNumber";
        private readonly string _poQrCodeTransactionsGetById = "PO_QrCode_Transactions_GetById";
        private readonly string _poQrCodeTransactionsLockReceipts = "PO_QrCode_Transactions_LockReceipts";
        private readonly string _poQrCodeTransactionsCancel = "PO_QrCode_Transactions_Cancel";
        private readonly string _poQrCodeTransactionsValidOrder = "PO_QrCode_Transactions_ValidOrder";
        private readonly string _poQrCodeCheckOrdersAdd = "PO_QrCode_CheckOrders_Add";
        private readonly string _poQrCodeTransactionsInvalid = "PO_QrCode_Transactions_Invalid";
        private readonly string _poQrCodeTransactionsGetResult = "PO_QrCode_Transactions_GetResult";
        private readonly string _poPOQrCodeValidInfoInvoice = "PO_QrCode_ValidInfoInvoice";
        private readonly string _poPOQrCodeCheckQRCodeIsUsed = "PO_QrCode_CheckQRCodeIsUsed";
        private readonly string _poPOQrCodeGetQRCodeByBillNumber = "PO_Qrcode_GetQRCodeByBillNumber";
        private readonly string _poPOQrcodeGeneratedInvoiceAdd = "PO_Qrcode_GeneratedInvoice_Add";
        private readonly string _poPOQrCodePaidInvoicesSave = "PO_QrCode_PaidInvoices_Save";
        private readonly string _poPOQrCodeLogCallAPISave = "PO_QrCode_LogCallAPI_Save";



        public int? Add(QrCodeTransactionModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_poQrCodeTransactionsAdd, DATA_PROVIDER_NAME,
                model.TransactionId,
                model.MerchantClientId,
                model.MerchantName,
                model.MerchantCode,
                model.TerminalId,
                model.CountryCode,
                model.QrCodeType,
                model.BillNumber,
                model.TxnId,
                model.Amount,
                model.Ccy,
                model.ConsumerId,
                model.Purpose,
                model.ProvinceCode,
                model.IpAddress,
                model.DataPaymentDetails,
                model.IsBrowserRequest,
                model.DeviceToken,
                // Response parameters
                model.ResponseCode,
                model.Description,
                model.QrCodeData,
                model.QrCodeId,
                model.TotalAmount,
                model.OriginalAmount,
                model.Fee,
                model.CreateDate,
                model.QrCodeImage,
                model.IsSuccess,
                model.ExpDate);

            return result;
        }
        public int? AddQrCodeInvoice(QrCodeTransactionModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_poPOQrcodeGeneratedInvoiceAdd, DATA_PROVIDER_NAME,              
                model.BillNumber,
                model.QrCodeData,
                model.QrCodeId,
                model.TotalAmount,
                model.OriginalAmount,
                model.Fee,
                model.QrCodeImage,
                model.IsSuccess,
                model.ExpDate,
                0,
                model.CreateDate,
                model.CreatedBy
                );
            return result;
        }
        public List<QrCodeTransactionModel> Get(out int total, BaseSearchModel search)
        {
           // List<QrCodeTransactionModel> lstTransactions = new List<QrCodeTransactionModel>();
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<QrCodeTransactionModel>(_poQrCodeTransactionsGet, DATA_PROVIDER_NAME,
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

        public QrCodeTransactionModel GetById(Guid? transactionId)
        {

            var dataTransaction = AppProcessor.ProcedureProvider.ExecuteScalarObject<QrCodeTransactionModel>(_poQrCodeTransactionsGetById, DATA_PROVIDER_NAME, transactionId);
            return dataTransaction;
            //if (dataTransaction == null || dataTransaction.Rows.Count <= 0) return null;
            //var rowTransaction = dataTransaction.Rows[0];
            //if (rowTransaction == null) return null;
            //var transactionModel = ModelProvider.CreateModelFromRow<QrCodeTransactionModel>(rowTransaction);
            //return transactionModel;
        }

        public PO_QrcodeGeneratedInvoiceModel GetByBillNumber(string billNumber)
        {
            var dataTransaction = AppProcessor.ProcedureProvider.ExecuteScalarObject<PO_QrcodeGeneratedInvoiceModel>(_poQrCodeTransactionsGetByBillNumber, DATA_PROVIDER_NAME, billNumber);
            return dataTransaction;
        }

        public List<QrCodeTransactionModel> GetAll()
        {
            int iTotal;
            return Get(out iTotal, null);
        }

        public int? AddResult(QrCodeTransactionResultModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_poQrCodeTransactionsAddResult, DATA_PROVIDER_NAME,
                model.TransactionId,
                model.ResponseCode,
                model.Description,
                model.MerchantClientId,
                model.MerchantName,
                model.MerchantCode,
                model.TerminalId,
                model.Amount,
                model.BillNumber,
                model.TxnId,
                model.MsgType,
                model.CustomerName,
                model.AccountNo,
                model.Mobile,
                model.Ccy,
                model.QrTxnId,
                model.QrCodeType,
                model.PaymentMethod,
                model.PayDate,
                model.AdditionalInfo,
                model.ExtraData,
                model.IpAddress,
                model.IsSuccess);
            return result;
        }
        public int? AddResultPaidInvoice(string billNumber, string paydate)
        {

            var result = AppProcessor.ProcedureProvider.Execute(_poPOQrCodePaidInvoicesSave, DATA_PROVIDER_NAME, paydate,
                null, billNumber
                );
            return result;
        }
        public QrCodeTransactionResultModel GetResult(string billNumber)
        {
            var dataTransaction = AppProcessor.ProcedureProvider.ExecuteScalarObject<QrCodeTransactionResultModel>(_poQrCodeTransactionsGetResult, DATA_PROVIDER_NAME, billNumber);
            return dataTransaction;        
        }

        public int? CheckOrder(QrCodeCheckOrderModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_poQrCodeCheckOrdersAdd, DATA_PROVIDER_NAME,
                model.BillNumber,
                model.IpAddress);

            return result;
        }
        public int? ValidOrder(string billNumber)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_poQrCodeTransactionsValidOrder, DATA_PROVIDER_NAME,
                billNumber);
            return result;
        }

        public int? Invalid(Guid? transactionId)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_poQrCodeTransactionsInvalid, DATA_PROVIDER_NAME,
                transactionId);
            return result;
        }
        // Kiểm tra thông tin đầu vào có map trung databasa hay k
        public QrCodeCheckInfoInvoiceModel ValidInfoInvoice(string ContractCode, int InvNo, DateTime Peroid, decimal? TotalAmount)
        {
            var result = AppProcessor.ProcedureProvider.ExecuteScalarObject<QrCodeCheckInfoInvoiceModel>(_poPOQrCodeValidInfoInvoice, DATA_PROVIDER_NAME,
                ContractCode, InvNo, Peroid, TotalAmount);
            return result;
        }
        //  1. Kiểm tra xem có thông tin này trong bảng hóa đơn đã thanh toán
        public int CheckQRCodeIsUsed(string billNumber)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_poPOQrCodeCheckQRCodeIsUsed, DATA_PROVIDER_NAME,
                billNumber);
            return result.GetValueOrDefault(0);
        }

        public ResCreateQRCode GetQRCodeByBillNumber(string billNumber)
        {
            var dataQRCode = AppProcessor.ProcedureProvider.ExecuteScalarObject<ResCreateQRCode>(_poPOQrCodeGetQRCodeByBillNumber, DATA_PROVIDER_NAME, billNumber);
            return dataQRCode;
        }

        public int? AddLogCallAPI(QrCodeReqCallAPILogModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_poPOQrCodeLogCallAPISave, DATA_PROVIDER_NAME,
                model.APINameOrURL, model.Request, model.Response);
            return result;
        }

    }
}