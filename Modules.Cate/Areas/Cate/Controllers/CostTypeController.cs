using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using System;
using System.Linq;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class CostTypeController : AppController
    {
        private readonly RM_CostTypeCache _costTypeCache;
        private readonly RM_CustomerCache _customerCache;
        private readonly string _CostTypeTitle = AppProcessor.Messagor.GetMessage("CostType_Title");
        public CostTypeController() {
            _costTypeCache = new RM_CostTypeCache();
            _customerCache = new RM_CustomerCache();
        }

        // GET: Cate/CostType
        public ActionResult Index()
        {
            return View();
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get()
        {
            var search = Request.Form.GetValues("search[value]")?[0];
            var draw = Request.Form.GetValues("draw")?[0];
            var order = Request.Form.GetValues("order[0][column]")?[0];
            var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0]);
            var dataSearch = new BaseSearchModel
            {
                Search = string.IsNullOrEmpty(search) ? null : search,
                Order = order,
                OrderDir = orderDir,
                StartIndex = startRec,
                PageSize = pageSize
            };
            var data = _costTypeCache.Get(out var total, dataSearch);
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
            var model = new RM_CostTypeModel();
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_CostTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CostType", model);
            }

            string response;

            var result = _costTypeCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_CostTypeTitle} [{model.CostTypeName}]",
                    EnumProcessType.Add,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_CostTypeTitle} [{model.CostTypeName}]",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_CostTypeTitle} [{model.CostTypeName}]",
                    EnumProcessType.Add,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _costTypeCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_CostTypeTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_CostTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CostType", model);
            }
            string response;

            var result = _costTypeCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_CostTypeTitle} [{model.CostTypeName}]",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_CostTypeTitle} [{model.CostTypeName}]",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_CostTypeTitle} [{model.CostTypeName}]",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _costTypeCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_CostTypeTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_CostTypeTitle} [{model.CostTypeName}]");

            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_CostTypeModel model)
        {
            var deleted = _costTypeCache.Delete(model, User.UserName);

            var response = CreateMessage($"{_CostTypeTitle} [{model.CostTypeName}]",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
    }
}