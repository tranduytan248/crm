using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Core.Cate.Caches;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Models
{
    public class ProductProjectOverviewModel : BaseModel
    {
        public RM_ProductProjectModel ProductProjectModel { get; set; }
        public List<RM_ProductCostModel> ProductCosts { get; set; }
        public List<RM_RevenueReceivedModel> RevenueReceiveds { get; set; }
        public List<RM_ProjectMemberModel> ProjectMembers { get; set; }
        public List<RM_TaskManagementModel> ProjectTasks { get; set; }
        public List<RM_ContractsModel> Contracts { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }
        public int TotalMember { get; set; }
        public int CountCost { get; set; }
        public int CountRevenue { get; set; }
        public int TotalTask { get; set; }
        public int TotalContract { get; set; }
        public List<string> PMs { get; set; }
        public List<string> AMs { get; set; }
        public List<string> BAs { get; set; }

    }
}
