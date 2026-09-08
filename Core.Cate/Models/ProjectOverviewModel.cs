using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Models
{
    public class ProjectOverviewModel : BaseModel
    {
        public RM_ProjectModel ProjectModel { get; set; }
        public List<RM_ProductProjectModel> ProductProjects { get; set; }
        public List<RM_ContractsModel> Contracts { get; set; }
        public List<RM_ReviewHistoryModel> ReviewHistory { get; set; }
        public decimal TotalExpectedRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public int TotalProduct { get; set; }
        public int TotalContract { get; set; }
        public int TotalReviewHistory { get; set; }
        public int? ReviewBatchID { get; set; }
    }
}
