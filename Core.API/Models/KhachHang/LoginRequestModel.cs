namespace Modules.API.Models.KhachHang
{
    public class LoginRequestModel
    {
        public string Method { get; set; }        
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CustomerCode { get; set; }
        public string Otp { get; set; }
        public string DeviceToken { get; set; }
    }

    public class KWC_User_LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceInfo { get; set; }
        public string DeviceUUID { get; set; }
    }
}
