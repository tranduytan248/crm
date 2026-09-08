using TSFramework.Libs.Attributes;

namespace CenIT.Solution.TOC.WebApp.Models
{
    public class ResetPasswordModel
    {
        [CustomDisplayName("User_Label_UserName")]
        public string UserName { get; set; }

        [CustomRequired]
        [CustomDisplayName("Authorize_New_Password")]
        public string NewPassword { get; set; } = null;

        [CustomRequired]
        [CustomDisplayName("Authorize_Confirm_Password")]
        [CustomCompare("NewPassword", ErrorMessage = "Common_MessageCompareNotMatch")]
        public string ConfirmPassword { get; set; }

        public string Salt { get; set; }

        public string ErrMessage { get; set; }

        public bool IsPermit { get; set; } = true;
    }
}