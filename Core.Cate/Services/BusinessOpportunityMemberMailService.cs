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
    public class BusinessOpportunityMemberMailService
    {
        #region Declaration

        private const string SysDataProviderName = "CenIT.Provider.Sys";
        private const string SysUserGetByUserNameProcedure = "Sys_User_GetByUserName";
        private const string MemberAddedTemplateConfigKey = "MailTemplate_BusinessOpportunityMemberAdded";
        private const string MemberRemovedTemplateConfigKey = "MailTemplate_BusinessOpportunityMemberRemoved";

        private readonly RM_BusinessOpportunityCache _businessOpportunityCache;
        private readonly MailTemplateService _mailTemplateService;
        private readonly NotificationService _notificationService;

        #endregion

        #region Constructor

        public BusinessOpportunityMemberMailService()
        {
            _businessOpportunityCache = new RM_BusinessOpportunityCache();
            _mailTemplateService = new MailTemplateService();
            _notificationService = new NotificationService();
        }

        #endregion

        #region Public Methods

        public void QueueSendMemberAddedMail(
            int businessOpportunityId,
            IEnumerable<string> userNames,
            string roleNames,
            string actionUserName)
        {
            List<string> normalizedUserNames = NormalizeUserNames(userNames);
            if (normalizedUserNames.Count == 0)
            {
                return;
            }

            QueueMailWork(delegate
            {
                SendMemberAddedMail(businessOpportunityId, normalizedUserNames, roleNames, actionUserName);
            });
        }


        public void QueueSendMemberRemovedMail(
            int businessOpportunityId,
            string userName,
            string roleNames,
            string actionUserName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return;
            }

            QueueMailWork(delegate
            {
                SendMemberRemovedMail(businessOpportunityId, userName, roleNames, actionUserName);
            });
        }


        public void SendMemberAddedMail(
            int businessOpportunityId,
            IEnumerable<string> userNames,
            string roleNames,
            string actionUserName)
        {
            RM_BusinessOpportunityModel businessOpportunity = _businessOpportunityCache.GetById(businessOpportunityId);
            if (businessOpportunity == null)
            {
                return;
            }

            SysUserMailInfo actionUser = GetUserByUserName(actionUserName);
            string actionByFullName = actionUser != null ? actionUser.FullName : actionUserName;
            string normalizedRoleNames = NormalizeRoleNames(roleNames);

            // Gửi kèm thông báo trên header, bấm vào sẽ mở chi tiết cơ hội kinh doanh
            _notificationService.PushOpportunityNotification(
                businessOpportunityId,
                "Bạn được thêm vào cơ hội kinh doanh",
                BuildMemberNotificationContent(businessOpportunity, normalizedRoleNames, actionByFullName),
                NormalizeUserNames(userNames),
                "BO_MEMBER_ADDED",
                actionUserName,
                actionByFullName,
                "fa-user-plus",
                "text-primary");

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
                    actionByFullName,
                    normalizedRoleNames,
                    "được thêm vào");

                string jsonData = JsonConvert.SerializeObject(templateData);

                _mailTemplateService.SendByConfigKey(
                    MemberAddedTemplateConfigKey,
                    recipient.Email,
                    jsonData,
                    actionUserName);
            }
        }


        public void SendMemberRemovedMail(
            int businessOpportunityId,
            string userName,
            string roleNames,
            string actionUserName)
        {
            RM_BusinessOpportunityModel businessOpportunity = _businessOpportunityCache.GetById(businessOpportunityId);
            if (businessOpportunity == null)
            {
                return;
            }

            SysUserMailInfo recipient = GetUserByUserName(userName);
            if (!CanSend(recipient))
            {
                return;
            }

            SysUserMailInfo actionUser = GetUserByUserName(actionUserName);
            string actionByFullName = actionUser != null ? actionUser.FullName : actionUserName;

            // Gửi kèm thông báo trên header, bấm vào sẽ mở chi tiết cơ hội kinh doanh
            _notificationService.PushOpportunityNotification(
                businessOpportunityId,
                "Bạn bị xóa khỏi cơ hội kinh doanh",
                BuildMemberNotificationContent(businessOpportunity, NormalizeRoleNames(roleNames), actionByFullName),
                new[] { userName },
                "BO_MEMBER_REMOVED",
                actionUserName,
                actionByFullName,
                "fa-user-minus",
                "text-danger");

            Dictionary<string, object> templateData = BuildTemplateData(
                businessOpportunity,
                recipient,
                actionByFullName,
                NormalizeRoleNames(roleNames),
                "bị xóa khỏi");

            string jsonData = JsonConvert.SerializeObject(templateData);

            _mailTemplateService.SendByConfigKey(
                MemberRemovedTemplateConfigKey,
                recipient.Email,
                jsonData,
                actionUserName);
        }

        #endregion

        #region Private Methods

        private Dictionary<string, object> BuildTemplateData(
            RM_BusinessOpportunityModel businessOpportunity,
            SysUserMailInfo recipient,
            string actionByFullName,
            string roleNames,
            string actionLabel)
        {
            businessOpportunity = businessOpportunity ?? new RM_BusinessOpportunityModel();
            recipient = recipient ?? new SysUserMailInfo();

            Dictionary<string, object> templateData =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { "FullName", recipient.FullName ?? string.Empty },
                    { "ToEmail", recipient.Email ?? string.Empty },
                    { "OpportunityName", businessOpportunity.OpportunityName ?? string.Empty },
                    { "CustomerName", businessOpportunity.CustomerName ?? string.Empty },
                    { "Description", StripHtml(businessOpportunity.Description) },
                    { "RoleNames", roleNames ?? string.Empty },
                    { "ActionByFullName", actionByFullName ?? string.Empty },
                    { "ActionLabel", actionLabel ?? string.Empty },
                    { "SentAt", DateTime.Now.ToString("dd/MM/yyyy HH:mm") }
                };

            return templateData;
        }


        /// <summary>
        /// Tạo nội dung tóm tắt cho thông báo thành viên cơ hội kinh doanh.
        /// </summary>
        /// <param name="businessOpportunity">Cơ hội kinh doanh liên quan.</param>
        /// <param name="roleNames">Danh sách vai trò được gán.</param>
        /// <param name="actionByFullName">Họ tên người thực hiện.</param>
        /// <returns>Nội dung tóm tắt hiển thị trên chuông thông báo.</returns>
        private string BuildMemberNotificationContent(
            RM_BusinessOpportunityModel businessOpportunity,
            string roleNames,
            string actionByFullName)
        {
            businessOpportunity = businessOpportunity ?? new RM_BusinessOpportunityModel();

            List<string> parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(businessOpportunity.OpportunityName))
            {
                parts.Add(businessOpportunity.OpportunityName);
            }

            if (!string.IsNullOrWhiteSpace(businessOpportunity.CustomerName))
            {
                parts.Add("Khách hàng: " + businessOpportunity.CustomerName);
            }

            if (!string.IsNullOrWhiteSpace(roleNames))
            {
                parts.Add("Vai trò: " + roleNames);
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
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return normalizedUserNames;
        }


        private string NormalizeRoleNames(string roleNames)
        {
            if (string.IsNullOrWhiteSpace(roleNames))
            {
                return string.Empty;
            }

            string normalizedRoleNames = string.Join(
                ", ",
                roleNames.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => item.Trim())
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Distinct(StringComparer.OrdinalIgnoreCase));

            return normalizedRoleNames;
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
