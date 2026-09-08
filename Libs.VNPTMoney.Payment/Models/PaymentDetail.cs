using Newtonsoft.Json;

namespace Libs.VNPTMoney.Payment.Models
{
    public class PaymentDetail
    {
        /// <summary>
        /// Loại khoản mục
        /// </summary>
        [JsonProperty("itemType")]
        public string ItemType { get; set; }

        /// <summary>
        /// Mã khoản mục
        /// </summary>
        [JsonProperty("itemCode")]
        public string ItemCode { get; set; }

        /// <summary>
        /// Tên khoản mục
        /// </summary>
        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        /// <summary>
        /// Số tiền từng khoản mục
        /// </summary>
        [JsonProperty("itemAmount")]
        public string ItemAmount { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        [JsonProperty("itemDescription")]
        public string ItemDescription { get; set; }
    }
}