using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Cate.Models
{
    public class RM_OpportunityPlanReminderModel
    {
        public int Id { get; set; }
        public int OpportunityPlanID { get; set; }
        public string ReminderType { get; set; }
        public DateTime ReminderTime { get; set; }
        public string SendStatus { get; set; }
    }
}
