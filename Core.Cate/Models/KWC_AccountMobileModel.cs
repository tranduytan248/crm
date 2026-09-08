using System;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;
namespace Core.Cate.Models
{
    public class KWC_AccountMobileModel : BaseModel
    {
        public int Account_ID { get; set; }
        public string AccountUser { get; set; }
        //public string CustomerName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastTimeLogin { get; set; }
        public bool IsActive { get; set; }
        public string TypeAccount { get; set; }
        public bool isSubscribedToEmail { get; set; } = false;
        public string Email { get; set; } = "";
        public string FullName { get; set; }
        public string Username { get; set; }
    }

    public class KWC_AccountMobileSearchModel
    {
        public string TuKhoa { get; set; } = "";
        public string Type { get; set; } = "";
    }

    public class KWC_ChangePasswordModel
    {

        [CustomDisplayName("User_Label_UserName")]
        public string CustomerCode { get; set; }
        [CustomDisplayName("User_Label_FullName")]
        public string CustomerName { get; set; }
        [CustomRequired]
        [CustomDisplayName("Authorize_Current_Password")]
        public string CurrentPassword { get; set; } = null;
        [CustomRequired]
        [CustomDisplayName("Authorize_New_Password")]
        public string NewPassword { get; set; } = null;

        [CustomRequired]
        [CustomDisplayName("Authorize_Confirm_Password")]
        [CustomCompare("NewPassword", ErrorMessage = "Common_MessageCompareNotMatch")]
        public string ConfirmPassword { get; set; }
        public string Salt { get; set; }
    }
}