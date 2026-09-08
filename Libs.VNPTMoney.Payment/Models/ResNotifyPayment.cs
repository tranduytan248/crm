using Newtonsoft.Json;

namespace Libs.VNPTMoney.Payment.Models
{
    public class ResNotifyPayment : ResBase
    {
        ///// <summary>
        ///// Mã lỗi, tham khảo Bảng mã lỗi (3)
        ///// </summary>
        //[JsonProperty("responseCode")]
        //public string ResponseCode { get; set; }

        //[JsonProperty("description")]
        //public string Description { get; set; }

        /// <summary>
        /// Thông tin bổ sung, dạng JSON
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }
    }
}