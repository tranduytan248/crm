using System;

namespace CenIT.Libs.VNPTMoney.Payment.Major.Model
{
    public class QrCodeCheckInfoInvoiceModel
    {
        public string ContractCode { get; set; }
        public int InvNo { get; set; }
        public decimal? Amount { get; set; }
        public DateTime Peroid { get; set; }
        public int PaymentStatus { get; set; }
    }
}