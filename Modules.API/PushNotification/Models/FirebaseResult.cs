using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Modules.API.PushNotification.Models
{
    public class FirebaseResult
    {
        public string Token { get; set; }          // Thiết bị nhận
        public bool IsSuccess { get; set; }        // Trạng thái gửi
        public string MessageId { get; set; }      // Mã message trả về từ Firebase
        public string DeviceUUID { get; set; }      // UUID Device
        public string ErrorCode { get; set; }      // Mã lỗi nếu thất bại
        public string RawResponse { get; set; }    // Chuỗi JSON gốc
        public DateTime SentAt { get; set; } = DateTime.Now; // Thời điểm gửi

        public FirebaseResult()
        {
            SentAt = DateTime.Now;
        }
    }

    public class FirebaseResponse
    {
        public bool IsSuccess { get; set; }
        public string MessageId { get; set; }
        public string ErrorMessage { get; set; }
        public string RawResponse { get; set; }
    }

}