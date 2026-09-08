namespace TSFramework.Libs.Models.Mail
{
    public class ConfigMailModel
    {
        public string Host { get; set; }
        public int Port { get; set; } = 587;
        public string UserCredential { get; set; }
        public string UserCredentialName { get; set; }
        public string UPass { get; set; }
        public bool EnableSsl { get; set; }
    }
}