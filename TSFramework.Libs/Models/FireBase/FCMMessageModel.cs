using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TSFramework.Libs.Models.FireBase
{
    public class FCMMessageModel
    {
        public string[] registration_ids { get; set; }
        public FCMNotificationModel notification { get; set; }
        public object data { get; set; }
    }
    public class FCMNotificationModel
    {
        public string title { get; set; }
        public string text { get; set; }
        public string priority { get; set; } = "HIGH";
        public string sound { get; set; } = "enabled";
    }
}
