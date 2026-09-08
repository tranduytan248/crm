using System;
using System.Collections.Generic;
using System.ComponentModel;
using CenIT.Libs.VNPTMoney.Payment.Major.Biz;
using CenIT.Libs.VNPTMoney.Payment.Major.Model;
using Libs.VNPTMoney.Payment.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;
namespace CenIT.Libs.VNPTMoney.Payment.Major.Cache
{
    [DataObject]
    public class QrCodeTransactionCache : CacheLayer
    {
        private QrCodeTransactionBiz _transactionInfoApi;

        private QrCodeTransactionBiz Api => _transactionInfoApi ?? (_transactionInfoApi = new QrCodeTransactionBiz());

        protected override string[] MasterCacheKeyArray => new[] { "QrCodeTransactionsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public int? Add(QrCodeTransactionModel model)
        {
            var retAdd = Api.Add(model);
            if (retAdd > 0)
                // Invalidate the cache
                InvalidateCache();
            return retAdd;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public int? AddQrCodeInvoice(QrCodeTransactionModel model)
        {
            var retAdd = Api.AddQrCodeInvoice(model);
            if (retAdd > 0)
                // Invalidate the cache
                InvalidateCache();
            return retAdd;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public int? AddLogCallAPI(QrCodeReqCallAPILogModel model)
        {
            var retAdd = Api.AddLogCallAPI(model);
            return retAdd;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<QrCodeTransactionModel> Get(out int total, BaseSearchModel search = null)
        {
            var objectKey = UtilEncrypt.FromObject(search);
            var rawKey =
                $"ListTransactions-ViaSearch-{objectKey}";
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            //if (GetCacheItem(rawKey) is List<QrCodeTransactionModel> transactionInfos) return transactionInfos;
            var transactionInfos = GetCacheItem(rawKey) as List<QrCodeTransactionModel>;
            if(transactionInfos != null) return transactionInfos;
            // Item not found in cache - retrieve it and insert it into the cache
            transactionInfos = Api.Get(out total, search);
            AddCacheItem(rawKey, transactionInfos);
            AddCacheItem(rawKeyTotal, total);

            return transactionInfos;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public QrCodeTransactionModel GetById(Guid? transactionId)
        {
            if (transactionId == null) return null;

            var rawKey = string.Concat("TransactionById-", transactionId);

            // See if the item is in the cache
            //if (GetCacheItem(rawKey) is QrCodeTransactionModel transactionInfo) return transactionInfo;
            var transactionInfo = GetCacheItem(rawKey) as QrCodeTransactionModel;
            if (transactionInfo != null) return transactionInfo;

            // Item not found in cache - retrieve it and insert it into the cache
            transactionInfo = Api.GetById(transactionId);
            if (transactionInfo != null) AddCacheItem(rawKey, transactionInfo);

            return transactionInfo;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public PO_QrcodeGeneratedInvoiceModel GetByBillNumber(string billNumber)
        {
            //if (string.IsNullOrEmpty(billNumber)) return null;

            //var rawKey = string.Concat("TransactionByBillNumber-", billNumber);

            //// See if the item is in the cache
            ////if (GetCacheItem(rawKey) is QrCodeTransactionModel transactionInfo) return transactionInfo;
            //var transactionInfo = GetCacheItem(rawKey) as QrCodeTransactionModel;
            //if (transactionInfo != null) return transactionInfo;
            //// Item not found in cache - retrieve it and insert it into the cache
            var transactionInfo = Api.GetByBillNumber(billNumber);
            //if (transactionInfo != null) AddCacheItem(rawKey, transactionInfo);

            return transactionInfo;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<QrCodeTransactionModel> GetAll()
        {
            var rawKey = "AllTransactions";
            //if (GetCacheItem(rawKey) is List<QrCodeTransactionModel> lstTransactions) return lstTransactions;
            var lstTransactions = GetCacheItem(rawKey) as List<QrCodeTransactionModel>;
            if (lstTransactions != null) return lstTransactions;
            // Item not found in cache - retrieve it and insert it into the cache
            lstTransactions = Api.GetAll();
            AddCacheItem(rawKey, lstTransactions);

            return lstTransactions;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public int? AddResult(QrCodeTransactionResultModel model)
        {
            var retAdd = Api.AddResult(model);
            if (retAdd > 0)
                // Invalidate the cache
                InvalidateCache();
            return retAdd;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public int? AddResultPaidInvoice(string billNumber, string paydate)
        {
            var retAdd = Api.AddResultPaidInvoice(billNumber, paydate);
            if (retAdd > 0)
                // Invalidate the cache
                InvalidateCache();
            return retAdd;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public QrCodeTransactionResultModel GetResult(string billNumber)
        {
            var transactionResult = Api.GetResult(billNumber);
            return transactionResult;
        }
        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public int? CheckOrder(QrCodeCheckOrderModel model)
        {
            var retAdd = Api.CheckOrder(model);
            if (retAdd > 0)
                // Invalidate the cache
                InvalidateCache();
            return retAdd;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public int? ValidOrder(string billNumber)
        {
            var retAction = Api.ValidOrder(billNumber);
            return retAction;
        }
        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Invalid(Guid? transactionId)
        {
            var retAction = Api.Invalid(transactionId);
            if (retAction > 0)
                // Invalidate the cache
                InvalidateCache();
            return retAction;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public QrCodeCheckInfoInvoiceModel ValidInfoInvoice(string ContractCode, int InvNo, DateTime Peroid, decimal? TotalAmount)
        {
            var retAction = Api.ValidInfoInvoice(ContractCode, InvNo, Peroid, TotalAmount);
            //if (retAction > 0)
            //    // Invalidate the cache
            //    InvalidateCache();
            return retAction;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int CheckQRCodeIsUsed(string billNumber)
        {
            var retAction = Api.CheckQRCodeIsUsed(billNumber);
            //if (retAction > 0billNumber
            //    // Invalidate the cache
            //    InvalidateCache();
            return retAction;
        }

        // Lấy danh sách QRCode đã gender
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public ResCreateQRCode GetQRCodeByBillNumber(string billNumber)
        {
            var dataQRCode = Api.GetQRCodeByBillNumber(billNumber);
            return dataQRCode;
        }
        //
        //    [DataObjectMethod(DataObjectMethodType.Update, false)]
        //    public int? LockReceipts(Guid? transactionId)
        //    {
        //        var retAction = Api.LockReceipts(transactionId);
        //        if (retAction > 0)
        //            // Invalidate the cache
        //            InvalidateCache();
        //        return retAction;
        //    }

        //    [DataObjectMethod(DataObjectMethodType.Update, false)]
        //    public int? Cancel(Guid? transactionId)
        //    {
        //        var retAction = Api.Cancel(transactionId);
        //        if (retAction > 0)
        //            // Invalidate the cache
        //            InvalidateCache();
        //        return retAction;
        //    }

        //    [DataObjectMethod(DataObjectMethodType.Select, false)]
        //    public int? ValidOrder(string billNumber)
        //    {
        //        var retAction = Api.ValidOrder(billNumber);
        //        return retAction;
        //    }
    }
}