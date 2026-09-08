using System;

namespace CenIT.Libs.VNPTMoney.Payment.Major.Model
{
    public class QrCodePaymentDetailModel : BaseModel
    {
        public Guid TransactionId { get; set; }

        public string ItemType { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public string ItemAmount { get; set; }

        public string ItemDescription { get; set; }
    }
}