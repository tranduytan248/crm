using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using Core.Sys.Models.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Sys.Areas.Sys.Controllers
{
    public class StatusController : AppController
    {
        private readonly string _statusTitle = AppProcessor.Messagor.GetMessage("Status_Title");
        private readonly RM_StatusCache _statusCache = new RM_StatusCache();
        private readonly SysConfigCache _configsCache = new SysConfigCache();

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = _statusTitle;
            var config = _configsCache.GetViaKey("STATUS_KEY");

            var searchModel = new RM_StatusSearchModel
            {
                ListStatusKey = config?.ConfigValue?.Split(';').Select(x => new SelectListItem
                {
                    Value = x,
                    Text = x
                })
                .ToList()
            };
            return View(searchModel);
        }

        #region Status - CURD

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(BaseSearchModel searchModel)
        {
            var draw = Request.Form.GetValues("draw")?[0];
            var order = Request.Form.GetValues("order[0][column]")?[0];
            var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0]);
            var search = Request.Form["search"];
            var searchKey = string.IsNullOrEmpty(Request.Form["searchKey"])
                            ? null
                            : Request.Form["searchKey"];

            var dataSearch = new BaseSearchModel
            {
                Search = string.IsNullOrEmpty(search) ? null : search,
                Order = order,
                OrderDir = orderDir,
                StartIndex = startRec,
                PageSize = pageSize,
            };
            var data = _statusCache.Get(out int total, searchKey, dataSearch);

            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult Add()
        {
            var config = _configsCache.GetViaKey("STATUS_KEY");

            var model = new RM_StatusModel
            {
                ListStatusKey = config?.ConfigValue?
                    .Split(';')
                    .Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    })
                    .ToList()
            };
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_StatusModel model)
        {
            if (model.StatusKey != "Project") { ModelState.Remove("Project_SuccessRate"); }
            else
            {
                if (model.Project_SuccessRate < 0 || model.Project_SuccessRate > 100)
                {
                    ModelState.AddModelError("Project_SuccessRate",  $"Vui lòng nhập trong khoảng 0 - 100");
                }
            }
            if (!ModelState.IsValid)
            {
                var config = _configsCache.GetViaKey("STATUS_KEY");

                model.ListStatusKey = config?.ConfigValue?
                        .Split(';')
                        .Select(x => new SelectListItem
                        {
                            Value = x,
                            Text = x
                        })
                        .ToList();
                return PartialView("_Status", model);
            }
            string response;
            var result = _statusCache.Save(model, User.UserName);
            if (result == 0)
                response = CreateMessage($"{_statusTitle} [{model.StatusName}]", EnumProcessType.Add, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_statusTitle} [{model.StatusName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_statusTitle} [{model.StatusName}]", EnumProcessType.Add, EnumMsgIcon.Success);

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);

        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = _statusCache.GetById(id);
            var config = _configsCache.GetViaKey("STATUS_KEY");

            model.ListStatusKey = config?.ConfigValue?
                    .Split(';')
                    .Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    })
                    .ToList();
            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_StatusModel model)
        {
            if (model.StatusKey != "Project") { ModelState.Remove("Project_SuccessRate"); }
            else
            {
                if (model.Project_SuccessRate < 0 || model.Project_SuccessRate > 100)
                {
                    ModelState.AddModelError("Project_SuccessRate", $"Vui lòng nhập trong khoảng 0 - 100");
                }
            }
            if (!ModelState.IsValid)
            {
                var config = _configsCache.GetViaKey("STATUS_KEY");

                model.ListStatusKey = config?.ConfigValue?
                        .Split(';')
                        .Select(x => new SelectListItem
                        {
                            Value = x,
                            Text = x
                        })
                        .ToList();
                return PartialView("_Status", model);
            }
            string response;
            var result = _statusCache.Save(model, User.UserName);
            if (result == 0)
                response = CreateMessage($"{_statusTitle} [{model.StatusName}]", EnumProcessType.Edit, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_statusTitle} [{model.StatusName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_statusTitle} [{model.StatusName}]", EnumProcessType.Edit, EnumMsgIcon.Success);

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _statusCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_statusTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmStatus = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_statusTitle} [{model.StatusName}]");

            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_StatusModel model)
        {
            var deleted = _statusCache.Delete(model, User.UserName);
            var response = CreateMessage($"{_statusTitle} [{model.StatusName}]",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion

    }
}