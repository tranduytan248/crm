using System;

namespace CenIT.Libs.VNPTMoney.Payment.Major.Model
{
    public class QrCodeResTransactionModel : BaseModel
    {
        public Guid TransactionId { get; set; }

        public string ResponseCode { get; set; }

        public string Description { get; set; }

        public string QrCodeData { get; set; }

        public string QrCodeId { get; set; }

        public string TotalAmount { get; set; }

        public string OriginalAmount { get; set; }

        public string Fee { get; set; }

        public string CreateDate { get; set; }

        public string QrCodeImage { get; set; }

        public string ConsumerId { get; set; }

        public bool IsSuccess { get; set; }

        public DateTime ExpDate { get; set; }
    }
}