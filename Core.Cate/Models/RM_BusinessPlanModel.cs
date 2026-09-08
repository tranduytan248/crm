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
    public class RM_BusinessPlanModel : BaseModel
    {
        public int BusinessPlanID { get; set; }
        [CustomDisplayName("BusinessPlan_Name_Label")]
        [CustomRequired]
        public string BusinessPlanName { get; set; }
        [CustomDisplayName("BusinessPlan_PlanYear_Label")]
        [CustomRequired]
        public int? PlanYear { get; set; }
        [CustomDisplayName("BusinessPlan_DecisionNo_Label")]
        [CustomRequired]
        public string DecisionNo { get; set; }
        [CustomRequired]
        [CustomDisplayName("BusinessPlan_DecisionDate_Label")]
        public DateTime? DecisionDate { get; set; }
        [CustomDisplayName("Contact_Content")]
        public string Note { get; set; }
        [CustomDisplayName("File_Attach_Label")]
        public string FileAttach { get; set; }
        public List<HttpPostedFileBase> DinhKemFile { get; set; }
        public List<RM_BusinessPlanFilePathModel> ExistingFiles { get; set; }
        public List<int> DeletedFileIds { get; set; }
        public List<RM_BusinessPlanFilePathModel> BusinessPlanFilePath { get; set; }
    }
}
