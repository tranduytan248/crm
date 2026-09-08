using Libs.VNPTMoney.Payment.Utils;
using Newtonsoft.Json;

namespace Libs.VNPTMoney.Payment.Models
{
    public class ResQueryTransactionBill:ResBase
    {
        ///// <summary>
        ///// Mã lỗi, tham khảo
        ///// </summary>
        //[JsonProperty("responseCode")]
        //public string ResponseCode { get; set; }

        ///// <summary>
        ///// Mô tả lỗi
        ///// </summary>
        //[JsonProperty("description")]
        //public string Description { get; set; }

        /// <summary>
        /// Định danh tài khoản kết nối API của merchant, do VNPTPAY cấp
        /// </summary>
        [JsonProperty("merchantClientId")]
        public string MerchantClientId { get; set; }

        /// <summary>
        /// Mã merchant, do VNPTPAY cấp
        /// </summary>
        [JsonProperty("merchantCode")]
        public string MerchantCode { get; set; }

        /// <summary>
        /// Mã điểm bán, do VNPT cấp
        /// </summary>
        [JsonProperty("terminalId")]
        public string TerminalId { get; set; }

        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        [JsonProperty("txnId")]
        public string TxnId { get; set; }

        /// <summary>
        /// Số hóa đơn, biên lai khi merchant create_qrcode
        /// </summary>
        [JsonProperty("billNumber")]
        public string BillNumber { get; set; }

        /// <summary>
        /// Số tiền thanh toán trước khi Khuyến mại
        /// </summary>
        [JsonProperty("debitAmount")]
        public int? DebitAmount { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// </summary>
        [JsonProperty("amount")]
        public int? Amount { get; set; }

        /// <summary>
        /// Loại QR
        /// </summary>
        [JsonProperty("qrCodeType")]
        public string QrCodeType { get; set; }

        /// <summary>
        /// Mã giao dịch trên ứng dụng quét QRCode
        /// </summary>
        [JsonProperty("orderCode")]
        public string OrderCode { get; set; }

        /// <summary>
        /// Mã giao dịch QRCode trên hệ thống PT PAY
        /// </summary>
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Trạng thái giao dịch ( Danh sách Trạng thái giao dịch )
        /// </summary>
        [JsonProperty("transactionStatus")]
        public int? TransactionStatus { get; set; }

        /// <summary>
        /// Số điện thoại khách hàng
        /// </summary>
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        /// <summary>
        /// Thời gian thanh toán. Định dạng yyyyMMddHHmmss
        /// </summary>
        [JsonProperty("payDate")]
        public string PayDate { get; set; }

        /// <summary>
        /// signData = responseCode + "|" + description + "|" + merchantClientId + "|" + merchantCode+ "|" + terminalId+ "|" + txnId+ "|" + billNumber + "|" + debitAmount+ "|" + amount + "|" + qrCodeType + "|" + orderCode+ "|" + transactionId+ "|" + transactionStatus+ "|" + phoneNumber+ "|" + customerName+ "|" + payDate+ "|" + secretKey;
        /// checksum = sha256(signData)
        /// </summary>
        [JsonProperty("checksum")]
        public string Checksum { get; set; }

        public override bool IsValidChecksum(string secretKey)
        {
            var checksum = Encryptor.SHA256Hash($"{ResponseCode}|{Description}|{MerchantClientId}|{MerchantCode}|{TerminalId}|{TxnId}|{BillNumber}|{DebitAmount}|{Amount}|{QrCodeType}|{OrderCode}|{TransactionId}|{TransactionStatus}|{PhoneNumber}|{CustomerName}|{PayDate}|{secretKey}");
            return checksum == Checksum;
        }
    }
}