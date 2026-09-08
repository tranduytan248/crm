using System;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class ChatbotModel
    {
        public string ObjectType { get; set; }
        public string ObjectName { get; set; }
        public string CustomerName { get; set; }
        public string StatusName { get; set; }
        public string Members { get; set; }
        public string LastProgress { get; set; }
        public DateTime? LastProgressDate { get; set; }
    }
}
