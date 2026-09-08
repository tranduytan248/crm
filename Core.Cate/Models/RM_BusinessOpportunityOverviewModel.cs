using Core.Cate.Models;
using System.Collections.Generic;

namespace Core.Cate.Models
{
    public class RM_BusinessOpportunityOverviewModel
    {
        public RM_BusinessOpportunityModel BusinessOpportunity { get; set; }
        public RM_ExchangeHistoryFormModel ExchangeHistoryForm { get; set; }
        public List<RM_SalesTeamMembersModel> Members { get; set; }
        public List<RM_OpportunityPlanModel> Plans { get; set; }
        public List<RM_ReviewHistoryModel> ReviewHistory { get; set; }
        public int TotalExchangeHistory { get; set; }
        public int TotalMember { get; set; }
        public int TotalPlan { get; set; }
        public int TotalReviewHistory { get; set; }
        public int? ReviewBatchID { get; set; }
    }
}