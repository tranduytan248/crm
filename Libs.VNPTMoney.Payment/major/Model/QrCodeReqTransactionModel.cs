using System;
using System.Collections.Generic;
using System.Data;

namespace CenIT.Libs.VNPTMoney.Payment.Major.Model
{
    public class QrCodeReqTransactionModel : BaseModel
    {
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

        public DateTime RequestOn { get; set; }

        public List<QrCodePaymentDetailModel> LisPaymentDetails = new List<QrCodePaymentDetailModel>();

        public DataTable DataPaymentDetails { get; set; }
    }
}