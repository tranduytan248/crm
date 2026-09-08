using Libs.VNPTMoney.Payment.Utils;
using Newtonsoft.Json;

namespace Libs.VNPTMoney.Payment.Models
{
    public class ReqQueryTransactionBill : ReqBase
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
        /// Số hóa đơn, biên lai
        /// </summary>
        [JsonProperty("billNumber")]
        public string BillNumber { get; set; }

        /// <summary>
        /// signData = merchantClientId + "|" + merchantCode+ "|" + terminalId+ "|" + billNumber+ "|" + secretKey;
        /// checksum = sha256(signData)
        /// </summary>
        [JsonProperty("checksum")]
        public string Checksum =>
            Encryptor.SHA256Hash($"{MerchantClientId}|{MerchantCode}|{TerminalId}|{BillNumber}|{SecretKey}");
    }
}