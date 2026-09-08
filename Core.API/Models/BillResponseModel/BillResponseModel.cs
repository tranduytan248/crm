using System;

namespace Modules.API.Models.KhachHang
{
    public class BillResponseModel
    {
        public int BillId { get; set; }               // ID hóa đơn
        public int ContractId { get; set; }           // ID hợp đồng
        public string BillingPeriod { get; set; }     // "MM/yyyy"
        public decimal Amount { get; set; }           // Tổng số tiền
        public decimal ConsumedVolume { get; set; }   // Lượng nước tiêu thụ
        public decimal MeterEndIndex { get; set; }    // Chỉ số đồng hồ cuối kỳ
        public DateTime DueDate { get; set; }         // Hạn thanh toán
        public string Status { get; set; }            // "paid", "unpaid", "overdue"
    }
}