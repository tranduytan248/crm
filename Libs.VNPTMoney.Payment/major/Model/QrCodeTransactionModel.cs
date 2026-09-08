using System;
using System.Collections.Generic;
using System.Data;

namespace CenIT.Libs.VNPTMoney.Payment.Major.Model
{
    public class QrCodeTransactionModel : BaseModel
    {
        // Request properties
        public Guid TransactionId { get; set; }

        public string MerchantClientId { get; set; }

        public string MerchantName { get; set; }

        public string MerchantCode { get; set; }

        public string TerminalId { get; set; }

        public string CountryCode { get; set; }

        public string QrCodeType { get; set; }

        public string BillNumber { get; set; }

        public string TxnId { get; set; }

        public double Amount { get; set; }

        public string Ccy { get; set; }

        public string ConsumerId { get; set; }

        public string Purpose { get; set; }

        public string ProvinceCode { get; set; }

        public string IpAddress { get; set; }

        public bool IsBrowserRequest { get; set; }

        public string DeviceToken { get; set; }
        public string CreatedBy { get; set; }

        public DateTime RequestOn { get; set; }

        public List<QrCodePaymentDetailModel> LisPaymentDetails = new List<QrCodePaymentDetailModel>();

        public DataTable DataPaymentDetails { get; set; }

        // Response properties

        public string ResponseCode { get; set; }

        public string Description { get; set; }

        public string QrCodeData { get; set; }

        public string QrCodeId { get; set; }

        public string TotalAmount { get; set; }

        public string OriginalAmount { get; set; }

        public string Fee { get; set; }

        public string CreateDate { get; set; }

        public string QrCodeImage { get; set; }

        public bool IsSuccess { get; set; }

        public DateTime? ExpDate { get; set; }
    }
}