using System.ComponentModel.DataAnnotations;
using TSFramework.Libs.Attributes;

namespace CenIT.Solution.TOC.WebApp.Models
{
    public class SignUpModel
    {
        [CustomDisplayName("Authorize_UserName")]
        [CustomRequired]
        public string UserName { get; set; }

        [CustomDisplayName("Authorize_Email")]
        [CustomRequired]
        //[EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        //[RegularExpression(@"^[a-zA-Z0-9._%+-]+(@vnpt\.vn)$", ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "{0} phải dài ít nhất {2} ký tự", MinimumLength = 6)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [CustomRequired]
        [CustomDisplayName("Authorize_Password")]
        public string Password { get; set; }

        [CustomRequired]
        [CustomDisplayName("Authorize_Confirm_Password")]
        [CustomCompare("NewPassword", ErrorMessage = "Common_MessageCompareNotMatch")]
        public string ConfirmPassword { get; set; }
    }
}