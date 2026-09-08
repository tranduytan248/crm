using System;
using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_ProjectMemberModel : BaseSearchModel
    {
        public int ProjectMemberID { get; set; }
        public int Employee_ID { get; set; }
        [CustomRequired]
        [CustomDisplayName("Role_Title")]
        public string RoleID { get; set; }
        public int ProductProjectID { get; set; }
        public string Employee_Code { get; set; }

        public string FullName { get; set; }

        public List<string> RoleNames { get; set; }

        public List<RM_RolesModel> Roles { get; set; }
    }

    public class RM_ProjectMemberSearchModel : BaseSearchModel
    {
        public int ProductProjectID { get; set; }
    }

    public class RM_ProjectMemberFormModel
    {
        public List<RM_ProjectMemberModel> ProjectMember { get; set; }

        public List<MN_EmployeeModel> Employees { get; set; }

        public int ProductProjectID { get; set; }

        public List<RM_RolesModel> Roles { get; set; }
        public List<MN_BoPhanModel> Departments { get; set; }

        [CustomRequired]
        [CustomDisplayName("Members_Title")]
        public string EmployeeIDs { get; set; }

        [CustomRequired]
        [CustomDisplayName("Role_Title")]
        public string RoleIDs { get; set; }
    }
}
