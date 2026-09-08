using Newtonsoft.Json;
using System;

namespace Modules.API.Models
{
    /// <summary>
    /// Model dùng cho api tạo mã Qr thanh toán
    /// </summary>
    public class ReqGenQrCodeModel
    {
        ///// <summary>
        ///// Customer Id
        ///// </summary>
        //[JsonProperty("CI")]
        //[JsonRequired]
        //public int CusId { get; set; }

        /// <summary>
        /// pay code
        /// </summary>
        [JsonProperty("IN")]
        [JsonRequired]
        public int InvNo { get; set; }

        /// <summary>
        /// Customer code
        /// </summary>
        [JsonProperty("CC")]
        [JsonRequired]
        public string ContractCode { get; set; }

        /// <summary>
        /// Danh sách phiếu cách bởi dấu phẩy (,)
        /// </summary>
        [JsonProperty("P")]
        [JsonRequired]
        public DateTime Period { get; set; }

        /// <summary>
        /// Tổng tiền thanh toán
        /// </summary>
        [JsonProperty("TA")]
        [JsonRequired]
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// Người Thanh Toán
        /// </summary>
        [JsonProperty("UP")]
        [JsonRequired]
        public string UserPay { get; set; }

        [JsonProperty("CS")]
        [JsonRequired]
        public string CheckSum { get; set; } = "";

    }
}