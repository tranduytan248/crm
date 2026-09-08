using System;
using System.Collections.Generic;
using Libs.VNPTMoney.Payment.Utils;
using Newtonsoft.Json;

namespace Libs.VNPTMoney.Payment.Models
{
    public class ReqCreateQRCode : ReqBase
    {
        ///// <summary>
        ///// Mã merchant, do VNPTPAY cấp
        ///// </summary>
        //[JsonProperty("merchantCode")]
        //public string MerchantCode { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// Bắt buộc đối với QR động
        /// </summary>
        [JsonProperty("amount")]
        [JsonRequired]
        public string Amount { get; set; }

        /// <summary>
        /// Nội dung thanh toán, Dùng tiếng việt không dấu và không chứa ký tự đặc biệt (Hiển thị trên ứng dụng chuyển tiền)
        /// </summary>
        [JsonProperty("purpose")]
        public string Purpose { get; set; }

        /// <summary>
        /// Mã khách hàng, SĐT của khách hàng dùng để truy vấn/gạch nợ hóa đơn
        /// Bắt buộc đối với QrType = 02
        /// </summary>
        [JsonProperty("consumerId")]
        public string ConsumerId { get; set; }

        /// <summary>
        /// Loại QR
        /// 01: QR cho các điểm Offline
        /// 02: QR thanh toán Hóa đơn
        /// </summary>
        [JsonProperty("qrCodeType")]
        [JsonRequired]
        public string QrCodeType { get; set; } = "01"; // Default is QR_CODE_TYPE_OFFLINE

        /// <summary>
        /// Thông tin bổ sung
        /// </summary>
        [JsonProperty("addInfo")]
        public string AddInfo { get; set; }

        /// <summary>
        /// 1. Nội dung sau chỉ áp dụng cho Merchant yêu cầu thêm thông tin chi tiết trong đơn hàng:
        ///     KEY_01=VALUE_01||KEY_02=VALUE_02||KEY_03=VALUE_03
        ///     Trong đó:
        ///         1. Ngăn cách các thông tin với nhau bởi "||"
        ///         2. "KEY_n" là cố định, do VNPTMoney quy định, cung cấp cho đối tác, ví dụ: CUST_ID, CUST_NAME, CUST_ADDR, ...
        ///         3. "VALUE_n" là giá trị bên đối tác tương ứng với KEY_n
        ///     Ví dụ: CUST_ID=123456||CUST_NAME=NGUYEN VAN A||CUST_ADDR=HNI
        ///     Chú ý: Thông tin này chỉ bổ sung trong báo cáo, không hiển thị trên giao diện thanh toán
        ///     Các key khả dụng:
        ///         - CUST_ID	Mã khách hàng
        ///         - CUST_NAME	Tên khách hàng
        ///         - CUST_ADDR	Địa chỉ khách hàng
        /// 2. Đối với Merchant mong muốn nhận thêm thông tin trả về trong luồng notifyPayment:
        ///     merchantData = Base64_Encode ({"required_extra_data_ipn":"true"})
        ///     Dữ liệu trả về trong trường extraData trong luồng notifyPayment:
        ///     Ví dụ: extraData = Base64_Encode
        ///     ({"bankCode":"VIETINBANK", "maCong":"5", "thuHo":"2", "maDoiTac":"970415", "maDiemGd":""}) 
        /// </summary>
        [JsonProperty("merchantData")]
        public string MerchantData { get; set; }

        ///// <summary>
        ///// 	Mã điểm bán, do VNPT cấp
        ///// </summary>
        //[JsonProperty("terminalId")]
        //public string TerminalId { get; set; }

        /// <summary>
        /// Tên viêt tắt của Terminal, do VNPTPAY cấp
        /// </summary>
        [JsonProperty("terminalName")]
        public string TerminalName { get; set; }

        /// <summary>
        /// Loại hình doanh nghiệp. Giá trị mặc định để "0"
        /// </summary>
        [JsonProperty("mcc")]
        public string Mcc { get; set; } = "0";

        /// <summary>
        /// Thời gian hết hạn thanh toán, định dạng: yyMMddHHmm
        /// </summary>
        [JsonProperty("expDate")]
        public string ExpDate { get; set; } = "9999-12-31"; // Default expiration date

        ///// <summary>
        ///// Tên viêt tắt của Merchant, do VNPTPAY cấp
        ///// </summary>
        //[JsonProperty("merchantName")]
        //public string MerchantName { get; set; }

        ///// <summary>
        ///// Định danh tài khoản kết nối API của merchant, do VNPTPAY cấp
        ///// </summary>
        //[JsonProperty("merchantClientId")]
        //public string MerchantClientId { get; set; }

        /// <summary>
        /// Tiền tip and fee. Giá trị mặc định để empty
        /// </summary>
        [JsonProperty("tipAndFee")]
        public string TipAndFee { get; set; } = String.Empty; // Default tip and fee is 0

        /// <summary>
        /// Mã quốc gia: default VN
        /// </summary>
        [JsonProperty("countryCode")]
        [JsonRequired]
        public string CountryCode { get; set; } = "VN";

        /// <summary>
        /// Mã tiền tệ : Giá trị mặc định 704
        /// </summary>
        [JsonProperty("ccy")]
        [JsonRequired]
        public string Ccy { get; set; } = "704"; // Default currency is VND

        /// <summary>
        /// signData = merchantClientId + "|" + merchantName+ "|" + countryCode+ "|" + merchantCode+ "|" + terminalId + "|" + qrCodeType+ "|" + txnId+ "|" + billNumber+ "|" + amount+ "|" + ccy+ "|" + expDate+ "|" + mcc+ "|" + tipAndFee+ "|" + consumerId+ "|" + purpose+ "|" + secretKey;
        /// checksum = sha256(signData)
        /// </summary>
        [JsonProperty("checksum")]
        [JsonRequired]
        public string Checksum =>
            Encryptor.SHA256Hash($"{MerchantClientId}|{MerchantName}|{CountryCode}|{MerchantCode}|{TerminalId}|{QrCodeType}|{TxnId}|{BillNumber}|{Amount}|{Ccy}|{ExpDate}|{Mcc}|{TipAndFee}|{ConsumerId}|{Purpose}|{SecretKey}");

        /// <summary>
        /// Số hóa đơn, biên lai
        /// Bắt buộc đối với QR động(duy nhất mỗi lần tạo mã QR )
        /// </summary>
        [JsonProperty("billNumber")]
        public string BillNumber { get; set; }

        /// <summary>
        /// Mã giao dịch
        /// </summary>
        [JsonProperty("txnId")]
        public string TxnId { get; set; } = ""; // Default is empty

        /// <summary>
        /// Mã gói cước (chỉ dùng cho các đơn vị cung cấp dịch vụ bán gói)
        /// </summary>
        [JsonProperty("packageCode")]
        public string PackageCode { get; set; }

        /// <summary>
        /// Mã tỉnh thành (dạng HNI,HCM,BDG,...)
        /// </summary>
        [JsonProperty("provinceCode")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// Thông tin chi tiết các khoản mục
        /// Chú ý: Số tiền thanh toán bằng tổng số tiền các khoản mục
        /// </summary>
        [JsonProperty("paymentDetail")]
        public List<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();

        //[JsonIgnore]
        //public string IpRequest { get; set; }
    }
}