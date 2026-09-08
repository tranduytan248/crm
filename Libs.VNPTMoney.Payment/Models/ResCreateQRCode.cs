using System.Collections.Generic;
using Libs.VNPTMoney.Payment.Utils;
using Newtonsoft.Json;

namespace Libs.VNPTMoney.Payment.Models
{
    public class ResCreateQRCode : ResBase
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
        /// Nội dung mã QRCode
        /// </summary>
        [JsonProperty("qrcodeData")]
        public string QrCodeData { get; set; }

        /// <summary>
        /// ID mã QRCode
        /// </summary>
        [JsonProperty("qrcodeId")]
        public string QrCodeId { get; set; }

        /// <summary>
        /// Tổng tiền thanh toán (totalAmount = originalAmount + fee)
        /// </summary>
        [JsonProperty("totalAmount")]
        public string TotalAmount   { get; set; }

        /// <summary>
        /// Số tiền thanh toán gốc ban đầu
        /// </summary>
        [JsonProperty("originalAmount")]
        public string OriginalAmount { get; set; }

        /// <summary>
        /// Số tiền phí thanh toán
        /// </summary>
        [JsonProperty("fee")]
        public string Fee { get; set; }

        /// <summary>
        /// Thời gian tạo QrCode. Định dạng yyyyMMddHHmmss
        /// </summary>
        [JsonProperty("createDate")]
        public string CreateDate { get; set; }

        /// <summary>
        /// signData = responseCode + "|" + description + "|" + qrcodeData + "|" + qrcodeId + "|" + totalAmount + "|" + originalAmount + "|" + fee + "|" + createDate + "|" + secretKey;
        /// checksum = sha256(signData)
        /// </summary>
        [JsonProperty("checksum")]
        public string Checksum { get; set; }

        /// <summary>
        /// Ảnh qrcode, định dạng text base64, định dạng ảnh png
        /// (data:image/png;base64,qrcodeImage)
        /// Nếu qrcodeImage empty thì sử dụng qrcodeData để sinh ảnh mã qrcode
        /// </summary>
        [JsonProperty("qrcodeImage")]
        public string QrCodeImage { get; set; }

        /// <summary>
        /// Thông tin chi tiết hóa đơn. Áp dụng cho qrCodeType = 02
        /// Object tương ứng với từng dịch vụ cụ thể
        /// </summary>
        [JsonProperty("billDetail")]
        public List<PaymentDetail> BillDetail { get; set; }

        public override bool IsValidChecksum(string secretKey)
        {
            var checksum= Encryptor.SHA256Hash(
                $"{ResponseCode}|{Description}|{QrCodeData}|{QrCodeId}|{TotalAmount}|{OriginalAmount}|{Fee}|{CreateDate}|{secretKey}");

            return checksum == Checksum;
        }
    }
}