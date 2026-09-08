using System;

namespace CenIT.Libs.VNPTMoney.Payment.Major.Model
{
    public class QrCodeCheckOrderModel : BaseModel
    {
        public Guid TransactionId { get; set; }

        public string BillNumber { get; set; }

        public string IpAddress { get; set; }

        public DateTime RequestOn { get; set; }
    }
}