using System.Collections.Generic;
//using Core.Cate.Models;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Attributes;

namespace Modules.Sys.Areas.Sys.Data
{
    public class UserPermitModel
    {
        [CustomRequired]
        [CustomDisplayName("User_Title")]
        public int? UserId { get; set; }

        [CustomDisplayName("User_Label_OfficeName")]
        public string OfficeName { get; set; }

        [CustomDisplayName("User_Label_FullName")]
        public string FullName { get; set; }

        [CustomDisplayName("User_Label_UserName")]
        public string UserName { get; set; }

        [CustomDisplayName("User_Label_Email")]
        public string Email { get; set; }

        [CustomDisplayName("Role_Title")] public string RoleIDs { get; set; }

        [CustomDisplayName("Role_Title")] public List<SysRoleModel> ListRoles { get; set; } = new List<SysRoleModel>();

        [CustomDisplayName("Module_Title")] public string ModuleIDs { get; set; }

        [CustomDisplayName("Module_Title")] public List<SysModuleModel> ListModules { get; set; } = new List<SysModuleModel>();

        [CustomDisplayName("TypeService_Title")] public string TypeServiceIDs { get; set; }

        //[CustomDisplayName("TypeService_Title")] public List<Cate_TypeServiceModel> ListTypeService { get; set; } = new List<Cate_TypeServiceModel>();
    }
}