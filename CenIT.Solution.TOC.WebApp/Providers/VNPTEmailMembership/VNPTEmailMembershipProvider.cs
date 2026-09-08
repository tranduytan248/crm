using System.Configuration;
using CenIT.Solution.TOC.WebApp.Providers.VNPTEmailMembership.Helper;

namespace CenIT.Solution.TOC.WebApp.Providers.VNPTEmailMembership
{
    /// <summary>
    /// Xác th?c tài kho?n VNPT qua SMTP server.
    /// Phiên b?n ASP.NET MVC (.NET Framework) — không dùng DI hay Cookie middleware.
    /// Vi?c t?o FormsAuthenticationTicket do AccountController x? lý sau khi ValidateUser() tr? v? true.
    /// </summary>
    public class VNPTEmailMembershipProvider
    {
        private readonly string _mailServerUrl;
        private readonly int _mailServerPort;
        private readonly bool _enableSsl;

        public VNPTEmailMembershipProvider()
        {
            _mailServerUrl = ConfigurationManager.AppSettings["VNPT_MailServer_URL"] ?? "";
            _mailServerPort = int.TryParse(ConfigurationManager.AppSettings["VNPT_MailServer_Port"], out var port) ? port : 25;
            _enableSsl = bool.TryParse(ConfigurationManager.AppSettings["VNPT_MailServer_EnableSsl"], out var ssl) && ssl;
        }

        /// <summary>
        /// Xác th?c credentials c?a user VNPT qua SMTP server n?i b?.
        /// </summary>
        /// <param name="email">Email ??y ??, vd: nguyenvana@vnpt.vn</param>
        /// <param name="password">M?t kh?u email VNPT</param>
        /// <returns>true n?u SMTP xác th?c thành công</returns>
        public bool ValidateUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(_mailServerUrl))
                return false;

            return SmtpHelper.ValidateCredentials(email, password, _mailServerUrl, _mailServerPort, _enableSsl);
        }
    }
}