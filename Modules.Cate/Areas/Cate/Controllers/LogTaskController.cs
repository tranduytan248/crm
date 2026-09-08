using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Cate.Services;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using Core.Sys.Models.Sys;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Extensions;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Modules.Cate.Areas.Cate.Controllers
{
    /// <summary>
    /// Xử lý các luồng cập nhật tiến độ, chỉnh sửa nhật ký công việc và quản lý file đính kèm của log task.
    /// </summary>
    public class LogTaskController : AppController
    {
        private readonly RM_LogTaskCache _logTaskCache;
        private readonly SysUserCache _sysUserCache;
        private readonly RM_TaskAssigneeCache _taskAssigneeCache;
        private readonly RM_TaskActivityCache _taskActivityCache;
        private readonly RM_CommentCache _commentCache;
        private readonly RM_TaskManagementCache _taskManagementCache;
        private readonly ProjectTaskMailService _projectTaskMailService;
        private readonly SysConfigCache _configsCache;

        private readonly TaskFileService _fileService;


        private readonly string _folderImage = ConfigurationManager.AppSettings["AppImageRoot_Path"] ?? "/Contents/imgs";
        private readonly string _Title = AppProcessor.Messagor.GetMessage("LogTask_Title");
        private readonly string _noPermissionToUpdateProgressMessage =
            AppProcessor.Messagor.GetMessage("LogTask_Message_NoPermissionToUpdateProgress");

        /// <summary>
        /// Khởi tạo cache phục vụ xử lý nhật ký công việc dự án.
        /// </summary>
        /// </summary>
        public LogTaskController()
        {
            _logTaskCache = new RM_LogTaskCache();
            _sysUserCache = new SysUserCache();
            _taskAssigneeCache = new RM_TaskAssigneeCache();
            _taskActivityCache = new RM_TaskActivityCache();
            _commentCache = new RM_CommentCache();
            _taskManagementCache = new RM_TaskManagementCache();
            _projectTaskMailService = new ProjectTaskMailService();
            _fileService = new TaskFileService();
            _configsCache = new SysConfigCache();
        }

        private List<SelectListItem> GetEmployeeSelectList(int taskManagementID)
        {
            return _taskAssigneeCache.GetByTaskManagementID(taskManagementID)
                .Select(x => new SelectListItem
                {
                    Value = x.Employee_ID.ToString(),
                    Text = x.FullName
                }).ToList();
        }

        private string FormatDuration(TimeSpan? start, TimeSpan? end)
        {
            if (start == null || end == null) return null;

            var diff = end.Value - start.Value;

            int hours = (int)diff.TotalHours;
            int minutes = diff.Minutes;

            if (hours > 0 && minutes > 0)
                return $"{hours}h{minutes}";

            if (hours > 0)
                return $"{hours}h";

            return $"{minutes}";
        }

        private List<TaskTimelineModel> GetListTimeLine(int taskManagementID)
        {
            var activityList = _taskActivityCache.GetTaskActiveByTaskManagementID(taskManagementID);
            var activities = activityList.Where(x => x.CreatedDate != null).Select(x =>
            {
                var description = x.Description;

                try
                {
                    var payload = JsonConvert.DeserializeObject<dynamic>(x.Description);
                    var changes = payload?.Changes;
                    if (changes != null)
                    {
                        var lines = new List<string>();
                        foreach (var change in changes)
                        {
                            string line = change["Description"]?.ToString();
                            if (!string.IsNullOrEmpty(line))
                                lines.Add(line);
                        }
                        description = string.Join("\n", lines);
                    }
                }
                catch { /* giữ nguyên description gốc nếu parse lỗi */ }

                return new TaskTimelineModel
                {
                    Id = x.ActivityID,
                    Time = x.CreatedDate.Value,
                    UserName = x.EmployeeName,
                    Type = "SYSTEM",
                    TypeAction = "Log_system_label",
                    Description = description, 
                    Extra = null,
                    CanEdit = false,
                    CanDelete = false,
                };
            });
            var commentList = _commentCache.GetComentByTaskManagementID(taskManagementID);
            var comments = commentList.Where(x => x.CreatedDate != null).Select(x => new TaskTimelineModel
            {
                Id = x.CommentID,
                Time = x.CreatedDate.Value,
                UserName = x.EmployeeName,
                Type = "COMMENT",
                TypeAction = "Log_comment_label",
                Description = x.Content,
                Extra = null,
                CanEdit = x.Employee_ID == User.UserId,
                CanDelete = x.Employee_ID == User.UserId,
                FilePaths = _fileService.GetFilePaths(commentId: x.CommentID)
            });
            var logList = _logTaskCache.GetByTaskManagementID(taskManagementID);
            var logs = logList.Where(x => x.CreatedDate != null).Select(x => new TaskTimelineModel
            {
                Id = x.LogTaskID,
                Time = x.CreatedDate.Value,
                UserName = x.FullName,
                Type = "LOG",
                TypeAction = "Log_log_label",
                Description = x.Description,
                Extra = (x.StartTime != null && x.EndTime != null)
                        ? $"{x.StartTime:hh\\:mm} - {x.EndTime:hh\\:mm} ({FormatDuration(x.StartTime, x.EndTime)})"
                        : null,
                CanEdit = x.Employee_ID == User.UserId,
                CanDelete = x.Employee_ID == User.UserId,
                FilePaths = _fileService.GetFilePaths(logTaskId: x.LogTaskID)
            });
            var ListTimeLine = activities
                .Concat(logs)
                .Concat(comments)
                .OrderByDescending(x => x.Time)
                .ToList();
            return ListTimeLine;
        }

        /// <summary>
        /// Truy cập trang nhật ký công việc.
        /// </summary>
        /// <param name="taskManagementId">Id công việc cần ghi nhật lý.</param>
        /// <returns>giao diện timeline nhận ký.</returns>
        public ActionResult TaskLog(int taskManagementId)
        {
            ViewBag.taskManagementId = taskManagementId;
            var model = GetListTimeLine(taskManagementId);
            return PartialView("_LichSuLog", model);
        }

        /// <summary>
        /// Thêm nhật ký công vieecjc .
        /// </summary>
        /// <param id="taskManagementId">Id công việc cần ghi nhật lý.</param>
        /// <returns>giao diện Thêm nhật ký.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(int id)
        {
            var modal = new RM_LogTaskModel
            {
                TaskManagementID = id,
            };
            ViewBag.ListAssigess = GetEmployeeSelectList(id);
            return PartialView("_AddLog", modal);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_LogTaskModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ListAssigess = GetEmployeeSelectList(model.TaskManagementID);
                return PartialView("_LogTask", model);
            }
            string response;
            var result = _logTaskCache.Save(model, User.UserName);

            if (result == 0)
            {
                response = CreateMessage($"{_Title}", EnumProcessType.Add, EnumMsgIcon.Error);
            }
            else if (result == -9)
            {
                response = CreateMessage($"{_Title}", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            }
            else
            {
                response = CreateMessage($"{_Title}", EnumProcessType.Add, EnumMsgIcon.Success);

                var task = _taskManagementCache.GetById(model.TaskManagementID);

                if (task != null)
                {
                    var assigneeIds = (task.AssigneeIDs ?? string.Empty)
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => int.TryParse(x, out var id) ? id : 0)
                        .Where(id => id > 0)
                        .ToList();

                    // Người nhận (To)
                    var toUserNames = _sysUserCache.GetAll()
                        .Where(x => x.UserId.HasValue && assigneeIds.Contains(x.UserId.Value))
                        .Select(x => x.UserName)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    // Thêm người tạo task
                    if (!string.IsNullOrWhiteSpace(task.CreatedBy))
                    {
                        toUserNames.Add(task.CreatedBy);
                    }

                    toUserNames = toUserNames
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    // Người quản lý (CC)
                    var ccUserNames = new List<string>();

                    bool sendMailToManager =
                        bool.TryParse(
                            _configsCache.GetViaKey("CONFIG_SEND_MAIL_TO_MANAGER")?.ConfigValue,
                            out bool enabled)
                        && enabled;

                    if (sendMailToManager)
                    {
                        ccUserNames = (_sysUserCache.GetManagement(task.AssigneeIDs) ?? Enumerable.Empty<SysUserModel>())
                            .Select(x => x.UserName)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .Except(toUserNames, StringComparer.OrdinalIgnoreCase)
                            .ToList();
                    }

                    _projectTaskMailService.QueueSendProjectTaskLogMail(
                        result,
                        toUserNames,
                        User.UserName,
                        ccUserNames);

                    var dbRequest = new SaveFilePathRequest
                    {
                        LogTaskID = result
                    };

                    _fileService.SaveImagePathsFromHtml(
                        model.Description,
                        dbRequest,
                        User.UserName);

                    var files = model.Files?
                        .Where(f => f != null && f.ContentLength > 0)
                        .ToList();

                    if (files != null && files.Any())
                    {
                        _fileService.UploadAndSave(
                            files,
                            dbRequest,
                            User.UserName,
                            subFolder: "log");
                    }
                }
            }

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var modal = _logTaskCache.GetById(id);
            modal.FilePaths = _fileService.GetFilePaths(
                commentId: modal.LogTaskID
            );
            ViewBag.ListAssigess = GetEmployeeSelectList(modal.TaskManagementID);
            return PartialView("_EditLog", modal);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_LogTaskModel model)
        {
            if (!ModelState.IsValid)
            {
                model.FilePaths = _fileService.GetFilePaths(
                    logTaskId: model.LogTaskID
                );
                return View("_LogTask", model);
            }
            string response;
            var result = _logTaskCache.Save(model, User.UserName);
            if (result == 0)
                response = CreateMessage($"{_Title}", EnumProcessType.Edit, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_Title}", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
            {
                var dbRequest = new SaveFilePathRequest
                {
                    LogTaskID = result  // ← result là ID mới
                };
                _fileService.SaveImagePathsFromHtml(model.Description, dbRequest, User.UserName);

                var files = model.Files?.Where(f => f != null && f.ContentLength > 0).ToList();
                if (files != null && files.Any())
                    _fileService.UploadAndSave(files, dbRequest, User.UserName, subFolder: "Log");

                response = CreateMessage($"{_Title}", EnumProcessType.Edit, EnumMsgIcon.Success);
            }

            return Json(new { status = true, message = response, taskManagementId = result > 0 ? result : 0 }, JsonRequestBehavior.AllowGet);

        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _logTaskCache.GetById(id);

            if (model == null)
                return Json(new { status = true, message = CreateMessage($"{_Title}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_Title}");
            return PartialView("_DeleteLog", model);
        }

        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_LogTaskModel model)
        {
            var deleted = _logTaskCache.Delete(model, User.UserName);

            EnumProcessType processType;
            EnumMsgIcon msgIcon;

            if (deleted > 0)
            {
                processType = EnumProcessType.Delete;
                msgIcon = EnumMsgIcon.Success;
            }
            else if (deleted == 0)
            {
                processType = EnumProcessType.DataUsed;
                msgIcon = EnumMsgIcon.Error;
            }
            else
            {
                processType = EnumProcessType.Delete;
                msgIcon = EnumMsgIcon.Error;
            }

            var response = CreateMessage($"{_Title}", processType, msgIcon);

            return Json(new { status = true, message = response });
        }
    }
}
