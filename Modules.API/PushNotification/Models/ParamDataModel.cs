using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Modules.API.PushNotification.Models
{
    public class ParamDataModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ParamDatas
    {
        public List<ParamDataModel> listParam { get; set; } = new List<ParamDataModel>();
    }

    public class ObjectDataJson
    {
        public string JsonData { get; set; }
    }
}