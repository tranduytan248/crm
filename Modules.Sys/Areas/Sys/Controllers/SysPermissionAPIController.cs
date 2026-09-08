using System.Web.Mvc;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using Core.Sys.Models.Sys;
using TSFramework.Core.Enums;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Helpers;
using TSFramework.Libs.Processors;

namespace Modules.Sys.Areas.Sys.Controllers
{
    public class SysPermissionAPIController : AppController
    {
        private readonly string _sysPermissionAPI = AppProcessor.Messagor.GetMessage("SysPermissionAPI_Title");
        private readonly SysPermissionAPICache _sysPermissionAPICache = new SysPermissionAPICache();
        private readonly SysUserCache _sysUserCache = new SysUserCache();


        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult Add(int id = 0)
        {
            var permissionApi = _sysPermissionAPICache.GetById(id);
            var userInfo = _sysUserCache.GetById(id);

            var model = new SysPermissionAPIModel
            {
                ApplyFor = permissionApi != null ? permissionApi.ApplyFor : EnumHelper.GetDescription(EnumApplyFor.DVLH),
                ThongTinUser = userInfo,
                UserID = userInfo.UserId,
                UserName = userInfo.UserName
            };
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(SysPermissionAPIModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_SysPermissionAPI", model);
            }

            string response;
            var data = _sysPermissionAPICache.Save(new SysPermissionAPIModel
            {
                UserID = model.UserID,
                UserName = model.UserName,
                ApplyFor = model.ApplyFor
            });

            if (data == 0)
                response = CreateMessage($"{_sysPermissionAPI} [{model.ThongTinUser.UserName}]",
                    EnumProcessType.Add,
                    EnumMsgIcon.Error);
            else if (data == -9)
                response = CreateMessage($"{_sysPermissionAPI} [{model.ThongTinUser.UserName} ]",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error
                );
            else
                response = CreateMessage($"{_sysPermissionAPI} [{model.ThongTinUser.UserName}]",
                    EnumProcessType.Add,
                    EnumMsgIcon.Success
                );

            return Json(new { status = true, message = response });
        }
    }
}