using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Hosting;
using Core.Cate.Models;
using Newtonsoft.Json;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Core.Cate.Services
{
    /// <summary>
    /// Xử lý gửi mail thông báo thêm hoặc xóa thành viên tham gia cơ hội kinh doanh.
    /// </summary>
    public class ReminderMailService
    {
        private const string SysDataProviderName = "CenIT.Provider.Sys";
        private const string SysUserGetByUserNameProcedure = "Sys_User_GetByUserName";
        private const string ReminderTemplateConfigKey = "MailTemplate_ReminderReport";

        private readonly MailTemplateService _mailTemplateService;
        private readonly NotificationService _notificationService;

        #region Constructor

        public ReminderMailService()
        {
            _mailTemplateService = new MailTemplateService();
            _notificationService = new NotificationService();
        }

        #endregion

        #region Public Methods

        public void QueueSendReminderMail(ReminderProjectOpportunityModel model, IEnumerable<string> userNames)
        {
            List<string> normalizedUserNames = NormalizeUserNames(userNames);
            if (normalizedUserNames.Count == 0)
            {
                return;
            }

            QueueMailWork(delegate
            {
                SendReminderMail(model, normalizedUserNames);
            });
        }

        public void SendReminderMail(ReminderProjectOpportunityModel model, IEnumerable<string> userNames)
        {
            // Gửi kèm thông báo trên header, bấm vào sẽ mở chi tiết dự án hoặc cơ hội tương ứng
            bool isProject = string.Equals(model.ObjectType, "Dự án", StringComparison.OrdinalIgnoreCase);
            string sourceType = isProject ? NotificationSourceType.Project : NotificationSourceType.Opportunity;

            _notificationService.Push(
                sourceType,
                model.ObjectID,
                "Nhắc cập nhật " + (model.ObjectType ?? "cơ hội kinh doanh"),
                model.ObjectName,
                NormalizeUserNames(userNames),
                "REMINDER_REPORT",
                "System",
                null,
                "fa-bell",
                "text-warning");

            foreach (string userName in NormalizeUserNames(userNames))
            {
                SysUserMailInfo recipient = GetUserByUserName(userName);
                if (!CanSend(recipient))
                {
                    continue;
                }

                Dictionary<string, object> templateData =
                    BuildTemplateData(model, recipient);

                string jsonData = JsonConvert.SerializeObject(templateData);

                _mailTemplateService.SendByConfigKey(
                    ReminderTemplateConfigKey,
                    recipient.Email,
                    jsonData,
                    "System");
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<string, object> BuildTemplateData(ReminderProjectOpportunityModel model, SysUserMailInfo recipient)
        {
            string domain = ConfigurationManager.AppSettings["App_HostUrl"]?.TrimEnd('/');
            string detailUrl = string.Empty;

            if (string.Equals(model.ObjectType, "Dự án", StringComparison.OrdinalIgnoreCase))
            {
                detailUrl = string.Format("{0}/Cate/ProjectOverview/Index/{1}", domain, model.ObjectID);
            } else
            {
                detailUrl = string.Format("{0}/Cate/BusinessOpportunityOverview/Index/{1}", domain, model.ObjectID);
            }
            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { "FullName", recipient.FullName },
                { "ToEmail", recipient.Email },
                { "ObjectType", model.ObjectType },           // Dự án/Cơ hội
                { "ObjectName", model.ObjectName },           // Tên dự án/cơ hội
                { "ObjectID", model.ObjectID },               // ID
                { "DetailUrl", detailUrl },
                { "SentAt", DateTime.Now.ToString("dd/MM/yyyy HH:mm") }
            };
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
