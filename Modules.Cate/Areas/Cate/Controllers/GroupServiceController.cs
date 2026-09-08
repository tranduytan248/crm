using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web;
using TSFramework.Libs.Models.Base;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;
using System.Web.Caching;
using DocumentFormat.OpenXml.EMMA;
using System.Collections.Generic;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class GroupServiceController : AppController
    {
        private readonly RM_GroupServiceCache _groupServiceCache;
        private readonly string _groupName = AppProcessor.Messagor.GetMessage("GroupService_Title");

        public GroupServiceController()
        {
            _groupServiceCache = new RM_GroupServiceCache();
        }
        private List<SelectListItem> BuildTreeDropdown(List<RM_GroupServiceModel> list)
        {
            var result = new List<SelectListItem>();

            void Build(int? parentId, int level)
            {
                var children = list
                    .Where(x => x.ParentGroupServiceID == parentId)
                    .OrderBy(x => x.GroupServiceID)
                    .ToList();

                foreach (var item in children)
                {
                    result.Add(new SelectListItem
                    {
                        Value = item.GroupServiceID.ToString(),
                        Text = new string('-', level * 2) + " " + item.NameGroup
                    });

                    Build(item.GroupServiceID, level + 1);
                }
            }

            Build(null, 0); 

            return result;
        }

        private List<SelectListItem> BuildParentGroupServiceList(int? id)
        {
            var list = _groupServiceCache.GetAll()
            .Where(x => !x.IsDeleted) 
            .ToList();

                // nếu create mới
                if (id == null)
                {
                    return BuildTreeDropdown(list);
                }

                // lấy danh sách cần loại
                var excludeIds = GetAllChildIds(list, id.Value);
                excludeIds.Add(id.Value);

                var filtered = list
                    .Where(x => !excludeIds.Contains(x.GroupServiceID))
                    .ToList();

                return BuildTreeDropdown(filtered);
        }

        private List<int> GetAllChildIds(List<RM_GroupServiceModel> list, int parentId)
        {
            var result = new List<int>();

            var children = list
                .Where(x => x.ParentGroupServiceID == parentId)
                .ToList();

            foreach (var child in children)
            {
                result.Add(child.GroupServiceID);
                result.AddRange(GetAllChildIds(list, child.GroupServiceID));
            }

            return result;
        }

        // GET: Cate/GroupService
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Lấy danh sách GroupService dạng Tree
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(string search)
        {
            var data = _groupServiceCache.Get(search);

            return Json(new
            {
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xử lý hiện giao diện
        /// </summary>
        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult Add(int? id)
        {
            var model = new RM_GroupServiceModel
            {
                IsActived = true
            };

            ViewBag.ParentList = BuildParentGroupServiceList(null);

            return PartialView("_Add", model);
        }

        /// <summary>
        /// Xử lý Chức năng
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_GroupServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentList = BuildParentGroupServiceList(null);

                return PartialView("_GroupService", model);
            }
            string response;
            var result = _groupServiceCache.Save(model, User.UserName);
            if (result == 0)
                response = CreateMessage($"{_groupName} [{model.NameGroup}]", EnumProcessType.Add, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_groupName} [{model.NameGroup}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_groupName} [{model.NameGroup}]", EnumProcessType.Add, EnumMsgIcon.Success);

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xử lý hiện giao diện
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _groupServiceCache.GetById(id);
            var list = _groupServiceCache.GetAll();

            ViewBag.ParentList = BuildParentGroupServiceList(id);

            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_GroupServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentList = BuildParentGroupServiceList(null);

                return PartialView("_GroupService", model);
            }
            string response;
            var result = _groupServiceCache.Save(model, User.UserName);
            if (result == 0)
                response = CreateMessage($"{_groupName} [{model.NameGroup}]", EnumProcessType.Edit, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_groupName} [{model.NameGroup}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_groupName} [{model.NameGroup}]", EnumProcessType.Edit, EnumMsgIcon.Success);

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xử lý hiện giao diện
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _groupServiceCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_groupName}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_groupName} [{model.NameGroup}]");

            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_GroupServiceModel model)
        {
            var deleted = _groupServiceCache.Delete(model, User.UserName);
            var response = CreateMessage($"{_groupName} [{model.NameGroup}]",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
    }
}