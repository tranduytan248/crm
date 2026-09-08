using Libs.VNPTMoney.Payment.Utils;
using Newtonsoft.Json;

namespace Libs.VNPTMoney.Payment.Models
{
    public class ReqCheckOrder : ReqBase
    {
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
        /// Tài khoản thanh toán
        /// </summary>
        [JsonProperty("virtualAccount")]
        public string VirtualAccount { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// </summary>
        [JsonProperty("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Thời gian thanh toán
        /// </summary>
        [JsonProperty("createDate")]
        public string CreateDate { get; set; }
        
        /// <summary>
        /// signData = merchantClientId + "|" + merchantCode + "|" + terminalId + "|" + billNumber + "|" + txnId + "|" + virtualAccount + "|" + amount + "|" + createDate + "|" + secretKey;
        /// checksum = sha256(signData)
        /// </summary>
        [JsonProperty("checksum")]
        public string Checksum => 
            Encryptor.SHA256Hash($"{MerchantClientId}|{MerchantCode}|{TerminalId}|{BillNumber}|{TxnId}|{VirtualAccount}|{Amount}|{CreateDate}|{SecretKey}");
    }
}