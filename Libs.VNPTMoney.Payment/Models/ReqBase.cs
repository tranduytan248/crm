using Newtonsoft.Json;

namespace Libs.VNPTMoney.Payment.Models
{
    public class ReqBase
    {
        /// <summary>
        /// Định danh tài khoản kết nối API của merchant, do VNPTPAY cấp
        /// </summary>
        [JsonProperty("merchantClientId")]
        [JsonRequired]
        public string MerchantClientId { get; set; }

        /// <summary>
        /// Tên viêt tắt của Merchant, do VNPTPAY cấp
        /// </summary>
        [JsonProperty("merchantName")]
        //[JsonRequired]
        public string MerchantName { get; set; }

        /// <summary>
        /// Mã merchant, do VNPTPAY cấp
        /// </summary>
        [JsonProperty("merchantCode")]
        [JsonRequired]
        public string MerchantCode { get; set; }

        /// <summary>
        /// Mã điểm bán, do VNPT cấp
        /// </summary>
        [JsonProperty("terminalId")]
        [JsonRequired]
        public string TerminalId { get; set; }

        [JsonIgnore]
        public string SecretKey { get; set; }
    }
}