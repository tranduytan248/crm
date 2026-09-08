using Core.Sys.BaseApp;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Attributes;
using Core.Cate.Caches;
using Core.Cate.Models;
using System.Web.Razor.Tokenizer.Symbols;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;
using System.Web.Services.Description;
using DocumentFormat.OpenXml.Office2010.Excel;
using Core.Sys.Caches.Sys;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using System.Web.Caching;
using Core.Sys.Models.Sys;
using DocumentFormat.OpenXml.EMMA;
using Core.Cate.Services;
using System.IO;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class CommentController : AppController
    {
        private readonly RM_CommentCache _commentCache;
        private readonly RM_TaskManagementCache _taskManagementCache;
        private readonly SysUserCache _sysUserCache;
        private readonly RM_LogTaskCache _filePathCache;
        private readonly SysConfigCache _configsCache;
        private readonly string _Title = AppProcessor.Messagor.GetMessage("Comment_Title");
        private readonly string _File = AppProcessor.Messagor.GetMessage("File_Title");
        private readonly TaskFileService _fileService;
        private readonly ProjectTaskMailService _projectTaskMailService;

        public CommentController()
        {
            _commentCache = new RM_CommentCache();
            _taskManagementCache = new RM_TaskManagementCache();
            _sysUserCache = new SysUserCache();
            _filePathCache = new RM_LogTaskCache();
            _configsCache = new SysConfigCache();
            _fileService = new TaskFileService();
            _projectTaskMailService = new ProjectTaskMailService();
        }


        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(int id)
        {
            var model = new RM_CommentModel
            {
                TaskManagementID = id
            };

            return PartialView("_Add", model);

        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_CommentModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Detail", model);
            }
            var task = _taskManagementCache.GetById(model.TaskManagementID);
            string response;
            model.Employee_ID = User.UserId;
            var result = _commentCache.Save(model, User.UserName);
            if (result > 0)
            {
                List<string> userNames = new List<string>();

                if (task != null)
                {
                    var assigneeIds = (task.AssigneeIDs ?? string.Empty)
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => int.TryParse(x, out var id) ? id : 0)
                        .Where(id => id > 0)
                        .ToList();

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

                    toUserNames = toUserNames.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                    var ccUserNames = new List<string>();

                    bool sendMailToManager = bool.TryParse(_configsCache.GetViaKey("CONFIG_SEND_MAIL_TO_MANAGER")?.ConfigValue, out bool enabled) && enabled;

                    if (sendMailToManager)
                    {
                        ccUserNames = (_sysUserCache.GetManagement(task.AssigneeIDs) ?? Enumerable.Empty<SysUserModel>())
                            .Select(x => x.UserName)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .Except(toUserNames, StringComparer.OrdinalIgnoreCase)
                            .ToList();
                    }

                    _projectTaskMailService.QueueSendProjectTaskLogMail(result, toUserNames, User.UserName, ccUserNames);
                }

                var dbRequest = new SaveFilePathRequest
                {
                    CommentID = result  // ← result là ID mới
                };
                _fileService.SaveImagePathsFromHtml(model.Content, dbRequest, User.UserName);

                var files = model.Files?.Where(f => f != null && f.ContentLength > 0).ToList();
                if (files != null && files.Any())
                    _fileService.UploadAndSave(files, dbRequest, User.UserName, subFolder: "comment");

            }
            if (result == 0)
                response = CreateMessage($"{_Title} [{task.TaskName}]", EnumProcessType.Add, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_Title} [{task.TaskName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_Title} [{task.TaskName}]", EnumProcessType.Add, EnumMsgIcon.Success);

            return Json(new { status = true, message = response, taskManagementId = result > 0 ? result : 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var modal = _commentCache.GetByID(id);
            modal.FilePaths = _fileService.GetFilePaths(
                commentId: modal.CommentID
            );
            return PartialView("_Edit", modal);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_CommentModel model)
        {
            if (!ModelState.IsValid)
            {
                model.FilePaths = _fileService.GetFilePaths(
                    commentId: model.CommentID
                );
                return View("_Edit", model);
            }
            string response;
            var Task = _taskManagementCache.GetById(model.TaskManagementID);
            model.Employee_ID = User.UserId;
            var result = _commentCache.Save(model, User.UserName);
            if (result > 0)
            {
                var dbRequest = new SaveFilePathRequest
                {
                    CommentID = result  // ← result là ID mới
                };
                _fileService.SaveImagePathsFromHtml(model.Content, dbRequest, User.UserName);

                var files = model.Files?.Where(f => f != null && f.ContentLength > 0).ToList();
                if (files != null && files.Any())
                    _fileService.UploadAndSave(files, dbRequest, User.UserName, subFolder: "comment");

            }
            if (result == 0)
                response = CreateMessage($"{_Title} [{Task.TaskName}]", EnumProcessType.Edit, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_Title} [{Task.TaskName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_Title} [{Task.TaskName}]", EnumProcessType.Edit, EnumMsgIcon.Success);

            return Json(new { status = true, message = response, taskManagementId = result > 0 ? result : 0 }, JsonRequestBehavior.AllowGet);

        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _commentCache.GetByID(id);
            var Task = _taskManagementCache.GetById(model.TaskManagementID);

            if (model == null)
                return Json(new { status = true, message = CreateMessage($"{_Title}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_Title} [{Task.TaskName}]");
            return PartialView("_Delete", model);
        }

        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_CommentModel model)
        {
            var deleted = _commentCache.Delete(model, User.UserName);

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

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteFile(int id)
        {
            var model = _fileService.GetFilePathById(id);

            if (model == null)
                return Json(new { status = true, message = CreateMessage($"{_File}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_File}");
            return PartialView("_DeleteFile", model);
        }

        //[AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteFile(RM_LogTaskFilePathModel model)
        {
            var modal = _fileService.GetFilePathById(model.FilePathID);
            var deleted = _fileService.DeleteFilePath(model.FilePathID, User.UserName);

            if (deleted > 0 && modal != null)
            {
                _fileService.DeletePhysicalFile(modal.FilePath);
            }
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

            var response = CreateMessage($"{_File}", processType, msgIcon);

            return Json(new { status = true, message = response });
        }

    }
}