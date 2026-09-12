using System;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_DigitalSalesMemberModel : BaseModel
    {
        public int MemberID { get; set; }
        public int DigitalSalesID { get; set; }

        [CustomRequired]
        [CustomDisplayName("DigitalSalesMember_User_Label")]
        public int UserID { get; set; }

        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        [CustomDisplayName("DigitalSalesMember_RoleTitle_Label")]
        public string RoleTitle { get; set; }

        [CustomDisplayName("DigitalSalesMember_IsAM_Label")]
        public bool IsAM { get; set; }

        [CustomDisplayName("DigitalSalesMember_Note_Label")]
        public string Note { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
