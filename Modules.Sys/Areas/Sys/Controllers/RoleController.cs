using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Helpers;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Modules.Sys.Areas.Sys.Controllers
{
    public class RoleController : AppController
    {
        private readonly string _funcName = AppProcessor.Messagor.GetMessage("Role_Title");
        private readonly SysFunctionCache _functionCache = new SysFunctionCache();
        private readonly SysPermissionCache _permissionCache = new SysPermissionCache();
        private readonly SysRoleCache _roleCache = new SysRoleCache();

        // GET: Role
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
            var data = _roleCache.Get(out int total, dataSearch);

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
            var model = new SysRoleModel();
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(SysRoleModel model)
        {
            if (!ModelState.IsValid) return PartialView("_Role", model);

            var idRole = _roleCache.Save(new SysRoleModel
            {
                RoleId = 0,
                Name = model.Name,
                IsDeleted = false
            });

            var response = CreateMessage($"{_funcName} [{model.Name}]", EnumProcessType.Add,
                idRole > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id = 0)
        {
            var model = _roleCache.GetById(id);
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
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(SysRoleModel model)
        {
            if (!ModelState.IsValid) return PartialView("_Role", model);
            var idRole = _roleCache.Save(new SysRoleModel
            {
                RoleId = model.RoleId,
                Name = model.Name,
                IsDeleted = false
            });

            var response = CreateMessage($"{_funcName} [{model.Name}]", EnumProcessType.Edit,
                idRole > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id = 0)
        {
            var model = _roleCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"<b>{_funcName} [{model.Name}]</b>");
            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(SysRoleModel model)
        {
            var deleted = _roleCache.Delete(model);

            var response = CreateMessage($"{_funcName} [{model.Name}]", EnumProcessType.Delete,
                deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit | EnumActionType.Create)]
        public ActionResult Permission(int id = 0)
        {
            var role = _roleCache.GetById(id);
            if (role == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var functions = _functionCache.GetAll();
            foreach (var f in functions)
            {
                f.Actions = CreateListItemAction();
                f.SelectedActions = _functionCache.GetActions(f.FunctionId);
            }

            role.Permissions = string.Join(",",
                _permissionCache.GetByRoleId(role.RoleId).Select(p => $"{p.FunctionId}.{p.Action}"));
            role.Functions = functions;

            return PartialView("_Permission", role);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Edit | EnumActionType.Create)]
        public ActionResult Permission(SysRoleModel model)
        {
            if (!ModelState.IsValid) return PartialView("_FunctionAction", model);
            model.Permissions = string.IsNullOrEmpty(model.Permissions) ? null : model.Permissions.Trim(',');

            var success = _permissionCache.Save(model.RoleId,
                UtilString.SplitToTable(model.Permissions, new[] { ',', '.' }), ",");
            
            var response = CreateMessage($"{_funcName} [{model.Name}]", EnumProcessType.Edit,
                success ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [NonAction]
        private List<ListItem> CreateListItemAction()
        {
            return new List<ListItem>
            {
                new ListItem
                {
                    Text = EnumHelper.GetDescription(EnumActionType.Create),
                    Value = EnumHelper.GetDescription(EnumActionType.Create)
                },
                new ListItem
                {
                    Text = EnumHelper.GetDescription(EnumActionType.Delete),
                    Value = EnumHelper.GetDescription(EnumActionType.Delete)
                },
                new ListItem
                {
                    Text = EnumHelper.GetDescription(EnumActionType.Edit),
                    Value = EnumHelper.GetDescription(EnumActionType.Edit)
                },
                new ListItem
                {
                    Text = EnumHelper.GetDescription(EnumActionType.View),
                    Value = EnumHelper.GetDescription(EnumActionType.View)
                }
            };
        }
    }
}