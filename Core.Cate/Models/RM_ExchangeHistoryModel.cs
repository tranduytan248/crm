using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_ExchangeHistoryModel : BaseModel
    {
        public int ExchangeHistoryID { get; set; }
        [CustomDisplayName("ParticipatingMembers_Title")]
        public int MemberID { get; set; }
        [CustomDisplayName("ContactPerson_Title")]
        public int? ContactPerson_ID { get; set; }
        [CustomRequired]
        [CustomDisplayName("ExchangeDate_Label")]
        public DateTime ExchangeDate { get; set; }
        [CustomRequired]
        [CustomDisplayName("Opportunity_Status_Label")]
        public int StatusID { get; set; }
        [CustomDisplayName("Contact_Content")]
        public string ExchangeContent { get; set; }
        public int BusinessOpportunityID { get; set; }

        public List<RM_StatusModel> OpportunityStatus { get; set; }

        public List<RM_SalesTeamMembersModel> Members { get; set; }

        [CustomDisplayName("File_Attach_Label")]
        public string FileAttach { get; set; }
        public List<HttpPostedFileBase> DinhKemFile { get; set; }

        [CustomDisplayName("File_Attach_Label")]
        public string FileAttachEdit { get; set; }

        public List<HttpPostedFileBase> DinhKemFileEdit { get; set; }
        public List<RM_ExchangeHistoryFilePathModel> ExistingFiles { get; set; }
        public List<int> DeletedFileIds { get; set; }
        public List<RM_ExchangeHistoryFilePathModel> ExchangeHistoryFilePath { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string StatusName { get; set; }
        public string StatusClass { get; set; }
        public string ParticipatingMembers { get; set; }
        public List<int> ParticipatingMembersSelect { get; set; }
        public List<int> ParticipatingMembersEHSelect { get; set; }
        public string ParticipatingMemberNames { get; set; }
        public string Index { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPosition { get; set; }
        public List<SelectListItem> ListContactPerson { get; set; }
        public int CustomerID { get; set; }
    }

    public class RM_ExchangeHistorySearchModel : BaseModel
    {
        public int BusinessOpportunityID { get; set; }
    }

    public class RM_ExchangeHistoryFormModel
    {
        public RM_ExchangeHistoryModel RM_ExchangeHistory { get; set; }


        public List<RM_ExchangeHistoryModel> RM_ExchangeHistorys { get; set; }

    }
}
