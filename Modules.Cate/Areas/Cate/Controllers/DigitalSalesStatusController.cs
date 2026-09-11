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
    public class DigitalSalesStatusController : AppController
    {
        private readonly RM_DigitalSalesStatusCache _cache = new RM_DigitalSalesStatusCache();
        private readonly string _title = AppProcessor.Messagor.GetMessage("DigitalSalesStatus_Title");

        public ActionResult Index() => View();

        [AjaxOnly, HttpPost, ActionType(Type = EnumActionType.View)]
        public ActionResult Get()
        {
            var search = new RM_DigitalSalesStatusSearchModel
            {
                Search = Request.Form["search[value]"],
                BusinessType = ParseNullableInt(Request.Form["businessType"]),
                Order = Request.Form["order[0][column]"],
                OrderDir = Request.Form["order[0][dir]"],
                StartIndex = ParseInt(Request.Form["start"]),
                PageSize = ParseInt(Request.Form["length"], 10)
            };
            var data = _cache.Get(out var total, search);
            return Json(new { draw = ParseInt(Request.Form["draw"]), recordsTotal = total, recordsFiltered = total, data });
        }

        [AjaxOnly, HttpGet, ActionType(Type = EnumActionType.Create)]
        public ActionResult Add() => PartialView("_Add", new RM_DigitalSalesStatusModel { BusinessType = 1, SortOrder = 1, IsActive = true });

        [AjaxOnly, HttpPost, ValidateAntiForgeryToken, ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(RM_DigitalSalesStatusModel model) => Save(model, false);

        [AjaxOnly, HttpGet, ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _cache.GetById(id);
            return model == null ? Missing() : PartialView("_Edit", model);
        }

        [AjaxOnly, HttpPost, ValidateAntiForgeryToken, ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(RM_DigitalSalesStatusModel model) => Save(model, true);

        [AjaxOnly, HttpGet, ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _cache.GetById(id);
            if (model == null) return Missing();
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"), $"{_title} [{model.StatusName}]");
            return PartialView("_Delete", model);
        }

        [AjaxOnly, HttpPost, ValidateAntiForgeryToken, ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_DigitalSalesStatusModel model)
        {
            var result = _cache.Delete(model.StatusID, User.UserName);
            return Json(new { status = true, message = CreateMessage($"{_title} [{model.StatusName}]", EnumProcessType.Delete, result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error) });
        }

        private ActionResult Save(RM_DigitalSalesStatusModel model, bool isEdit)
        {
            if (model.IsDefault && !model.IsActive)
                ModelState.AddModelError("IsActive", "Trạng thái mặc định phải được kích hoạt.");
            if (!ModelState.IsValid) return PartialView("_Status", model);
            model.StatusCode = model.StatusCode.Trim().ToUpperInvariant();
            var result = _cache.Save(model, User.UserName);
            var processType = isEdit ? EnumProcessType.Edit : EnumProcessType.Add;
            var icon = result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error;
            var message = result == -9
                ? CreateMessage($"{_title} [{model.StatusCode}]", EnumProcessType.DataExisted, EnumMsgIcon.Error)
                : CreateMessage($"{_title} [{model.StatusName}]", processType, icon);
            return Json(new { status = true, message });
        }

        private ActionResult Missing() => Json(new { status = true, message = CreateMessage(_title, EnumProcessType.DataNotExist, EnumMsgIcon.Error) }, JsonRequestBehavior.AllowGet);
        private static int ParseInt(string value, int fallback = 0) { return int.TryParse(value, out var result) ? result : fallback; }
        private static int? ParseNullableInt(string value) { return int.TryParse(value, out var result) && result > 0 ? (int?)result : null; }
    }
}
