using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Modules.API.Models
{
    public class SendOTPModel
    {
        public long IdOTP { get; set; }
    }
    public class VerifyOTPModel : SendOTPModel
    {
        public string OTP { get; set; }
    }
}