using DocumentFormat.OpenXml.Spreadsheet;
using System;

namespace Core.API.Models
{
    /// <summary>
    /// Model chứa thông tin đăng ký tài khoản
    /// </summary>
    public class MemberRegisterModel: APIModel
    {
        public int Nationlity { get; set; } = 0;
        public string Facebook_ID { get; set; } = string.Empty;
        public string Google_ID { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public int Gender { get; set; } = 0;

    }

    /// <summary>
    /// Model chứa thông tin khi đăng ký thành công
    /// </summary>
    public class DataReturnSignInModel
    {
        public App_AccommodationLocationModel MemberInfo { get; set; }
        public string TokenAccess { get; set; } = Guid.NewGuid().ToString();
        public DateTime SignInDate { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Model chứa thông tin đăng nhập tài khoản
    /// </summary>
    public class MemberSignInModel: APIModel
    {

        public string Password { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public object Method { get; set; }
    }

}