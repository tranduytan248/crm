using Libs.VNPTMoney.Payment.Utils;
using Newtonsoft.Json;

namespace Libs.VNPTMoney.Payment.Models
{
    public class ReqNotifyPayment: ReqBase
    {
        /// <summary>
        /// Kết quả thực hiện giao dịch, tham khảo Bảng mã lỗi (2)
        /// </summary>
        [JsonProperty("responseCode")]
        public string ResponseCode { get; set; }

        /// <summary>
        /// Mô tả mã lỗi
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        ///// <summary>
        ///// Định danh tài khoản kết nối API của merchant, do VNPTPAY cấp
        ///// </summary>
        //[JsonProperty("merchantClientId")]
        //public string MerchantClientId { get; set; }

        ///// <summary>
        ///// Mã merchant, do VNPTPAY cấp
        ///// </summary>
        //[JsonProperty("merchantCode")]
        //public string MerchantCode { get; set; }

        ///// <summary>
        ///// Mã điểm bán, do VNPT cấp
        ///// </summary>
        //[JsonProperty("terminalId")]
        //public string TerminalId { get; set; }

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
        /// Loại hình thanh toán, giá trị mặc định: 1
        /// </summary>
        [JsonProperty("msgType")]
        public string MsgType { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        /// <summary>
        /// Số tài khoản
        /// </summary>
        [JsonProperty("accountNo")]
        public string AccountNo { get; set; }

        /// <summary>
        /// Số điện thoại khách hàng
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// </summary>
        [JsonProperty("amount")]
        public int? Amount { get; set; }

        /// <summary>
        /// Mã tiền tệ
        /// </summary>
        [JsonProperty("ccy")]
        public string Ccy { get; set; }

        /// <summary>
        /// Loại QRCode
        /// </summary>
        [JsonProperty("qrcodeType")]
        public string QrCodeType { get; set; }

        /// <summary>
        /// Mã giao dịch phía VNPTPAY
        /// </summary>
        [JsonProperty("qrTxnId")]
        public string QrTxnId { get; set; }

        /// <summary>
        /// Mã phương thức thanh toán
        /// </summary>
        [JsonProperty("paymentMethod")]
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Thời gian thanh toán
        /// </summary>
        [JsonProperty("payDate")]
        public string PayDate { get; set; }

        /// <summary>
        /// Thông tin bổ sung
        /// </summary>
        [JsonProperty("additionalInfo")]
        public string AdditionalInfo { get; set; }

        /// <summary>
        /// Thông tin IPN bổ sung
        /// </summary>
        [JsonProperty("extraData")]
        public string ExtraData { get; set; }

        /// <summary>
        /// signData = responseCode + "|" + description + "|" + merchantClientId + "|" + merchantCode + "|" + terminalId + "|" + billNumber + "|" + txnId + "|" + msgType + "|" + customerName + "|" + accountNo + "|" + mobile + "|" + amount + "|" + ccy + "|" + qrCodeType + "|" + qrTxnId + "|" + paymentMethod + "|" + payDate + "|" + additionalInfo + "|" + secretKey;
        /// checksum = sha256(signData)
        /// </summary>
        [JsonProperty("checksum")]
        public string Checksum =>
            Encryptor.SHA256Hash($"{ResponseCode}|{Description}|{MerchantClientId}|{MerchantCode}|{TerminalId}|{BillNumber}|{TxnId}|{MsgType}|{CustomerName}|{AccountNo}|{Mobile}|{Amount}|{Ccy}|{QrCodeType}|{QrTxnId}|{PaymentMethod}|{PayDate}|{AdditionalInfo}|{SecretKey}");
    }
}