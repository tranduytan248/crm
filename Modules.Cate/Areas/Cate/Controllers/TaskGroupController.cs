using System;
using System.Web.Mvc;
using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class TaskGroupController : AppController
    {
        private readonly RM_TaskGroupCache _cache;
        private readonly string _Title = AppProcessor.Messagor.GetMessage("TaskGroup_Title");

        public TaskGroupController()
        {
            _cache = new RM_TaskGroupCache();
        }

        // GET: Cate/TaskGroup
        public ActionResult Index() => View();

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get()
        {
            var draw = Request.Form.GetValues("draw")?[0];
            var order = Request.Form.GetValues("order[0][column]")?[0];
            var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0]);

            var dataSearch = new BaseSearchModel
            {
                Order = order,
                OrderDir = orderDir,
                StartIndex = startRec,
                PageSize = pageSize
            };

            var data = _cache.Get(out var total, dataSearch);
            return Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
        }

        // ADD
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add() => PartialView("_Add", new RM_TaskGroupModel());

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(RM_TaskGroupModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_TaskGroup", model);

            string response;
            var result = _cache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_Title} [{model.TaskGroupName}]", EnumProcessType.Add, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_Title} [{model.TaskGroupName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_Title} [{model.TaskGroupName}]", EnumProcessType.Add, EnumMsgIcon.Success);

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        // EDIT
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _cache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_Title}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(RM_TaskGroupModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_TaskGroup", model);

            string response;
            var result = _cache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_Title} [{model.TaskGroupName}]", EnumProcessType.Edit, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_Title} [{model.TaskGroupName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_Title} [{model.TaskGroupName}]", EnumProcessType.Edit, EnumMsgIcon.Success);

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        // DELETE
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _cache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_Title}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_Title} [{model.TaskGroupName}]");
            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_TaskGroupModel model)
        {
            var deleted = _cache.Delete(model, User.UserName);
            var response = CreateMessage($"{_Title} [{model.TaskGroupName}]",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
    }
}