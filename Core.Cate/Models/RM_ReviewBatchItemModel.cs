using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Models
{
    public class RM_ReviewBatchItemModel : BaseModel
    {
        public List<RM_ReviewProjectModel> ListProject { get; set; }
        public List<RM_ReviewBusinessOpportunityModel> ListBusinessOpportunity { get; set; }
    }

    public class RM_ReviewBatchItemSearchModel : BaseModel
    {
        public RM_ReviewProjectSearchModel ProjectSearch { get; set; }
        public RM_ReviewBusinessOpportunitySearchModel BusinessOpportunitySearch { get; set; }
    }

    public class RM_ReviewProjectSearchModel
    {
        public string Keyword { get; set; }
        public int ReviewBatchID { get; set; }
        public int UserLevel { get; set; }
        public int? Status { get; set; }
        public int BoPhanID { get; set; }
        public int EmployeeID { get; set; }
        public bool IsReviewed { get; set; } = false;
        public string UserName { get; set; }
        public List<RM_ReviewBatchModel> ListReviewPatch { get; set; }
        public List<RM_StatusModel> ListStatus { get; set; }
        public List<MN_BoPhanModel> Departments { get; set; }
    }

    public class RM_ReviewBusinessOpportunitySearchModel
    {
        public string Keyword { get; set; }
        public int ReviewBatchID { get; set; }
        public int UserLevel { get; set; }
        public int? StatusID { get; set; }
        public int BoPhanID { get; set; }
        public int EmployeeID { get; set; }
        public bool IsReviewed { get; set; } = false;
        public string UserName { get; set; }
        public List<RM_ReviewBatchModel> ListReviewPatch { get; set; }
        public List<RM_StatusModel> ListStatus { get; set; }
        public List<MN_BoPhanModel> Departments { get; set; }
    }

    public class RM_ReviewProjectFilterSessionModel : BaseSearchModel
    {
        public string Keyword { get; set; }
        public int? ReviewBatchID { get; set; }
        public int? Status { get; set; }
        public bool? IsReviewed { get; set; }
    }

    public class RM_ReviewBusinessOpportunityFilterSessionModel : BaseSearchModel
    {
        public string Keyword { get; set; }
        public int? ReviewBatchID { get; set; }
        public int? StatusID { get; set; }
        public bool? IsReviewed { get; set; }
    }

    public class RM_ReviewProjectModel : BaseModel
    {
        public int ReviewBatchID { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string StatusName { get; set; }
        public string StatusClass { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public int ReviewBatchItemID { get; set; }
        public int HighestReviewedLevel { get; set; }
        public DateTime? LastReviewedDate { get; set; }
        public bool IsReviewed { get; set; }
        public string AMName { get; set; }
        public DateTime? LastReviewDate { get; set; }
        public string LastReviewerName { get; set; }
        public string LastReviewComment { get; set; }
        public int ReviewCount { get; set; }
    }

    public class RM_ReviewBusinessOpportunityModel : BaseModel
    {
        public int ReviewPatchID { get; set; }
        public int BusinessOpportunityID { get; set; }
        public string CodeOpportunity { get; set; }
        public string OpportunityName { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string StatusName { get; set; }
        public string StatusClass { get; set; }
        public decimal ExpectedValue { get; set; }
        public decimal ClosingProbability { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ReviewBatchItemID { get; set; }
        public int HighestReviewedLevel { get; set; }
        public DateTime? LastReviewedDate { get; set; }
        public bool IsReviewed { get; set; }
        public string AMName { get; set; }
        public DateTime? LastReviewDate { get; set; }
        public string LastReviewerName { get; set; }
        public string LastReviewComment { get; set; }
        public int ReviewCount { get; set; }
    }

    public class RM_ReviewFormModel
    {
        public int ReviewHistoryID { get; set; }
        public int ReviewBatchID { get; set; }
        public int ReviewBatchItemID { get; set; }
        public byte ObjectType { get; set; }
        public int ObjectID { get; set; }
        public int UserLevel { get; set; }
        [CustomDisplayName("ReviewBatch_ReviewComment_Label")]
        public string ReviewComment { get; set; }
        [CustomDisplayName("ReviewBatch_Confirm_Label")]
        public bool IsConfirmed { get; set; }
        public List<HttpPostedFileBase> DinhKemFile { get; set; }
        public List<RM_ReviewBatchFilePathModel> ExistingFiles { get; set; }
        public List<int> DeletedFileIds { get; set; }
    }

    public class RM_ReviewHistoryModel : BaseModel
    {
        public int ReviewHistoryID { get; set; }
        public int ReviewBatchID { get; set; }
        public int ReviewBatchItemID { get; set; }
        public string Reviewer { get; set; }
        public string BatchCode { get; set; }
        public string BatchName { get; set; }
        public int ReviewLevel { get; set; }
        public byte ReviewAction { get; set; }
        public string ReviewComment { get; set; }
        public bool IsConfirmed { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<RM_ReviewBatchFilePathModel> ExistingFiles { get; set; }
    }

    public class RM_ReviewBatchFilePathModel : BaseModel
    {
        public int FilePathID { get; set; }
        public int ReviewHistoryID { get; set; }
        public string FilePath { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
