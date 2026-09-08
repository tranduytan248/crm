using System;
using System.Collections.Generic;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_OpportunityPlanModel : BaseModel
    {
        public int Id { get; set; }

        public int BusinessOpportunityID { get; set; }

        public string Username { get; set; }

        [CustomRequired]
        [CustomDisplayName("OpportunityPlan_PlanName_Label")]
        public string PlanName { get; set; }

        [CustomDisplayName("OpportunityPlan_Content_Label")]
        [CustomRequired]
        public string Content { get; set; }

        [CustomRequired]
        [CustomDisplayName("OpportunityPlan_WorkingDate_Label")]
        public DateTime? WorkingDate { get; set; }

        [CustomDisplayName("OpportunityPlan_AddressMeeting_Label")]
        [CustomRequired]
        public string AddressMeeting { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string RelatedPersonUsernames { get; set; }
        public List<RelatedPersonUserModel> RelatedPersonUsers { get; set; }
        public List<RM_OpportunityPlanRelatedPersonModel> RelatedPersons { get; set; }
        public List<string> ExistingRelatedPersonUsernames { get; set; }
    }
}
