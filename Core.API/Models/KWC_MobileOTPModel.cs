using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class KWC_MobileOTPModel
    {
        public long ID { get; set; }
        public string AccountUser { get; set; }
        public string PhoneNumber { get; set; }
        public string OTP { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceUUID { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string InfoOTP { get; set; }
    }

    public class QrCodeReqCallAPILogModel
    {
        public string APINameOrURL { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }

    }
}
