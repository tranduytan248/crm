using System.Collections.Generic;

namespace Libs.VNPTMoney.Payment.Consts
{
    public class VNPTMoneyQRPaymentConst
    {
        public class EnumVNPTPayResponseCode
        {
            /// <summary>
            /// Thành công
            /// </summary>
            public const string _00 = "00"; // Thành công
            /// <summary>
            /// Giao dịch thất bại
            /// </summary>
            public const string _01 = "01"; // Giao dịch thất bại
            /// <summary>
            /// Dữ liệu không đúng định dạng
            /// </summary>
            public const string _02 = "02"; // Dữ liệu không đúng định dạng
            /// <summary>
            /// Không tìm thấy dữ liệu
            /// </summary>
            public const string _05 = "05"; // Không tìm thấy dữ liệu
            /// <summary>
            /// Lỗi hệ thống
            /// </summary>
            public const string _06 = "06"; // Lỗi hệ thống
            /// <summary>
            /// Chữ ký không đúng
            /// </summary>
            public const string _07 = "07"; // Chữ ký không đúng
            /// <summary>
            /// Tài khoản kết nối của merchant đang bị khóa
            /// </summary>
            public const string _08 = "08"; // Tài khoản kết nối của merchant đang bị khóa
            /// <summary>
            /// Tài khoản kết nối của merchant không tồn tại
            /// </summary>
            public const string _09 = "09"; // Tài khoản kết nối của merchant không tồn tại
            /// <summary>
            /// Hệ thống đang bảo trì
            /// </summary>
            public const string _96 = "96"; // Hệ thống đang bảo trì
        }

        public static readonly Dictionary<string, string> DictVNPTPayResponseStatus = new Dictionary<string, string>
        {
            {"00","Thành công"},
            {"01","Giao dịch thất bại"},
            {"02","Dữ liệu không đúng định dạng"},
            {"05","Không tìm thấy dữ liệu"},
            {"06","Lỗi hệ thống"},
            {"07","Chữ ký không đúng"},
            {"08","Tài khoản kết nối của merchant đang bị khóa"},
            {"09","Tài khoản kết nối của merchant không tồn tại"},
            {"96","Hệ thống đang bảo trì"}
        };

        public class EnumMerchantResponseCode
        {
            /// <summary>
            /// Thành công
            /// </summary>
            public const string _00 = "00"; // Thành công"
            /// <summary>
            /// Giao dịch không tồn tại
            /// </summary>
            public const string _01 = "01"; // Giao dịch không tồn tại
            /// <summary>
            /// Giao dịch đã thanh toán(đã confirm)
            /// </summary>
            public const string _02 = "02"; // Giao dịch đã thanh toán(đã confirm)
            /// <summary>
            /// Chữ ký không đúng
            /// </summary>
            public const string _07 = "07"; // Chữ ký không đúng
            /// <summary>
            /// Hệ thống merchant bảo trì hoặc timeout
            /// </summary>
            public const string _08 = "08"; // Hệ thống merchant bảo trì hoặc timeout
            /// <summary>
            /// Các lỗi khác
            /// </summary>
            public const string _99 = "99"; // Các lỗi khác
        }

        public static readonly Dictionary<string, string> DictMerchantResponseStatus = new Dictionary<string, string>
        {
            {"00","Thành công"},
            {"01","Giao dịch không tồn tại"},
            {"02","Giao dịch đã thanh toán(đã confirm)"},
            {"07","Chữ ký không đúng"},
            {"08","Hệ thống merchant bảo trì hoặc timeout"},
            {"99","Các lỗi khác"}
        };

        public class EnumTransactionStatus
        {
            /// <summary>
            /// Giao dịch thành công
            /// </summary>
            public const string _50 = "50"; // Giao dịch thành công
            /// <summary>
            /// Giao dịch khởi tạo
            /// </summary>
            public const string _0 = "0"; // Giao dịch khởi tạo
            /// <summary>
            /// Giao dịch đang chờ xử lý
            /// </summary>
            public const string _5 = "5"; // Giao dịch đang chờ xử lý
            /// <summary>
            /// Giao dịch thất bại
            /// </summary>
            public const string _55 = "55"; // Giao dịch thất bại
            /// <summary>
            /// Hệ thống đang bận
            /// </summary>
            public const string _99 = "-99"; // Hệ thống đang bận
        }

        public static readonly Dictionary<string, string> DictTransactionStatus = new Dictionary<string, string>
        {
            {"50","Giao dịch thành công"},
            {"0","Giao dịch khởi tạo"},
            {"5","Giao dịch đang chờ xử lý"},
            {"55","Giao dịch thất bại"},
            {"-99","Hệ thống đang bận"}
        };

        public class EnumQRCodeType
        {
            /// <summary>
            ///  QR cho các điểm Offline
            /// </summary>
            public const string QR_CODE_TYPE_OFFLINE = "01";
            /// <summary>
            /// QR thanh toán Hóa đơn
            /// </summary>
            public const string QR_CODE_TYPE_INVOICE = "02";
        }

        public class ConstsContentTypes
        {
            public const string JSON = "application/json";
            public const string JSON_UTF8 = "application/json;charset=UTF-8";
            public const string FORM_URLENCODED = "application/x-www-form-urlencoded";
            public const string FORM_DATA = "multipart/form-data";
            public const string FILE_PDF = "application/pdf";
        }
    }
}