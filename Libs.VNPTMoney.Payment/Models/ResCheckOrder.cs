using Libs.VNPTMoney.Payment.Utils;
using Newtonsoft.Json;

namespace Libs.VNPTMoney.Payment.Models
{
    public class ResCheckOrder : ResBase
    {
        ///// <summary>
        ///// Mã lỗi, tham khảo Bảng mã lỗi (4)
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
        /// Mã giao dịch/mã hóa đơn do merchant sinh ra (qrcode động)
        /// </summary>
        [JsonProperty("billNumber")]
        public string BillNumber { get; set; }

        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        [JsonProperty("txnId")]
        public string TxnId { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// </summary>
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("customerCode")] public string CustomerCode { get; set; }

        [JsonProperty("customerName")] public string CustomerName { get; set; }

        [JsonProperty("customerAddress")] public string CustomerAddress { get; set; }

        [JsonProperty("customerPhone")] public string CustomerPhone { get; set; }

        [JsonProperty("paymentCode")] public string PaymentCode { get; set; }

        [JsonProperty("createDate")] public string CreateDate { get; set; }

        /// <summary>
        /// signData = responseCode + "|" + description + "|" + merchantClientId + "|" + merchantCode+ "|" + terminalId+ "|" + billNumber + "|" + txnId+ "|" + amount+ "|" + createDate+ "|" + secretKey;
        /// checksum = sha256(signData)
        /// </summary>
        [JsonProperty("checksum")]
        public string Checksum { get; set; }
        
        [JsonIgnore]
        
        public string SecretKey { get; set; }
        public override bool IsValidChecksum(string secretKey)
        {
            var signData =
                $"{ResponseCode}|{Description}|{MerchantClientId}|{MerchantCode}|{TerminalId}|{BillNumber}|{TxnId}|{Amount}|{CreateDate}|{secretKey}";
            return Encryptor.SHA256Hash(signData) == Checksum;
        }
    }
}