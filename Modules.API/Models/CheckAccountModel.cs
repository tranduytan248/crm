using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Modules.API.Models
{
    public class CheckAccountModel
    {
        public string AccountUser { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceInfo { get; set; }
        public string DeviceUUID { get; set; }
        public string UsingFor { get; set; } = "Sigin";
        public string TypeAccount { get; set; }

    }

    public class CreateAccountModel
    {
        public string AccountUser { get; set; }
        public string TypeAccount { get; set; }
        public string NewPassword { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceInfo { get; set; }
        public string DeviceUUID { get; set; }
        public string DeviceToken { get; set; }

    }
}
