
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Hosting;
using TSFramework.Libs.Models.Mail;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Providers;

namespace Core.Cate.Process
{
    public class EmailProcess
    {
        public void SendEmailThongBao<T>(T model, string templateKey, string email, string subject, DateTime? period = null, string contractCode = null)
        {
            int isSendEmail = int.Parse(ConfigurationManager.AppSettings["Email_ISSENDEMAIL"]);
            if (isSendEmail == 1)
            {
                try
                {
                    var emailIsValid = ValidateEmail(email);
                    if (!emailIsValid)
                    {
                        new LogProvider().Message($"Email không hợp lệ: {email}");
                        return;
                    }

                    var templatePath = HostingEnvironment.MapPath(ConfigurationManager.AppSettings[templateKey]);
                    var htmlBody = RenderTemplateHtmlProvider.RenderPartialToHtml(templatePath, model);

                    var logoAppPath = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["Logo_App_Path"]);
                    var logoAppBytes = System.IO.File.ReadAllBytes(logoAppPath);

                    AppProcessor.Mailer.PushEmail(new List<MailModel>
                {
                    new MailModel
                    {
                        From = null,
                        DisplayNameFrom = null,
                        Subject = subject,
                        To = new List<string> { email },
                        IsBodyHtml = true,
                        Body = htmlBody,
                        DicImgs = new Dictionary<string, byte[]>
                        {
                            { "LogoApp", logoAppBytes }
                        }
                    }
                });

                    new LogProvider().Message($"Gửi mail thành công tới {email}");

                    //new Cate_ServiceRequestsCache().SaveMailLog(new Cate_Email_LogModel()
                    //{
                    //    EmailTemplate = templateKey,
                    //    EmailReceive = email,
                    //    Title = subject,
                    //    CONTENT = htmlBody,
                    //    Period = period,
                    //    ContractCode = contractCode
                    //});
                }
                catch (Exception ex)
                {
                    new LogProvider().Message($"Lỗi khi gửi mail tới {email}: {ex.Message}");
                }
            }
        }

        private bool ValidateEmail(string emailAddress)
        {
            if (emailAddress == null) return false;
            // Define the regular expression pattern for email validation
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            // Create a Regex object with the pattern
            Regex regex = new Regex(pattern);
            // Use the IsMatch method to validate the email address
            bool isValid = regex.IsMatch(emailAddress);
            return isValid;
        }
    }
}
