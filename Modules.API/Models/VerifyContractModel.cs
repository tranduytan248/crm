using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Modules.API.Models
{
    public class VerifyContractModel
    {
        public string ContractCode { get; set; }
        public string PhoneNumber { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceInfo { get; set; }
        public string DeviceUUID { get; set; }
    }

}