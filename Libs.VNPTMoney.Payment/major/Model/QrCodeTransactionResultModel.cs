using System;

namespace CenIT.Libs.VNPTMoney.Payment.Major.Model
{
    public class QrCodeTransactionResultModel
    {
        public Guid TransactionId { get; set; }

        public string ResponseCode { get; set; }

        public string Description { get; set; }

        public string MerchantClientId { get; set; }

        public string MerchantName { get; set; }

        public string MerchantCode { get; set; }

        public string TerminalId { get; set; }

        public double Amount { get; set; }

        public string BillNumber { get; set; }

        public string TxnId { get; set; }

        public string MsgType { get; set; }

        public string CustomerName { get; set; }

        public string AccountNo { get; set; }

        public string Mobile { get; set; }

        public string Ccy { get; set; }

        public string QrTxnId { get; set; }

        public string QrCodeType { get; set; }

        public string PaymentMethod { get; set; }

        public string PayDate { get; set; }

        public string AdditionalInfo { get; set; }

        public string ExtraData { get; set; }

        public string IpAddress { get; set; }

        public DateTime RequestOn { get; set; }

        public bool IsSuccess { get; set; }
    }
}