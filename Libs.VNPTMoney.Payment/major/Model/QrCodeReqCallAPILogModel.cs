using System;
using System.Collections.Generic;
using System.Data;

namespace CenIT.Libs.VNPTMoney.Payment.Major.Model
{
    public class QrCodeReqCallAPILogModel
    {
        public string APINameOrURL { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }

    }
}