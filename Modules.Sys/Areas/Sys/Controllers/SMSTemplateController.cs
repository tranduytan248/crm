using System;
using System.Web.Mvc;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Sys.Areas.Sys.Controllers
{
    public class SMSTemplateController : AppController
    {
        private readonly string _funcName = AppProcessor.Messagor.GetMessage("SMSTemplate_Title");
        private readonly SysSMSTemplateCache _sysSMSTemplateCache = new SysSMSTemplateCache();

        // GET: 
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
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
            var data = _sysSMSTemplateCache.Get(out int total, dataSearch);

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
            var model = new SysSMSTemplateModel();
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(SysSMSTemplateModel model)
        {
            if (!ModelState.IsValid) return PartialView("_SMSTemplate", model);
            string response;
            var configId = _sysSMSTemplateCache.Save(model, User.UserName);

            if (configId == 0)
                response = CreateMessage($"{_funcName} [{model.TemplateName}]", EnumProcessType.Add, EnumMsgIcon.Success);
            else if (configId == -2)
                response = CreateMessage($"{_funcName} [ {model.TemplateName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
            {
                //SaveRefFiles(model.RefFile, configId);
                response = CreateMessage($"{_funcName} [ {model.TemplateName}]", EnumProcessType.Add, EnumMsgIcon.Success);
            }

            return Json(new
            {
                status = true,
                message = response
            }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _sysSMSTemplateCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(SysSMSTemplateModel model)
        {
            if (!ModelState.IsValid) return PartialView("_SMSTemplate", model);

            var configId = _sysSMSTemplateCache.Save(model, User.UserName);

            string response;
            if (configId == 0)
                response = CreateMessage($"{_funcName} [{model.TemplateName}]", EnumProcessType.Add, EnumMsgIcon.Success);
            else if (configId == -2)
                response = CreateMessage($"{_funcName} [ {model.TemplateName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
            {
                response = CreateMessage($"{_funcName} [ {model.TemplateName}]", EnumProcessType.Add, EnumMsgIcon.Success);
            }

            return Json(new
            {
                status = true,
                message = response
            }, JsonRequestBehavior.AllowGet);
        }


        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id = 0)
        {
            var model = _sysSMSTemplateCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"<b>{_funcName} [{model.TemplateName}]</b>");

            return PartialView("_Delete", model);
        }

        //[AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(SysSMSTemplateModel model)
        {
            var delete = _sysSMSTemplateCache.GetById(model.SMSTemplateID);
            _sysSMSTemplateCache.GetByCode("");
            var deleted = _sysSMSTemplateCache.Delete(delete, User.UserName);

            var response = CreateMessage($"{_funcName} [{model.TemplateName}]", EnumProcessType.Delete,
                deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
    }
}