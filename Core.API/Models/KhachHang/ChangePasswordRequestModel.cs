namespace Modules.API.Models.KhachHang
{
    public class ChangePasswordRequestModel
    {
        public string CustomerCode { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string Salt { get; set; }
    }

    public class ForgotPasswordModel
    {
        public string Username { get; set; } = string.Empty;
    }
}
