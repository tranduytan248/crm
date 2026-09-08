using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_RevenueAllocationModel : BaseModel
    {
        public int RevenueAllocationID { get; set; }   
        public int RevenueReceivedID { get; set; }
        public int ProjectMemberID { get; set; }
        public double AllocationRate { get; set; }

        // Thông tin join để hiển thị
        public string MemberName { get; set; }
        public string RoleName { get; set; }
        public int ProjectID { get; set; }
    }
}