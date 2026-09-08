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
    public class AnniversaryTypeController : AppController
    {
        private readonly RM_AnniversaryTypeCache _AnniversaryTypeCache;
        private readonly RM_ProductProjectCache _productProjectCache;
        private readonly RM_CostTypeCache _costTypeCache;
        private readonly string _AnniversaryTypeTitle = AppProcessor.Messagor.GetMessage("AnniversaryType_Title");
        public AnniversaryTypeController() {
            _AnniversaryTypeCache = new RM_AnniversaryTypeCache();
            _productProjectCache = new RM_ProductProjectCache();
            _costTypeCache = new RM_CostTypeCache();
        }

        // GET: Cate/AnniversaryType
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
            var data = _AnniversaryTypeCache.Get(out var total, dataSearch);
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult Add(int? id)
        {
            var model = new RM_AnniversaryTypeModel();
            if (id.HasValue)
            {
                model.AnniversaryType_ID = id.Value;
            }
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_AnniversaryTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_AnniversaryType", model);
            }

            string response;

            model.CodeAnniversaryType = model.CodeAnniversaryType.ToUpper();
            var result = _AnniversaryTypeCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_AnniversaryTypeTitle} [{model.NameAnniversaryType}]",
                    EnumProcessType.Add,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_AnniversaryTypeTitle} [{model.NameAnniversaryType}]",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_AnniversaryTypeTitle} [{model.NameAnniversaryType}]",
                    EnumProcessType.Add,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _AnniversaryTypeCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_AnniversaryTypeTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_AnniversaryTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_AnniversaryType", model);
            }
            string response;

            model.CodeAnniversaryType = model.CodeAnniversaryType.ToUpper();
            var result = _AnniversaryTypeCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_AnniversaryTypeTitle} [{model.NameAnniversaryType}]",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_AnniversaryTypeTitle} [{model.NameAnniversaryType}]",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_AnniversaryTypeTitle} [{model.NameAnniversaryType}]",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _AnniversaryTypeCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_AnniversaryTypeTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_AnniversaryTypeTitle} [{model.NameAnniversaryType}]");

            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_AnniversaryTypeModel model)
        {
            var deleted = _AnniversaryTypeCache.Delete(model, User.UserName);

            var response = CreateMessage($"{_AnniversaryTypeTitle} [{model.NameAnniversaryType}]",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
    }
}