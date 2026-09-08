using System;
using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_SalesTeamMembersModel : BaseSearchModel
    {
        //MemberID 
        public int MemberID { get; set; }
        //EmployeeID 
        public int EmployeeID { get; set; }
        //Role 
        [CustomRequired]
        [CustomDisplayName("Role_Title")]
        public string RoleID { get; set; }
        //BusinessOpportunityID 
        public int BusinessOpportunityID { get; set; }
        //CreatedDate 
        public DateTime CreatedDate { get; set; }
        //CreatedBy 
        public string CreatedBy { get; set; }
        //UpdatedDate 
        public DateTime UpdatedDate { get; set; }
        //UpdatedBy 
        public string UpdatedBy { get; set; }
        //IsDeleted 
        public bool IsDeleted { get; set; }

        public string Employee_Code { get; set; }

        public string Employee_Name { get; set; }

        public string FullName { get; set; }

        public string RoleNames { get; set; }

        public List<RM_RolesModel> Roles { get; set; }
        public string TenBoPhan { get; set; }
        public string TenChucVu { get; set; }
        public string Phone { get; set; }
    }

    public class RM_SalesTeamMembersSearchModel : BaseSearchModel
    {
        public int BusinessOpportunityID { get; set; }

        public string DepartmentID { get; set; }
    }

    public class RM_SalesTeamMembersFormModel
    {
        public List<RM_SalesTeamMembersModel> SalesTeamMembers { get; set; }

        public List<MN_EmployeeModel> Employees { get; set; }

        public int BusinessOpportunityID { get; set; }

        public List<RM_RolesModel> Roles { get; set; }

        public string BoPhanID { get; set; }

        public List<MN_BoPhanModel> Departments { get; set; }


        [CustomRequired]
        [CustomDisplayName("Members_Title")]
        public string EmployeeIDs { get; set; }

        [CustomRequired]
        [CustomDisplayName("Role_Title")]
        public string RoleIDs { get; set; }

        public string Keyword { get; set; }
    }
}
