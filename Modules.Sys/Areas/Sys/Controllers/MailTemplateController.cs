using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Core.Cate.Models;
using Core.Cate.Services;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Sys.Areas.Sys.Controllers
{
    /// <summary>
    /// Controller quản lý danh mục mẫu email hệ thống.
    /// </summary>
    public class MailTemplateController : AppController
    {
        private const string InvalidTemplateMessageKey = "SysMailTemplate_Message_InvalidData";
        private const string TemplateCodeRequiredMessageKey = "SysMailTemplate_Message_TemplateCodeRequired";
        private const string TemplateNameRequiredMessageKey = "SysMailTemplate_Message_TemplateNameRequired";
        private const string SubjectRequiredMessageKey = "SysMailTemplate_Message_SubjectRequired";
        private const string TemplateContentRequiredOnCreateMessageKey = "SysMailTemplate_Message_TemplateContentRequiredOnCreate";
        private const string TemplateContentRequiredOnEditMessageKey = "SysMailTemplate_Message_TemplateContentRequiredOnEdit";
        private const string DuplicateParamCodeMessageKey = "SysMailTemplate_Message_DuplicateParamCode";
        private const string ParamCodeWhitespaceMessageKey = "SysMailTemplate_Message_ParamCodeNoWhitespace";
        private const string TemplateNotFoundMessageKey = "SysMailTemplate_Message_TemplateNotFound";
        private const string TemplateFileNotFoundMessageKey = "SysMailTemplate_Message_TemplateFileNotFound";
        private const string SaveFailedMessageKey = "SysMailTemplate_Message_SaveFailed";

        private readonly string _mailTemplateTitle = AppProcessor.Messagor.GetMessage("SysMailTemplate_Title");
        private readonly string _invalidTemplateMessage = AppProcessor.Messagor.GetMessage(InvalidTemplateMessageKey);
        private readonly string _templateCodeRequiredMessage = AppProcessor.Messagor.GetMessage(TemplateCodeRequiredMessageKey);
        private readonly string _templateNameRequiredMessage = AppProcessor.Messagor.GetMessage(TemplateNameRequiredMessageKey);
        private readonly string _subjectRequiredMessage = AppProcessor.Messagor.GetMessage(SubjectRequiredMessageKey);
        private readonly string _templateContentRequiredOnCreateMessage = AppProcessor.Messagor.GetMessage(TemplateContentRequiredOnCreateMessageKey);
        private readonly string _templateContentRequiredOnEditMessage = AppProcessor.Messagor.GetMessage(TemplateContentRequiredOnEditMessageKey);
        private readonly string _duplicateParamCodeMessage = AppProcessor.Messagor.GetMessage(DuplicateParamCodeMessageKey);
        private readonly string _paramCodeWhitespaceMessage = AppProcessor.Messagor.GetMessage(ParamCodeWhitespaceMessageKey);
        private readonly string _templateNotFoundMessage = AppProcessor.Messagor.GetMessage(TemplateNotFoundMessageKey);
        private readonly string _templateFileNotFoundMessage = AppProcessor.Messagor.GetMessage(TemplateFileNotFoundMessageKey);
        private readonly string _saveFailedMessage = AppProcessor.Messagor.GetMessage(SaveFailedMessageKey);
        private readonly SysMailTemplateCache _sysMailTemplateCache = new SysMailTemplateCache();
        private readonly MailTemplateService _mailTemplateService = new MailTemplateService();

        /// <summary>
        /// Hiển thị màn hình danh sách mẫu email.
        /// </summary>
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Lấy dữ liệu mẫu email để hiển thị lên lưới.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get()
        {
            var search = Request.Form["search"];
            var drawValue = Request.Form.GetValues("draw")?[0];

            var baseSearchModel = new BaseSearchModel
            {
                Search = string.IsNullOrWhiteSpace(search) ? null : search
            };

            int total;
            var data = _sysMailTemplateCache.Get(out total, baseSearchModel);

            return Json(
                new
                {
                    draw = Convert.ToInt32(drawValue),
                    recordsTotal = total,
                    recordsFiltered = total,
                    data
                },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị popup thêm mới mẫu email.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add()
        {
            var model = new SysMailTemplateModel
            {
                IsActive = true
            };

            return PartialView("_Add", model);
        }

        /// <summary>
        /// Xử lý thêm mới mẫu email.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(SysMailTemplateModel model)
        {
            return SaveTemplate(model, EnumProcessType.Add);
        }

        /// <summary>
        /// Hiển thị popup cập nhật mẫu email.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _mailTemplateService.GetTemplateForEdit(id);
            if (model == null)
            {
                return Json(
                    new
                    {
                        status = false,
                        message = CreateMessage(_mailTemplateTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    },
                    JsonRequestBehavior.AllowGet);
            }

            return PartialView("_Edit", model);
        }

        /// <summary>
        /// Xử lý cập nhật mẫu email.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(SysMailTemplateModel model)
        {
            return SaveTemplate(model, EnumProcessType.Edit);
        }

        /// <summary>
        /// Hiển thị popup hướng dẫn sử dụng màn mẫu email.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Guide()
        {
            return PartialView("_Guide");
        }

        /// <summary>
        /// Tải file mẫu email hiện tại.
        /// </summary>
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Download(int id)
        {
            var model = _sysMailTemplateCache.GetById(id);
            if (model == null || string.IsNullOrWhiteSpace(model.FilePath))
            {
                return HttpNotFound(_templateNotFoundMessage);
            }

            var virtualPath = model.FilePath.Replace("\\", "/").TrimStart('~', '/');
            var absolutePath = HostingEnvironment.MapPath(string.Concat("~/", virtualPath));
            if (string.IsNullOrWhiteSpace(absolutePath) || !System.IO.File.Exists(absolutePath))
            {
                return HttpNotFound(_templateFileNotFoundMessage);
            }

            var bytes = System.IO.File.ReadAllBytes(absolutePath);
            var fileName = Path.GetFileName(absolutePath);

            return File(bytes, "text/plain", fileName);
        }

        /// <summary>
        /// Hiển thị popup xác nhận xóa mẫu email.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _sysMailTemplateCache.GetById(id);
            if (model == null)
            {
                return Json(
                    new
                    {
                        status = false,
                        message = CreateMessage(_mailTemplateTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    },
                    JsonRequestBehavior.AllowGet);
            }

            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                _mailTemplateTitle.ToLower(),
                model.TemplateName);

            return PartialView("_Delete", model);
        }

        /// <summary>
        /// Xử lý xóa mẫu email.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(SysMailTemplateModel model)
        {
            var result = _mailTemplateService.DeleteTemplate(model.MailTemplateId, User.UserName);
            if (result > 0)
            {
                _sysMailTemplateCache.ClearCache();
            }

            var response = CreateMessage(
                string.Format("{0} [{1}]", _mailTemplateTitle, model.TemplateName),
                EnumProcessType.Delete,
                result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);

            return Json(
                new
                {
                    status = result > 0,
                    message = response
                },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Validate và lưu dữ liệu mẫu email.
        /// </summary>
        private ActionResult SaveTemplate(SysMailTemplateModel model, EnumProcessType processType)
        {
            ValidateTemplate(model);

            if (!ModelState.IsValid)
            {
                return PartialView("_MailTemplate", model);
            }

            try
            {
                var result = _mailTemplateService.SaveTemplate(model, User.UserName);
                if (result == -9)
                {
                    var existedMessage = CreateMessage(
                        string.Format("{0} [{1}]", _mailTemplateTitle, model.TemplateCode),
                        EnumProcessType.DataExisted,
                        EnumMsgIcon.Error);

                    return Json(
                        new
                        {
                            status = false,
                            errorCode = -9,
                            message = existedMessage
                        },
                        JsonRequestBehavior.AllowGet);
                }

                if (result > 0)
                {
                    _sysMailTemplateCache.ClearCache();
                }

                var response = CreateMessage(
                    string.Format("{0} [{1}]", _mailTemplateTitle, model.TemplateName),
                    processType,
                    result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);

                return Json(
                    new
                    {
                        status = result > 0,
                        message = response
                    },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
                ModelState.AddModelError(string.Empty, _saveFailedMessage);
                return PartialView("_MailTemplate", model);
            }
        }

        /// <summary>
        /// Kiểm tra dữ liệu đầu vào trước khi lưu mẫu email.
        /// </summary>
        private void ValidateTemplate(SysMailTemplateModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError(string.Empty, _invalidTemplateMessage);
                return;
            }

            if (string.IsNullOrWhiteSpace(model.TemplateCode))
            {
                ModelState.AddModelError("TemplateCode", _templateCodeRequiredMessage);
            }

            if (string.IsNullOrWhiteSpace(model.TemplateName))
            {
                ModelState.AddModelError("TemplateName", _templateNameRequiredMessage);
            }

            if (string.IsNullOrWhiteSpace(model.SubjectTemplate))
            {
                ModelState.AddModelError("SubjectTemplate", _subjectRequiredMessage);
            }

            var hasUploadFile = model.TemplateFile != null && model.TemplateFile.ContentLength > 0;
            var hasTemplateContent = !string.IsNullOrWhiteSpace(model.TemplateContent);

            if (model.MailTemplateId <= 0 && !hasUploadFile && !hasTemplateContent)
            {
                ModelState.AddModelError("TemplateContent", _templateContentRequiredOnCreateMessage);
            }

            if (model.MailTemplateId > 0 && !hasUploadFile && !hasTemplateContent && string.IsNullOrWhiteSpace(model.FilePath))
            {
                ModelState.AddModelError("TemplateContent", _templateContentRequiredOnEditMessage);
            }

            ValidateTemplateParams(model.TemplateParams);
        }

        /// <summary>
        /// Kiểm tra danh sách tham số khai báo trên mẫu email.
        /// </summary>
        private void ValidateTemplateParams(IEnumerable<SysMailTemplateParamModel> templateParams)
        {
            if (templateParams == null)
            {
                return;
            }

            var normalizedTemplateParams = templateParams
                .Where(item => item != null && !string.IsNullOrWhiteSpace(item.ParamCode))
                .ToList();

            var duplicateParamCodes = normalizedTemplateParams
                .GroupBy(item => item.ParamCode.Trim(), StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicateParamCodes.Count > 0)
            {
                ModelState.AddModelError(
                    "TemplateParams",
                    string.Format(_duplicateParamCodeMessage, string.Join(", ", duplicateParamCodes)));
            }

            foreach (var templateParam in normalizedTemplateParams)
            {
                if (templateParam.ParamCode.IndexOf(" ", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ModelState.AddModelError(
                        "TemplateParams",
                        string.Format(_paramCodeWhitespaceMessage, templateParam.ParamCode));
                }

                if (string.IsNullOrWhiteSpace(templateParam.ParamName))
                {
                    templateParam.ParamName = templateParam.ParamCode.Trim();
                }
            }
        }
    }
}
