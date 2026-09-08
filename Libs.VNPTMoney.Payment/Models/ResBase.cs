using Newtonsoft.Json;

namespace Libs.VNPTMoney.Payment.Models
{
    public class ResBase
    {
        /// <summary>
        /// Mã lỗi, tham khảo
        /// </summary>
        [JsonProperty("responseCode")]
        public string ResponseCode { get; set; }

        /// <summary>
        /// Mô tả lỗi
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        public virtual bool IsValidChecksum(string secretKey)
        {
            return true;
        }
    }
}