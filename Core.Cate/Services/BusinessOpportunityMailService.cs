using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Hosting;
using Core.Cate.Caches;
using Core.Cate.Models;
using Newtonsoft.Json;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Core.Cate.Services
{
    /// <summary>
    /// Xử lý gửi mail thông báo thêm hoặc xóa thành viên tham gia cơ hội kinh doanh.
    /// </summary>
    public class BusinessOpportunityMailService
    {
        private const string SysDataProviderName = "CenIT.Provider.Sys";
        private const string SysUserGetByUserNameProcedure = "Sys_User_GetByUserName";
        private const string UpdateStatusTemplateConfigKey = "MailTemplate_BusinessOpportunityUpdateStatus";

        private readonly RM_BusinessOpportunityCache _businessOpportunityCache;
        private readonly MailTemplateService _mailTemplateService;
        private readonly NotificationService _notificationService;

        #region Constructor
        public BusinessOpportunityMailService()
        {
            _businessOpportunityCache = new RM_BusinessOpportunityCache();
            _mailTemplateService = new MailTemplateService();
            _notificationService = new NotificationService();
        }
        #endregion

        #region Public Methods
        public void QueueSendUpdateStatusMail(
            int businessOpportunityId,
            IEnumerable<string> userNames,
            string actionUserName,
            List<string> ccEmails = null)
        {
            List<string> normalizedUserNames = NormalizeUserNames(userNames);
            if (normalizedUserNames.Count == 0)
            {
                return;
            }

            QueueMailWork(delegate
            {
                SendUpdateStatusMail(businessOpportunityId, normalizedUserNames, actionUserName, ccEmails);
            });
        }

        public void SendUpdateStatusMail(int businessOpportunityId, IEnumerable<string> userNames, string actionUserName, List<string> ccUserNames = null)
        {
            RM_BusinessOpportunityModel businessOpportunity = _businessOpportunityCache.GetById(businessOpportunityId);
            if (businessOpportunity == null)
            {
                return;
            }

            SysUserMailInfo actionUser = GetUserByUserName(actionUserName);
            string actionByFullName = actionUser != null ? actionUser.FullName : actionUserName;

            // Email của toàn bộ người nhận chính — dùng để loại khỏi CC,
            // tránh người vừa là quản lý vừa là thành viên tham gia nhận trùng thư
            var toEmails = new HashSet<string>(
                NormalizeUserNames(userNames)
                    .Select(GetUserByUserName)
                    .Where(x => CanSend(x))
                    .Select(x => x.Email.Trim()),
                StringComparer.OrdinalIgnoreCase);

            // Convert CC UserName -> Email, loại người đã có trong danh sách nhận chính
            var ccEmails = NormalizeUserNames(ccUserNames)
                .Select(GetUserByUserName)
                .Where(x => CanSend(x))
                .Select(x => x.Email.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(x => !toEmails.Contains(x))
                .ToList();

            // Gửi kèm thông báo trên header cho cả người nhận chính và người nhận CC
            List<string> notificationReceivers = NormalizeUserNames(userNames)
                .Concat(NormalizeUserNames(ccUserNames))
                .ToList();

            _notificationService.PushOpportunityNotification(
                businessOpportunityId,
                "Cập nhật cơ hội kinh doanh: " + businessOpportunity.OpportunityName,
                BuildStatusNotificationContent(businessOpportunity, actionByFullName),
                notificationReceivers,
                "BO_UPDATE_STATUS",
                actionUserName,
                actionByFullName,
                "fa-handshake",
                "text-primary");

            // CC chỉ đính kèm vào MỘT thư duy nhất — người quản lý không nhận
            // trùng một bản CC cho mỗi thành viên trong danh sách nhận chính
            List<string> pendingCcEmails = ccEmails;

            foreach (string userName in NormalizeUserNames(userNames))
            {
                SysUserMailInfo recipient = GetUserByUserName(userName);
                if (!CanSend(recipient))
                {
                    continue;
                }

                Dictionary<string, object> templateData = BuildTemplateData(
                    businessOpportunity,
                    recipient,
                    actionByFullName);

                string jsonData = JsonConvert.SerializeObject(templateData);

                _mailTemplateService.SendByConfigKey(
                    UpdateStatusTemplateConfigKey,
                    recipient.Email,
                    jsonData,
                    actionUserName,
                    pendingCcEmails);

                // Các thư tiếp theo không CC nữa
                pendingCcEmails = null;
            }
        }
        #endregion

        #region Private Methods
        private Dictionary<string, object> BuildTemplateData(
            RM_BusinessOpportunityModel businessOpportunity,
            SysUserMailInfo recipient,
            string actionByFullName)
        {
            Dictionary<string, object> templateData =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { "FullName", recipient.FullName },
                    { "ToEmail", recipient.Email },
                    { "OpportunityName", businessOpportunity.OpportunityName },
                    { "CustomerName", businessOpportunity.CustomerName },
                    { "Description", StripHtml(businessOpportunity.ExchangeContent) },
                    { "ActionByFullName", actionByFullName },
                    { "StatusName", businessOpportunity.StatusName },
                    { "ExchangeDate", businessOpportunity.ExchangeDate?.ToString("dd/MM/yyyy HH:mm") },
                    { "ContactPersonName", businessOpportunity.ContactPersonName },
                    { "SentAt", DateTime.Now.ToString("dd/MM/yyyy HH:mm") }
                };

            return templateData;
        }

        /// <summary>
        /// Tạo nội dung tóm tắt cho thông báo cập nhật cơ hội kinh doanh.
        /// </summary>
        /// <param name="businessOpportunity">Cơ hội kinh doanh liên quan.</param>
        /// <param name="actionByFullName">Họ tên người thực hiện cập nhật.</param>
        /// <returns>Nội dung tóm tắt hiển thị trên chuông thông báo.</returns>
        private string BuildStatusNotificationContent(
            RM_BusinessOpportunityModel businessOpportunity,
            string actionByFullName)
        {
            List<string> parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(businessOpportunity.StatusName))
            {
                parts.Add("Trạng thái: " + businessOpportunity.StatusName);
            }

            if (!string.IsNullOrWhiteSpace(businessOpportunity.CustomerName))
            {
                parts.Add("Khách hàng: " + businessOpportunity.CustomerName);
            }

            string exchangeContent = StripHtml(businessOpportunity.ExchangeContent);
            if (!string.IsNullOrWhiteSpace(exchangeContent))
            {
                parts.Add(exchangeContent);
            }

            if (!string.IsNullOrWhiteSpace(actionByFullName))
            {
                parts.Add("Người thực hiện: " + actionByFullName);
            }

            return string.Join(" - ", parts);
        }

        private List<string> NormalizeUserNames(IEnumerable<string> userNames)
        {
            IEnumerable<string> sourceUserNames = userNames ?? Enumerable.Empty<string>();

            List<string> normalizedUserNames = sourceUserNames
                .Where(userName => !string.IsNullOrWhiteSpace(userName))
                .Select(userName => userName.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return normalizedUserNames;
        }

        private string StripHtml(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string htmlDecoded = System.Web.HttpUtility.HtmlDecode(value);
            string plainText = Regex.Replace(htmlDecoded, "<.*?>", string.Empty);

            return plainText.Trim();
        }

        private bool CanSend(SysUserMailInfo user)
        {
            bool canSend = user != null
                && !string.IsNullOrWhiteSpace(user.Email)
                && UtilString.IsValidEmail(user.Email);

            return canSend;
        }

        private SysUserMailInfo GetUserByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return null;
            }

            try
            {
                SysUserMailInfo user = AppProcessor.ProcedureProvider.ExecuteScalarObject<SysUserMailInfo>(
                    SysUserGetByUserNameProcedure,
                    SysDataProviderName,
                    userName);

                return user;
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
                return null;
            }
            finally
            {
                // Không có xử lý bổ sung.
            }
        }

        private void QueueMailWork(Action action)
        {
            if (action == null)
            {
                return;
            }

            HostingEnvironment.QueueBackgroundWorkItem(delegate
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    AppProcessor.Logger.Error(ex);
                }
                finally
                {
                    // Không có xử lý bổ sung.
                }

                return Task.CompletedTask;
            });
        }
        #endregion

        #region Other
        private class SysUserMailInfo
        {
            #region Property

            public string UserName { get; set; }

            public string FullName { get; set; }

            public string Email { get; set; }

            #endregion
        }
        #endregion
    }
}
