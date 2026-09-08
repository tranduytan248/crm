using System;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_ContractRemindersModel : BaseModel
    {
        public string ReminderId { get; set; }
        public int ContractID { get; set; }
        public DateTime? ReminderDate { get; set; }
        public string Title { get; set; }
    }


    public class Job_ContractPaymentReminderModel : RM_ContractsModel
    {
        public string AM_FullName { get; set; }
        public string AM_Email { get; set; }
        public string AM_Phone { get; set; }
        public string CustomerPhone { get; set; }
        public string ProjectName { get; set; }
    }
}
