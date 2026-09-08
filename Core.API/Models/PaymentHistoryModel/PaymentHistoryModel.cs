using System;

namespace Modules.API.Models.NewsKhachHang
{
    public class PaymentHistoryModel
    {
       public string billingPeriod { get; set; }
       public string paymentDate { get; set; }
       public string usageFrom { get; set; }
       public string usageTo { get; set; }
       public decimal meterEndIndex { get; set; }
       public decimal consumedWater { get; set; }
       public decimal amountDue { get; set; }
       public decimal debtAmount { get; set; }
       public string paymentStatus { get; set; }
    }
}
