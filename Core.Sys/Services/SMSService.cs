using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Core.Sys.Caches.Sys;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Principals;
using TSFramework.Libs.Processors;

namespace Core.Services
{
    /// <summary>
    /// Service gửi SMS theo template code.
    /// </summary>
    public class SMSService
    {
        private readonly SysSMSTemplateCache _sysSMSTemplateCache = new SysSMSTemplateCache();
        private readonly string _smsApiUrl = ConfigurationManager.AppSettings["Key_Send_SMS_API"];

        /// <summary>
        /// Gửi SMS cho nhiều số điện thoại dựa trên template code.
        /// </summary>
        /// <param name="templateCode">Code template SMS.</param>
        /// <param name="phoneNumbers">Danh sách số điện thoại nhận.</param>
        /// <param name="model">Object dữ liệu render template.</param>
        public void SendByTemplateCode(string templateCode, List<string> phoneNumbers, string username, object model = null)
        {
            if (string.IsNullOrWhiteSpace(templateCode))
                return;

            if (phoneNumbers == null || !phoneNumbers.Any())
                return;

            var template = _sysSMSTemplateCache.GetByCode(templateCode);

            if (template == null || !template.IsActive)
                return;

            string content = RenderTemplate(template.TemplateContent, model);

            content = NormalizeSmsContent(content);

            if (content.Length > 127)
            {
                content = content.Substring(0, 127);
            }

            foreach (var phone in phoneNumbers.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                try
                {
                    CallSmsApi(phone, content);
                    SaveSmsLog(phone, templateCode, content, true, null, username);
                }
                catch (Exception ex)
                {
                    SaveSmsLog(phone, templateCode, content, false, ex.Message, username);
                    AppProcessor.Logger.Message(
                        $"[SMS ERROR] TemplateCode: {templateCode} | Phone: {phone} | Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Render biến trong template theo format {{Variable}}.
        /// </summary>
        private string RenderTemplate(string template, object model)
        {
            if (model == null)
                return template;

            return Regex.Replace(
                template,
                @"\{\{(.*?)\}\}",
                match =>
                {
                    string propertyName = match.Groups[1].Value.Trim();

                    var prop = model.GetType().GetProperty(propertyName);

                    return prop?.GetValue(model)?.ToString() ?? "";
                });
        }

        /// <summary>
        /// Chuyển nội dung thành không dấu.
        /// </summary>
        private string NormalizeSmsContent(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            string normalized = input.Normalize(NormalizationForm.FormD);

            var chars = normalized
                .Where(c =>
                    CharUnicodeInfo.GetUnicodeCategory(c)
                    != UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars)
                .Replace('đ', 'd')
                .Replace('Đ', 'D');
        }

        /// <summary>
        /// Gọi API gửi SMS.
        /// </summary>
        private void CallSmsApi(string phoneNumber, string content)
        {
            using (var client = new HttpClient())
            {
                string url =
                    $"{_smsApiUrl}" +
                    $"?phoneNums={Uri.EscapeDataString(phoneNumber)}" +
                    $"&smsContents={Uri.EscapeDataString(content)}";

                var response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Send SMS failed.");
                }
            }
        }

        private void SaveSmsLog(string phoneNumber, string templateCode, string content, bool isSuccess, string errorMessage, string username)
        {
            try
            {
                var log = new SysSMSLogsModel
                {
                    PhoneNumber = phoneNumber,
                    TemplateCode = templateCode,
                    Content = content,
                    IsSuccess = isSuccess,
                    ErrorMessage = errorMessage
                };

                _sysSMSTemplateCache.InsertLog(log, username);
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Message(
                    $"[SMS LOG ERROR] {ex.Message}");
            }
        }
    }
}