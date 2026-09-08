using System;

namespace Core.Cate.Models
{
    public class RM_BusinessOpportunityMemberMailModel
    {
        public string FullName { get; set; }

        public string ToEmail { get; set; }

        public string OpportunityName { get; set; }

        public string CustomerName { get; set; }

        public string Description { get; set; }

        public string RoleNames { get; set; }

        public string ActionByFullName { get; set; }

        public string ActionLabel { get; set; }

        public DateTime SentAt { get; set; }
    }
}
