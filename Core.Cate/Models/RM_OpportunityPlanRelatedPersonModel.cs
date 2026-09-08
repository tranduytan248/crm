using System.Collections.Generic;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_OpportunityPlanRelatedPersonModel : BaseSearchModel
    {
        public int Id { get; set; }
        public int OpportunityPlanID { get; set; }
        public string Username { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        // Join từ Sys_Users
        public string FullName { get; set; }
        public string TenBoPhan { get; set; }
        public string TenChucVu { get; set; }
    }

    public class RM_OpportunityPlanRelatedPersonSearchModel : BaseSearchModel
    {
        public int OpportunityPlanID { get; set; }
    }

    public class RM_OpportunityPlanRelatedPersonFormModel
    {
        public int OpportunityPlanID { get; set; }

        public List<RelatedPersonUserModel> Users { get; set; }
        public List<string> ExistingUsernames { get; set; }

        public string Usernames { get; set; }

        public string Keyword { get; set; }
    }

    public class RelatedPersonUserModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string OfficeName { get; set; }
    }
}
