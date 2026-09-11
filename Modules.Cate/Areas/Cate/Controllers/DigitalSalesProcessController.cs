using System;
using System.Linq;
using System.Web.Mvc;
using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class DigitalSalesProcessController : AppController
    {
        private readonly RM_DigitalSalesProcessCache _processCache = new RM_DigitalSalesProcessCache();
        private readonly RM_DigitalSalesProgressCache _progressCache = new RM_DigitalSalesProgressCache();
        private readonly RM_DigitalSalesStatusCache _statusCache = new RM_DigitalSalesStatusCache();
        private readonly string _title = AppProcessor.Messagor.GetMessage("DigitalSalesProcess_Title");

        public ActionResult Index()
        {
            ViewBag.Statuses = _statusCache.GetAll();
            return View();
        }

        [AjaxOnly, HttpPost, ActionType(Type = EnumActionType.View)]
        public ActionResult Get()
        {
            var search = new RM_DigitalSalesProcessSearchModel
            {
                Search = Request.Form["search[value]"], BusinessType = ParseNullableInt(Request.Form["businessType"]),
                StatusID = ParseNullableInt(Request.Form["statusId"]), Order = Request.Form["order[0][column]"],
                OrderDir = Request.Form["order[0][dir]"], StartIndex = ParseInt(Request.Form["start"]),
                PageSize = ParseInt(Request.Form["length"], 10)
            };
            var data = _processCache.Get(out var total, search);
            return Json(new { draw = ParseInt(Request.Form["draw"]), recordsTotal = total, recordsFiltered = total, data });
        }

        [AjaxOnly, HttpGet, ActionType(Type = EnumActionType.Create)]
        public ActionResult Add()
        {
            return PartialView("_Add", Prepare(new RM_DigitalSalesProcessModel { SortOrder = 1, IsActive = true }));
        }

        [AjaxOnly, HttpPost, ValidateAntiForgeryToken, ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(RM_DigitalSalesProcessModel model) => SaveProcess(model, false);

        [AjaxOnly, HttpGet, ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _processCache.GetById(id);
            return model == null ? Missing() : PartialView("_Edit", Prepare(model));
        }

        [AjaxOnly, HttpPost, ValidateAntiForgeryToken, ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(RM_DigitalSalesProcessModel model) => SaveProcess(model, true);

        [AjaxOnly, HttpGet, ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _processCache.GetById(id);
            if (model == null) return Missing();
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"), $"{_title} [{model.ProcessName}]");
            return PartialView("_Delete", model);
        }

        [AjaxOnly, HttpPost, ValidateAntiForgeryToken, ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_DigitalSalesProcessModel model)
        {
            var result = _processCache.Delete(model.ProcessID, User.UserName);
            return Json(new { status = true, message = CreateMessage($"{_title} [{model.ProcessName}]", EnumProcessType.Delete, result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error) });
        }

        [AjaxOnly, HttpGet, ActionType(Type = EnumActionType.View)]
        public ActionResult Progress(int id)
        {
            var process = _processCache.GetById(id);
            if (process == null) return Missing();
            return PartialView("_Progress", new RM_DigitalSalesProgressPageModel
            {
                Process = process,
                Editor = new RM_DigitalSalesProgressModel { ProcessID = id, SortOrder = 1, IsActive = true },
                Items = _progressCache.GetByProcess(id)
            });
        }

        [AjaxOnly, HttpGet, ActionType(Type = EnumActionType.View)]
        public ActionResult ProgressList(int id)
        {
            return Json(new { status = true, data = _progressCache.GetByProcess(id) }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly, HttpGet, ActionType(Type = EnumActionType.Edit)]
        public ActionResult ProgressItem(int id)
        {
            var item = _progressCache.GetById(id);
            return Json(new { status = item != null, data = item }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly, HttpPost, ValidateAntiForgeryToken, ActionType(Type = EnumActionType.Edit)]
        public ActionResult SaveProgress(RM_DigitalSalesProgressModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray() });
            model.ProgressCode = model.ProgressCode.Trim().ToUpperInvariant();
            var isEdit = model.ProgressID > 0;
            var result = _progressCache.Save(model, User.UserName);
            var message = result == -9
                ? CreateMessage($"Tiến trình [{model.ProgressCode}]", EnumProcessType.DataExisted, EnumMsgIcon.Error)
                : CreateMessage($"Tiến trình [{model.ProgressName}]", isEdit ? EnumProcessType.Edit : EnumProcessType.Add, result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = result > 0, message });
        }

        [AjaxOnly, HttpPost, ValidateAntiForgeryToken, ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteProgress(int id)
        {
            var result = _progressCache.Delete(id, User.UserName);
            return Json(new { status = result > 0, message = CreateMessage("Tiến trình mẫu", EnumProcessType.Delete, result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error) });
        }

        private RM_DigitalSalesProcessModel Prepare(RM_DigitalSalesProcessModel model)
        {
            model.Statuses = _statusCache.GetAll();
            return model;
        }

        private ActionResult SaveProcess(RM_DigitalSalesProcessModel model, bool isEdit)
        {
            if (!ModelState.IsValid) return PartialView("_Process", Prepare(model));
            model.ProcessCode = model.ProcessCode.Trim().ToUpperInvariant();
            var result = _processCache.Save(model, User.UserName);
            var processType = isEdit ? EnumProcessType.Edit : EnumProcessType.Add;
            var message = result == -9
                ? CreateMessage($"{_title} [{model.ProcessCode}]", EnumProcessType.DataExisted, EnumMsgIcon.Error)
                : CreateMessage($"{_title} [{model.ProcessName}]", processType, result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message });
        }

        private ActionResult Missing() => Json(new { status = true, message = CreateMessage(_title, EnumProcessType.DataNotExist, EnumMsgIcon.Error) }, JsonRequestBehavior.AllowGet);
        private static int ParseInt(string value, int fallback = 0) { return int.TryParse(value, out var result) ? result : fallback; }
        private static int? ParseNullableInt(string value) { return int.TryParse(value, out var result) && result > 0 ? (int?)result : null; }
    }
}
