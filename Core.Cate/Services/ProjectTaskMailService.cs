using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Core.Cate.Caches;
using Core.Cate.Models;
using Newtonsoft.Json;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Core.Cate.Services
{
    /// <summary>
    /// Xử lý gửi mail cho các luồng công việc cơ hội kinh doanh.
    /// </summary>
    public class ProjectTaskMailService
    {
        #region Declaration

        private const string SysDataProviderName = "CenIT.Provider.Sys";
        private const string SysUserGetByUserNameProcedure = "Sys_User_GetByUserName";
        private const string ProjectTaskCreatedTemplateConfigKey = "MailTemplate_ProjectTaskCreated";
        private const string ProjectTaskUpdatedTemplateConfigKey = "MailTemplate_ProjectTaskUpdated";
        private const string ProjectTaskLogTemplateConfigKey = "MailTemplate_ProjectTaskLog";

        private readonly RM_TaskManagementCache _taskManagementCache;
        private readonly RM_CommentCache _commentCache;
        private readonly MailTemplateService _mailTemplateService;
        private readonly NotificationService _notificationService;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo cache và service phục vụ gửi mail cho công việc dự án.
        /// </summary>
        public ProjectTaskMailService()
        {
            _taskManagementCache = new RM_TaskManagementCache();
            _commentCache = new RM_CommentCache();
            _mailTemplateService = new MailTemplateService();
            _notificationService = new NotificationService();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gửi email thông báo tạo mới công việc.
        /// </summary>
        /// <param name="taskId">Mã công việc cần gửi thông báo.</param>
        /// <param name="creatorUserName">Username người tạo công việc.</param>
        public void ProjectTaskCreatedMail(int taskId, List<string> assigneeUserNames, string creatorUserName, List<string> ccEmails = null)
        {
            var task = _taskManagementCache.GetById(taskId);

            if (task == null || assigneeUserNames == null || !assigneeUserNames.Any())
            {
                return;
            }

            // người tạo
            SysUserMailInfo creator = GetUserByUserName(creatorUserName);

            string creatorFullName = creator?.FullName ?? string.Empty;

            // Gửi kèm thông báo trên header, bấm vào sẽ mở chi tiết dự án hoặc cơ hội chứa công việc
            PushTaskNotification(task, "Bạn được giao công việc mới: " + task.TaskName,
                assigneeUserNames, "TASK_CREATED", creatorUserName, creatorFullName);

            // CC đã loại người nhận chính và chỉ đính kèm vào MỘT thư duy nhất,
            // tránh người quản lý nhận trùng một bản CC cho mỗi thành viên
            List<string> pendingCcEmails = ResolveCcEmails(ccEmails, assigneeUserNames);

            foreach (var assigneeUserName in assigneeUserNames.Distinct())
            {
                // người nhận
                SysUserMailInfo recipient = GetUserByUserName(assigneeUserName);

                if (!CanSend(recipient))
                {
                    continue;
                }

                Dictionary<string, object> templateData =
                    BuildTemplateData(
                        task,
                        null,
                        recipient,
                        creatorFullName,
                        string.Empty);

                string jsonData = JsonConvert.SerializeObject(templateData);

                _mailTemplateService.SendByConfigKey(
                    ProjectTaskCreatedTemplateConfigKey,
                    recipient.Email,
                    jsonData,
                    creatorUserName,
                    pendingCcEmails);

                // Các thư tiếp theo không CC nữa
                pendingCcEmails = null;
            }
        }

        /// <summary>
        /// Gửi email thông báo tạo mới công việc.
        /// </summary>
        /// <param name="taskId">Mã công việc cần gửi thông báo.</param>
        /// <param name="creatorUserName">Username người tạo công việc.</param>
        public void ProjectTaskUpdatedMail(int taskId, List<string> assigneeUserNames, string creatorUserName, List<string> ccEmails = null)
        {
            var task = _taskManagementCache.GetById(taskId);

            if (task == null || assigneeUserNames == null || !assigneeUserNames.Any())
            {
                return;
            }

            // người tạo
            SysUserMailInfo creator = GetUserByUserName(creatorUserName);

            string creatorFullName = creator?.FullName ?? string.Empty;

            // Gửi kèm thông báo trên header, bấm vào sẽ mở chi tiết dự án hoặc cơ hội chứa công việc
            PushTaskNotification(task, "Công việc được cập nhật: " + task.TaskName,
                assigneeUserNames, "TASK_UPDATED", creatorUserName, creatorFullName);

            // CC đã loại người nhận chính và chỉ đính kèm vào MỘT thư duy nhất,
            // tránh người quản lý nhận trùng một bản CC cho mỗi thành viên
            List<string> pendingCcEmails = ResolveCcEmails(ccEmails, assigneeUserNames);

            foreach (var assigneeUserName in assigneeUserNames.Distinct())
            {
                // người nhận
                SysUserMailInfo recipient = GetUserByUserName(assigneeUserName);

                if (!CanSend(recipient))
                {
                    continue;
                }

                Dictionary<string, object> templateData =
                    BuildTemplateData(
                        task,
                        null,
                        recipient,
                        creatorFullName,
                        string.Empty);

                string jsonData = JsonConvert.SerializeObject(templateData);

                _mailTemplateService.SendByConfigKey(
                    ProjectTaskUpdatedTemplateConfigKey,
                    recipient.Email,
                    jsonData,
                    creatorUserName,
                    pendingCcEmails);

                // Các thư tiếp theo không CC nữa
                pendingCcEmails = null;
            }
        }

        /// <summary>
        /// Đưa tác vụ gửi email tạo mới công việc vào hàng đợi xử lý nền.
        /// </summary>
        /// <param name="taskId">Mã công việc cần gửi thông báo.</param>
        /// <param name="creatorUserName">Username người tạo công việc.</param>
        public void QueueSendProjectTaskCreatedMail(int taskId, List<string> assigneeUserNames, string creatorUserName, List<string> ccEmails = null)
        {
            QueueMailWork(delegate
            {
                ProjectTaskCreatedMail(taskId, assigneeUserNames, creatorUserName, ccEmails);
            });
        }

        /// <summary>
        /// Đưa tác vụ gửi email tạo mới công việc vào hàng đợi xử lý nền.
        /// </summary>
        /// <param name="taskId">Mã công việc cần gửi thông báo.</param>
        /// <param name="creatorUserName">Username người tạo công việc.</param>
        public void QueueSendProjectTaskUpdatedMail(int taskId, List<string> assigneeUserNames, string creatorUserName, List<string> ccEmails = null)
        {
            QueueMailWork(delegate
            {
                ProjectTaskUpdatedMail(taskId, assigneeUserNames, creatorUserName, ccEmails);
            });
        }

        /// <summary>
        /// Gửi email thông báo cho các người liên quan vừa được thêm vào công việc.
        /// </summary>
        /// <param name="logTaskId">Mã công việc cần gửi thông báo.</param>
        /// <param name="userNamesRaw">Danh sách username được thêm mới.</param>
        public void SendProjectTaskLogMail(int commentId, List<string> assigneeUserNames, string creatorUserName, List<string> ccUserNames = null)
        {
            if (assigneeUserNames == null || !assigneeUserNames.Any())
            {
                return;
            }

            var logTask = _commentCache.GetByID(commentId);
            if (logTask == null)
            {
                return;
            }

            var task = _taskManagementCache.GetById(logTask.TaskManagementID);
            if (task == null)
            {
                return;
            }

            // Người tạo task
            SysUserMailInfo creator = GetUserByUserName(logTask.CreatedBy);

            string creatorName = creator != null ? creator.FullName : logTask.CreatedBy;

            // Chuẩn hóa danh sách người nhận
            var recipients = assigneeUserNames
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Gửi kèm thông báo trên header, bấm vào sẽ mở chi tiết dự án hoặc cơ hội chứa công việc
            PushTaskNotification(task, "Nhật ký công việc mới: " + task.TaskName,
                recipients.Concat(ccUserNames ?? new List<string>()).ToList(),
                "TASK_LOG", logTask.CreatedBy, creatorName);

            // CC đã loại toàn bộ người nhận chính và chỉ đính kèm vào MỘT thư duy nhất,
            // tránh người quản lý nhận trùng một bản CC cho mỗi thành viên
            List<string> pendingCcEmails = ResolveCcEmails(ccUserNames, recipients);

            foreach (string userName in recipients)
            {
                // Người nhận
                SysUserMailInfo recipient = GetUserByUserName(userName);

                if (!CanSend(recipient))
                {
                    continue;
                }

                Dictionary<string, object> templateData = BuildTemplateData(task, logTask, recipient, creatorName, string.Empty);

                string jsonData = JsonConvert.SerializeObject(templateData);

                _mailTemplateService.SendByConfigKey(
                    ProjectTaskLogTemplateConfigKey,
                    recipient.Email,
                    jsonData,
                    creatorUserName,
                    pendingCcEmails);

                // Các thư tiếp theo không CC nữa
                pendingCcEmails = null;
            }
        }

        /// <summary>
        /// Đưa tác vụ gửi email cho người liên quan mới vào hàng đợi xử lý nền.
        /// </summary>
        /// <param name="taskId">Mã công việc cần gửi thông báo.</param>
        /// <param name="userNamesRaw">Danh sách username được thêm mới.</param>
        public void QueueSendProjectTaskLogMail(int logTaskId, List<string> assigneeUserNames, string userNamesRaw, List<string> ccUserNames = null)
        {
            QueueMailWork(delegate
            {
                SendProjectTaskLogMail(logTaskId, assigneeUserNames, userNamesRaw, ccUserNames);
            });
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Chuẩn hóa danh sách CC trước khi gửi:
        /// - Quy đổi username về email (chấp nhận cả trường hợp đầu vào đã là email);
        /// - Khử trùng lặp theo email;
        /// - Loại toàn bộ email của người nhận chính (To) để người vừa là quản lý
        ///   vừa là thành viên tham gia không nhận trùng thư.
        /// </summary>
        /// <param name="ccUserNamesOrEmails">Danh sách username hoặc email cần CC.</param>
        /// <param name="toUserNames">Danh sách username người nhận chính.</param>
        /// <returns>Danh sách email CC đã chuẩn hóa.</returns>
        private List<string> ResolveCcEmails(IEnumerable<string> ccUserNamesOrEmails, IEnumerable<string> toUserNames)
        {
            var ccSource = (ccUserNamesOrEmails ?? Enumerable.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (ccSource.Count == 0)
            {
                return new List<string>();
            }

            // Email của toàn bộ người nhận chính — dùng để loại khỏi CC
            var toEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string toUserName in (toUserNames ?? Enumerable.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                SysUserMailInfo toUser = GetUserByUserName(toUserName.Trim());
                if (toUser != null && !string.IsNullOrWhiteSpace(toUser.Email))
                {
                    toEmails.Add(toUser.Email.Trim());
                }
            }

            var ccEmails = new List<string>();
            foreach (string entry in ccSource)
            {
                string email = null;

                if (UtilString.IsValidEmail(entry))
                {
                    email = entry;
                }
                else
                {
                    SysUserMailInfo ccUser = GetUserByUserName(entry);
                    if (CanSend(ccUser))
                    {
                        email = ccUser.Email.Trim();
                    }
                }

                if (string.IsNullOrWhiteSpace(email) || toEmails.Contains(email))
                {
                    continue;
                }

                if (!ccEmails.Contains(email, StringComparer.OrdinalIgnoreCase))
                {
                    ccEmails.Add(email);
                }
            }

            return ccEmails;
        }

        /// <summary>
        /// Tạo thông báo trên header cho một công việc, điều hướng tới dự án hoặc cơ hội chứa công việc đó.
        /// </summary>
        /// <param name="task">Công việc liên quan.</param>
        /// <param name="title">Tiêu đề thông báo.</param>
        /// <param name="userNames">Danh sách username người nhận.</param>
        /// <param name="notificationType">Mã loại nghiệp vụ.</param>
        /// <param name="createdBy">Username người thực hiện.</param>
        /// <param name="createdByFullName">Họ tên người thực hiện.</param>
        private void PushTaskNotification(
            RM_TaskManagementModel task,
            string title,
            IEnumerable<string> userNames,
            string notificationType,
            string createdBy,
            string createdByFullName)
        {
            if (task == null)
            {
                return;
            }

            _notificationService.PushTaskNotification(
                task.TaskManagementID,
                task.ProjectID,
                task.BusinessOpportunityID,
                title,
                BuildTaskNotificationContent(task, createdByFullName),
                userNames,
                notificationType,
                createdBy,
                createdByFullName);
        }

        /// <summary>
        /// Tạo nội dung tóm tắt cho thông báo công việc.
        /// </summary>
        /// <param name="task">Công việc liên quan.</param>
        /// <param name="createdByFullName">Họ tên người thực hiện.</param>
        /// <returns>Nội dung tóm tắt hiển thị trên chuông thông báo.</returns>
        private string BuildTaskNotificationContent(RM_TaskManagementModel task, string createdByFullName)
        {
            List<string> parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(task.ProjectName))
            {
                parts.Add("Dự án: " + task.ProjectName);
            }

            if (!string.IsNullOrWhiteSpace(task.StatusName))
            {
                parts.Add("Trạng thái: " + task.StatusName);
            }

            if (task.EndDate.HasValue)
            {
                parts.Add("Hạn: " + task.EndDate.Value.ToString("dd/MM/yyyy"));
            }

            if (!string.IsNullOrWhiteSpace(createdByFullName))
            {
                parts.Add("Người thực hiện: " + createdByFullName);
            }

            return string.Join(" - ", parts);
        }

        /// <summary>
        /// Tạo dữ liệu tham số dùng để render mẫu email công việc cơ hội kinh doanh.
        /// </summary>
        /// <param name="task">Dữ liệu công việc cần gửi mail.</param>
        /// <param name="recipient">Thông tin người nhận email.</param>
        /// <param name="creatorFullName">Họ tên người tạo công việc.</param>
        /// <param name="note">Nội dung ghi chú gửi kèm email.</param>
        /// <returns>Tập dữ liệu tham số cho template email.</returns>
        private Dictionary<string, object> BuildTemplateData(
            RM_TaskManagementModel task,
            RM_CommentModel log,
            SysUserMailInfo recipient,
            string creatorFullName,
            string note)
        {
            task = task ?? new RM_TaskManagementModel();
            log = log ?? new RM_CommentModel();
            recipient = recipient ?? new SysUserMailInfo();

            Dictionary<string, object> templateData =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { "FullName", recipient.FullName ?? string.Empty },
                { "ToEmail", recipient.Email ?? string.Empty },
                // Dự án
                { "ProjectName", task.ProjectName ?? string.Empty },
                // Công việc
                { "TaskName", task.TaskName ?? string.Empty },
                { "TaskCode", task.TaskManagementID },
                { "TaskType", task.TaskTypeName ?? string.Empty },
                // Trạng thái / ưu tiên
                { "Status", task.StatusName ?? string.Empty },
                { "Priority", task.PriorityName ?? string.Empty },
                // Người đảm nhận
                { "Assignee", task.AssigneeNames ?? string.Empty },
                // Thời gian
                { "EstimatedHours", task.EstimatedHours?.ToString("0.00") ?? "0.00" },
                { "ActualHours", task.ActualHours?.ToString("0.00") ?? "0.00" },
                // Ngày
                { "StartDate", task.StartDate?.ToString("dd/MM/yyyy") ?? string.Empty },
                { "EndDate", task.EndDate?.ToString("dd/MM/yyyy") ?? string.Empty },
                // Tiến độ
                { "Progress", $"{task.CompletionPercentage ?? 0} %" },
                // Mô tả
                { "Description", StripHtml(task.Description ?? string.Empty) },
                // ===== LOG TASK =====
                // Nội dung log
                { "LogDescription", StripHtml(log.Content ?? string.Empty) },

                // Thời gian ghi log
                { "CreatedDate", log.CreatedDate?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty },

                // Thông tin bổ sung
                { "CreatedByFullName", creatorFullName ?? string.Empty },
                { "HostUrl", GetHostUrl() },
            };

            return templateData;
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
        /// Convert chuỗi html thành nội dung thuần text để hiển thị trong email, bao gồm decode entity, bỏ tag và giữ nguyên xuống dòng.
        /// </summary>
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
