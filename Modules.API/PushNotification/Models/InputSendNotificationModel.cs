using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;

namespace Modules.API.PushNotification.Models
{
    public class InputSendNotificationModel
    {
        public string deviceToken { get; set; } 
        public string title { get; set; }       
        public string body { get; set; }
        public string deviceUUID { get; set; }
        public ObjectDataJson jsonInputData { get; set; }
    }

}