using TSFramework.Libs.Models.Base;

namespace Core.Sys.Models.Sys
{
    public class SysPermissionModel : BaseModel
    {
        public int PermissionId { get; set; }
        public int RoleId { get; set; }
        public int FunctionId { get; set; }
        public string Action { get; set; }
        public new int? TotalRow { get; set; } = 0;
    }
}