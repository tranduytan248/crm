using System;

namespace CenIT.Libs.VNPTMoney.Payment.Major.Model
{
    public class PO_QrcodeGeneratedInvoiceModel
    {
        public long QrcodeGeneratedInvoiceId { get; set; }

        public string BillNumber { get; set; } = "";

        public string QrCodeData { get; set; }

        public string QrCodeId { get; set; }

        public string TotalAmount { get; set; }

        public string OriginalAmount { get; set; }

        public string Fee { get; set; }

        public string QrCodeImage { get; set; }

        public bool? IsSuccess { get; set; }

        public DateTime? ExpDate { get; set; }

        public bool? IsCancel { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }
    }
}