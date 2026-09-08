using System.Collections.Generic;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Sys.Models.Sys
{
    public class SysRoleModel : BaseModel
    {
        public int RoleId { get; set; }

        [CustomRequired]
        [CustomDisplayName("Role_Label_Name")]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }
        public List<SysFunctionModel> Functions { get; set; }
        public string Permissions { get; set; }
        public new int? TotalRow { get; set; } = 0;

        [CustomDisplayName("User_Title")] public string Users { get; set; }

        [CustomDisplayName("User_Title")] public List<string> SelectedUsers { get; set; }

        [CustomDisplayName("User_Title")] public List<SysUserModel> ListUsers { get; set; }

        public int UserId { get; set; } = 0;
    }
}