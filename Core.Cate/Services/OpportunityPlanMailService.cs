using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Xử lý gửi mail cho các luồng kế hoạch cơ hội kinh doanh.
    /// </summary>
    public class OpportunityPlanMailService
    {
        #region Declaration

        private const string SysDataProviderName = "CenIT.Provider.Sys";
        private const string SysUserGetByUserNameProcedure = "Sys_User_GetByUserName";
        private const string PlanCreatedTemplateConfigKey = "MailTemplate_OpportunityPlanCreated";
        private const string RelatedPersonAddedTemplateConfigKey = "MailTemplate_OpportunityPlanRelatedPersonAdded";
        private const string Reminder1DayTemplateConfigKey = "MailTemplate_OpportunityPlanReminder1Day";
        private const string Reminder30MinTemplateConfigKey = "MailTemplate_OpportunityPlanReminder30Min";

        private readonly RM_OpportunityPlanCache _opportunityPlanCache;
        private readonly RM_OpportunityPlanRelatedPersonCache _relatedPersonCache;
        private readonly MailTemplateService _mailTemplateService;
        private readonly NotificationService _notificationService;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo cache và service phục vụ gửi mail cho kế hoạch cơ hội kinh doanh.
        /// </summary>
        public OpportunityPlanMailService()
        {
            _opportunityPlanCache = new RM_OpportunityPlanCache();
            _relatedPersonCache = new RM_OpportunityPlanRelatedPersonCache();
            _mailTemplateService = new MailTemplateService();
            _notificationService = new NotificationService();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gửi email thông báo tạo mới kế hoạch cho người tạo kế hoạch.
        /// </summary>
        /// <param name="planId">Mã kế hoạch cần gửi thông báo.</param>
        /// <param name="creatorUserName">Username người tạo kế hoạch.</param>
        public void SendPlanCreatedMail(int planId, string creatorUserName)
        {
            RM_OpportunityPlanModel plan = _opportunityPlanCache.GetById(planId);
            if (plan == null)
            {
                return;
            }

            SysUserMailInfo recipient = GetUserByUserName(creatorUserName);
            if (!CanSend(recipient))
            {
                return;
            }

            // Gửi kèm thông báo trên header, bấm vào sẽ mở chi tiết cơ hội kinh doanh của kế hoạch
            _notificationService.PushOpportunityNotification(
                plan.BusinessOpportunityID,
                "Kế hoạch kinh doanh mới: " + plan.PlanName,
                BuildPlanNotificationContent(plan, recipient.FullName),
                new[] { creatorUserName },
                "PLAN_CREATED",
                creatorUserName,
                recipient.FullName,
                "fa-calendar-plus",
                "text-purple");

            Dictionary<string, object> templateData = BuildTemplateData(plan, recipient, recipient.FullName, string.Empty);
            string jsonData = JsonConvert.SerializeObject(templateData);

            _mailTemplateService.SendByConfigKey(
                PlanCreatedTemplateConfigKey,
                recipient.Email,
                jsonData,
                creatorUserName);
        }

        /// <summary>
        /// Đưa tác vụ gửi email tạo mới kế hoạch vào hàng đợi xử lý nền.
        /// </summary>
        /// <param name="planId">Mã kế hoạch cần gửi thông báo.</param>
        /// <param name="creatorUserName">Username người tạo kế hoạch.</param>
        public void QueueSendPlanCreatedMail(int planId, string creatorUserName)
        {
            QueueMailWork(delegate
            {
                SendPlanCreatedMail(planId, creatorUserName);
            });
        }

        /// <summary>
        /// Gửi email thông báo cho các người liên quan vừa được thêm vào kế hoạch.
        /// </summary>
        /// <param name="planId">Mã kế hoạch cần gửi thông báo.</param>
        /// <param name="userNamesRaw">Danh sách username được thêm mới.</param>
        public void SendRelatedPersonAddedMail(int planId, string userNamesRaw)
        {
            if (string.IsNullOrWhiteSpace(userNamesRaw))
            {
                return;
            }

            RM_OpportunityPlanModel plan = _opportunityPlanCache.GetById(planId);
            if (plan == null)
            {
                return;
            }

            SysUserMailInfo creator = GetUserByUserName(plan.Username);
            string creatorName = creator != null ? creator.FullName : plan.Username;

            // Gửi kèm thông báo trên header, bấm vào sẽ mở chi tiết cơ hội kinh doanh của kế hoạch
            _notificationService.PushOpportunityNotification(
                plan.BusinessOpportunityID,
                "Bạn được thêm vào kế hoạch: " + plan.PlanName,
                BuildPlanNotificationContent(plan, creatorName),
                SplitUserNames(userNamesRaw),
                "PLAN_RELATED_PERSON_ADDED",
                plan.Username,
                creatorName,
                "fa-user-plus",
                "text-purple");

            foreach (string userName in SplitUserNames(userNamesRaw))
            {
                SysUserMailInfo recipient = GetUserByUserName(userName);
                if (!CanSend(recipient))
                {
                    continue;
                }

                Dictionary<string, object> templateData = BuildTemplateData(plan, recipient, creatorName, string.Empty);
                string jsonData = JsonConvert.SerializeObject(templateData);

                _mailTemplateService.SendByConfigKey(
                    RelatedPersonAddedTemplateConfigKey,
                    recipient.Email,
                    jsonData,
                    plan.Username);
            }
        }

        /// <summary>
        /// Đưa tác vụ gửi email cho người liên quan mới vào hàng đợi xử lý nền.
        /// </summary>
        /// <param name="planId">Mã kế hoạch cần gửi thông báo.</param>
        /// <param name="userNamesRaw">Danh sách username được thêm mới.</param>
        public void QueueSendRelatedPersonAddedMail(int planId, string userNamesRaw)
        {
            QueueMailWork(delegate
            {
                SendRelatedPersonAddedMail(planId, userNamesRaw);
            });
        }

        /// <summary>
        /// Gửi email nhắc lịch trước một ngày cho kế hoạch.
        /// </summary>
        /// <param name="planId">Mã kế hoạch cần nhắc lịch.</param>
        public void SendReminder1DayMail(int planId)
        {
            SendReminderMail(planId, Reminder1DayTemplateConfigKey, null);
        }

        /// <summary>
        /// Gửi email nhắc lịch trước ba mươi phút cho kế hoạch.
        /// </summary>
        /// <param name="planId">Mã kế hoạch cần nhắc lịch.</param>
        public void SendReminder30MinMail(int planId)
        {
            SendReminderMail(planId, Reminder30MinTemplateConfigKey, null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gửi email nhắc lịch cho danh sách người nhận của kế hoạch theo mẫu email cấu hình.
        /// </summary>
        /// <param name="planId">Mã kế hoạch cần gửi nhắc lịch.</param>
        /// <param name="templateConfigKey">Khóa cấu hình mẫu email cần sử dụng.</param>
        /// <param name="note">Nội dung ghi chú gửi kèm email.</param>
        private void SendReminderMail(int planId, string templateConfigKey, string note)
        {
            if (string.IsNullOrWhiteSpace(templateConfigKey))
            {
                return;
            }

            RM_OpportunityPlanModel plan = _opportunityPlanCache.GetById(planId);
            if (plan == null)
            {
                return;
            }

            SysUserMailInfo creator = GetUserByUserName(plan.Username);
            string creatorName = creator != null ? creator.FullName : plan.Username;

            // Gửi kèm thông báo trên header, bấm vào sẽ mở chi tiết cơ hội kinh doanh của kế hoạch
            _notificationService.PushOpportunityNotification(
                plan.BusinessOpportunityID,
                "Nhắc lịch kế hoạch: " + plan.PlanName,
                BuildPlanNotificationContent(plan, creatorName),
                GetReminderRecipients(planId, plan.Username),
                "PLAN_REMINDER",
                plan.Username,
                creatorName,
                "fa-bell",
                "text-warning");

            foreach (string userName in GetReminderRecipients(planId, plan.Username))
            {
                SysUserMailInfo recipient = GetUserByUserName(userName);
                if (!CanSend(recipient))
                {
                    continue;
                }

                Dictionary<string, object> templateData = BuildTemplateData(
                    plan,
                    recipient,
                    creatorName,
                    BuildReminderNote(plan, note));

                string jsonData = JsonConvert.SerializeObject(templateData);

                _mailTemplateService.SendByConfigKey(
                    templateConfigKey,
                    recipient.Email,
                    jsonData,
                    plan.Username);
            }
        }

        /// <summary>
        /// Tạo nội dung tóm tắt cho thông báo kế hoạch cơ hội kinh doanh.
        /// </summary>
        /// <param name="plan">Kế hoạch liên quan.</param>
        /// <param name="creatorFullName">Họ tên người tạo kế hoạch.</param>
        /// <returns>Nội dung tóm tắt hiển thị trên chuông thông báo.</returns>
        private string BuildPlanNotificationContent(RM_OpportunityPlanModel plan, string creatorFullName)
        {
            plan = plan ?? new RM_OpportunityPlanModel();

            List<string> parts = new List<string>();

            if (plan.WorkingDate.HasValue)
            {
                parts.Add("Thời gian: " + plan.WorkingDate.Value.ToString("dd/MM/yyyy HH:mm"));
            }

            if (!string.IsNullOrWhiteSpace(plan.AddressMeeting))
            {
                parts.Add("Địa điểm: " + plan.AddressMeeting);
            }

            if (!string.IsNullOrWhiteSpace(creatorFullName))
            {
                parts.Add("Người tạo: " + creatorFullName);
            }

            return string.Join(" - ", parts);
        }

        /// <summary>
        /// Lấy danh sách người nhận email nhắc lịch, bao gồm người tạo kế hoạch và người liên quan.
        /// </summary>
        /// <param name="planId">Mã kế hoạch cần lấy người nhận.</param>
        /// <param name="creatorUserName">Username người tạo kế hoạch.</param>
        /// <returns>Danh sách username nhận email nhắc lịch.</returns>
        private IEnumerable<string> GetReminderRecipients(int planId, string creatorUserName)
        {
            List<string> recipients = new List<string>();

            if (!string.IsNullOrWhiteSpace(creatorUserName))
            {
                recipients.Add(creatorUserName);
            }

            int total;
            List<RM_OpportunityPlanRelatedPersonModel> relatedPersons =
                _relatedPersonCache.GetByOpportunityPlanID(planId, out total, null)
                ?? new List<RM_OpportunityPlanRelatedPersonModel>();

            recipients.AddRange(
                relatedPersons
                    .Where(item => item != null
                        && !item.IsDeleted
                        && !string.IsNullOrWhiteSpace(item.Username))
                    .Select(item => item.Username));

            IEnumerable<string> normalizedRecipients = recipients
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase);

            return normalizedRecipients;
        }

        /// <summary>
        /// Tạo dữ liệu tham số dùng để render mẫu email kế hoạch cơ hội kinh doanh.
        /// </summary>
        /// <param name="plan">Dữ liệu kế hoạch cần gửi mail.</param>
        /// <param name="recipient">Thông tin người nhận email.</param>
        /// <param name="creatorFullName">Họ tên người tạo kế hoạch.</param>
        /// <param name="note">Nội dung ghi chú gửi kèm email.</param>
        /// <returns>Tập dữ liệu tham số cho template email.</returns>
        private Dictionary<string, object> BuildTemplateData(
            RM_OpportunityPlanModel plan,
            SysUserMailInfo recipient,
            string creatorFullName,
            string note)
        {
            plan = plan ?? new RM_OpportunityPlanModel();
            recipient = recipient ?? new SysUserMailInfo();

            Dictionary<string, object> templateData =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { "FullName", recipient.FullName ?? string.Empty },
                    { "ToEmail", recipient.Email ?? string.Empty },
                    { "PlanName", plan.PlanName ?? string.Empty },
                    { "Content", plan.Content ?? string.Empty },
                    { "WorkingDate", plan.WorkingDate },
                    { "WorkingDateText", FormatWorkingDate(plan.WorkingDate) },
                    { "AddressMeeting", plan.AddressMeeting ?? string.Empty },
                    { "CreatedByFullName", creatorFullName ?? string.Empty },
                    { "HostUrl", GetHostUrl() },
                    { "NoiDung", note ?? string.Empty }
                };

            return templateData;
        }

        /// <summary>
        /// Định dạng thời gian làm việc của kế hoạch để hiển thị trong nội dung email.
        /// </summary>
        /// <param name="workingDate">Thời gian làm việc của kế hoạch.</param>
        /// <returns>Chuỗi thời gian đã định dạng.</returns>
        private string FormatWorkingDate(DateTime? workingDate)
        {
            if (!workingDate.HasValue)
            {
                return string.Empty;
            }

            return workingDate.Value.ToString("dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Tách chuỗi username thô thành danh sách username hợp lệ và không trùng lặp.
        /// </summary>
        /// <param name="userNamesRaw">Chuỗi username đầu vào.</param>
        /// <returns>Danh sách username đã chuẩn hóa.</returns>
        private IEnumerable<string> SplitUserNames(string userNamesRaw)
        {
            if (string.IsNullOrWhiteSpace(userNamesRaw))
            {
                return Enumerable.Empty<string>();
            }

            IEnumerable<string> userNames = userNamesRaw
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            return userNames;
        }

        /// <summary>
        /// Kiểm tra một người dùng có đủ điều kiện để nhận email hay không.
        /// </summary>
        /// <param name="user">Thông tin người dùng cần kiểm tra.</param>
        /// <returns>True nếu người dùng có email hợp lệ để gửi.</returns>
        private bool CanSend(SysUserMailInfo user)
        {
            bool canSend = user != null
                && !string.IsNullOrWhiteSpace(user.Email)
                && UtilString.IsValidEmail(user.Email);

            return canSend;
        }

        /// <summary>
        /// Lấy thông tin email của người dùng theo username từ hệ thống.
        /// </summary>
        /// <param name="userName">Username cần tra cứu.</param>
        /// <returns>Thông tin người dùng phục vụ gửi mail.</returns>
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
        }

        /// <summary>
        /// Tạo nội dung ghi chú mặc định cho email nhắc lịch nếu không có nội dung truyền vào.
        /// </summary>
        /// <param name="plan">Dữ liệu kế hoạch dùng để tạo ghi chú.</param>
        /// <param name="defaultNote">Nội dung ghi chú ưu tiên nếu đã có sẵn.</param>
        /// <returns>Nội dung ghi chú dùng trong email nhắc lịch.</returns>
        private string BuildReminderNote(RM_OpportunityPlanModel plan, string defaultNote)
        {
            if (!string.IsNullOrWhiteSpace(defaultNote))
            {
                return defaultNote;
            }

            string planName = string.IsNullOrWhiteSpace(plan?.PlanName)
                ? AppProcessor.Messagor.GetMessage("OpportunityPlan_Message_DefaultPlanName")
                : string.Format(
                    AppProcessor.Messagor.GetMessage("OpportunityPlan_Message_PlanNameFormat"),
                    plan.PlanName);

            string note = string.Format(
                AppProcessor.Messagor.GetMessage("OpportunityPlan_Message_ReminderNote"),
                planName);

            return note;
        }

        /// <summary>
        /// Đưa tác vụ gửi mail vào hàng đợi xử lý nền để tránh chặn request hiện tại.
        /// </summary>
        /// <param name="action">Tác vụ gửi mail cần xử lý nền.</param>
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

                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// Lấy địa chỉ host của ứng dụng để chèn vào nội dung email.
        /// </summary>
        /// <returns>Địa chỉ host dùng trong template email.</returns>
        private string GetHostUrl()
        {
            string hostUrl = AppProcessor.Messagor.GetMessage("HostURL");
            if (string.IsNullOrWhiteSpace(hostUrl))
            {
                hostUrl = AppProcessor.Messagor.GetMessage("App_HostUrl");
            }

            if (string.IsNullOrWhiteSpace(hostUrl))
            {
                return "/";
            }

            return hostUrl.TrimEnd('/');
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
