using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class KWC_AccountMobileModel
    {
        public int Account_ID { get; set; }
        public string AccountUser { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastTimeLogin { get; set; }
        public bool IsActive { get; set; }
        public string TypeAccount { get; set; }
    }

    // API ___________________________________

    public class KWC_AccountMobile_LoginModel
    {
        public string AccountUser { get; set; }
        public string Password { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceInfo { get; set; }
        public string DeviceUUID { get; set; }
        public string DeviceToken { get; set; }
    }
}
