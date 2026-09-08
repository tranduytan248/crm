using System;
using System.Collections.Generic;
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
    public class RevenueAllocationController : AppController
    {
        private const string RevenueNotEnoughMessageKey = "RevenueAllocation_Message_RevenueNotEnough";
        private const string InvalidKeyMessageKey = "RevenueAllocation_Message_InvalidKey";
        private const string DuplicateMessageKey = "RevenueAllocation_Message_Duplicate";
        private const string SaveFailedMessageKey = "RevenueAllocation_Message_SaveFailed";

        private readonly RM_RevenueAllocationCache _cache;
        private readonly RM_RevenueReceivedCache _revenueReceivedCache;
        private readonly string _title = AppProcessor.Messagor.GetMessage("RevenueAllocation_Title");
        private readonly string _revenueNotEnoughMessage = AppProcessor.Messagor.GetMessage(RevenueNotEnoughMessageKey);
        private readonly string _invalidKeyMessage = AppProcessor.Messagor.GetMessage(InvalidKeyMessageKey);
        private readonly string _duplicateMessage = AppProcessor.Messagor.GetMessage(DuplicateMessageKey);
        private readonly string _saveFailedMessage = AppProcessor.Messagor.GetMessage(SaveFailedMessageKey);

        public RevenueAllocationController()
        {
            _cache = new RM_RevenueAllocationCache();
            _revenueReceivedCache = new RM_RevenueReceivedCache();
        }

        /// <summary>
        /// Hiển thị popup phân bổ doanh thu cho sản phẩm dự án được chọn.
        /// </summary>
        [AjaxOnly]
        [ActionType(Type = EnumActionType.Edit)]
        [HttpGet]
        public ActionResult Allocate(int id = 0)
        {
            try
            {
                if (!HasEnoughRevenue(id))
                {
                    var response = CreateMessage(_revenueNotEnoughMessage, EnumProcessType.NonFormat, EnumMsgIcon.Error);
                    return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
                }

                ViewBag.ProjectID = id;

                var data = _cache.GetByProductProjectID(id);
                return PartialView("_RevenueAllocation", data ?? new List<RM_RevenueAllocationModel>());
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);

                var response = CreateMessage(_title, EnumProcessType.Edit, EnumMsgIcon.Error);
                return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Lưu tỷ lệ phân bổ doanh thu cho danh sách thành viên dự án.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Allocate(int projectId = 0, List<RM_RevenueAllocationModel> models = null)
        {
            try
            {
                if (models == null || models.Count == 0)
                {
                    var response = CreateMessage(_title, EnumProcessType.Edit, EnumMsgIcon.Error);
                    return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
                }

                var productProjectId = projectId > 0
                    ? projectId
                    : models.FirstOrDefault()?.ProjectID ?? 0;

                if (!HasEnoughRevenue(productProjectId))
                {
                    var response = CreateMessage(_revenueNotEnoughMessage, EnumProcessType.NonFormat, EnumMsgIcon.Error);
                    return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
                }

                var hasInvalidRate = models.Any(item => item == null || item.AllocationRate < 0 || item.AllocationRate > 100);
                if (hasInvalidRate)
                {
                    var response = CreateMessage(_title, EnumProcessType.Edit, EnumMsgIcon.Error);
                    return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
                }

                var hasInvalidKey = models.Any(item =>
                    item == null
                    || item.ProjectMemberID <= 0
                    || item.RevenueReceivedID <= 0);

                if (hasInvalidKey)
                {
                    var response = CreateMessage(_invalidKeyMessage, EnumProcessType.Edit, EnumMsgIcon.Error);
                    return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
                }

                var totalRate = models.Sum(item => item?.AllocationRate ?? 0);
                if (totalRate > 100)
                {
                    var response = CreateMessage(_title, EnumProcessType.Edit, EnumMsgIcon.Error);
                    return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
                }

                var processed = 0;
                var successCount = 0;
                var duplicateCount = 0;

                foreach (var model in models)
                {
                    if (model == null)
                    {
                        continue;
                    }

                    processed++;

                    var result = _cache.Save(model, User.UserName);
                    if (result > 0)
                    {
                        successCount++;
                        continue;
                    }

                    if (result == -9)
                    {
                        duplicateCount++;
                    }
                }

                if (processed > 0 && successCount == processed)
                {
                    var response = CreateMessage(_title, EnumProcessType.Edit, EnumMsgIcon.Success)
                        + "reloadRevenueAllocation();";

                    return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
                }

                if (duplicateCount > 0)
                {
                    var response = CreateMessage(_duplicateMessage, EnumProcessType.Edit, EnumMsgIcon.Error);
                    return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
                }

                return Json(
                    new
                    {
                        status = false,
                        message = CreateMessage(_saveFailedMessage, EnumProcessType.Edit, EnumMsgIcon.Error)
                    },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);

                var response = CreateMessage(_title, EnumProcessType.Edit, EnumMsgIcon.Error);
                return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Kiểm tra sản phẩm dự án có doanh thu thực nhận đủ điều kiện để phân bổ hay không.
        /// </summary>
        private bool HasEnoughRevenue(int productProjectId)
        {
            if (productProjectId <= 0)
            {
                return false;
            }

            var revenueReceived = _revenueReceivedCache.GetByProductProjectID(productProjectId);
            if (revenueReceived == null || revenueReceived.Count == 0)
            {
                return false;
            }

            var totalRevenue = revenueReceived.Sum(item => item?.Amount ?? 0);
            return totalRevenue > 0;
        }
    }
}
