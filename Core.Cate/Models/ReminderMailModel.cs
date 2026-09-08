using System;
using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class ReminderProjectOpportunityModel
    {
        public int ObjectID { get; set; }
        public string ObjectType { get; set; }
        public string ObjectName { get; set; }

        public string UserNames { get; set; }
    }
}
