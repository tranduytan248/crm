using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using System;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class DigitalSalesWorkflowController : AppController
    {
        private readonly RM_DigitalSalesWorkflowCache _workflowCache;
        private readonly string _titleStatus = "Trạng thái";
        private readonly string _titleProcess = "Quy trình";
        private readonly string _titleProgress = "Tiến trình";

        public DigitalSalesWorkflowController()
        {
            _workflowCache = new RM_DigitalSalesWorkflowCache();
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index(byte? businessType)
        {
            var type = businessType ?? 1;
            var data = _workflowCache.GetAllStatuses(type);
            ViewBag.BusinessType = type;
            ViewBag.Title = "Cấu hình Danh mục Trạng thái - Quy trình - Tiến trình";
            return View(data);
        }

        #region 1. Trạng Thái (Status)
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetStatuses(byte? businessType)
        {
            var type = businessType ?? 1;
            var data = _workflowCache.GetAllStatuses(type);
            ViewBag.BusinessType = type;
            return PartialView("_StatusList", data);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult AddStatus(byte? businessType)
        {
            var model = new RM_DigitalSalesStatusModel
            {
                BusinessType = businessType.GetValueOrDefault(1),
                IsActive = true,
                SortOrder = 1
            };
            return PartialView("_StatusModal", model);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditStatus(int id)
        {
            var model = _workflowCache.GetStatusByID(id);
            if (model == null)
            {
                return Json(new { status = false, success = false, message = CreateMessage($"{_titleStatus}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) }, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_StatusModal", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create | EnumActionType.Edit)]
        public ActionResult SaveStatus(RM_DigitalSalesStatusModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, success = false, message = "Vui lòng nhập đầy đủ thông tin bắt buộc." });
            }

            var result = _workflowCache.SaveStatus(model, User.UserName);
            if (result > 0)
            {
                var procType = model.StatusID == 0 ? EnumProcessType.Add : EnumProcessType.Edit;
                return Json(new
                {
                    status = true,
                    success = true,
                    businessType = model.BusinessType,
                    statusId = result,
                    message = CreateMessage($"{_titleStatus} [{model.StatusName}]", procType, EnumMsgIcon.Success)
                });
            }
            else if (result == -9)
            {
                return Json(new
                {
                    status = false,
                    success = false,
                    message = CreateMessage($"{_titleStatus} [{model.StatusName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error)
                });
            }
            var errProcType = model.StatusID == 0 ? EnumProcessType.Add : EnumProcessType.Edit;
            return Json(new { status = false, success = false, message = CreateMessage($"{_titleStatus}", errProcType, EnumMsgIcon.Error) });
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteStatus(int id)
        {
            var model = _workflowCache.GetStatusByID(id);
            if (model == null)
            {
                return Json(new { status = false, success = false, message = CreateMessage($"{_titleStatus}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.ConfirmMessage = $"Bạn có chắc chắn muốn xóa Trạng thái <strong>[{model.StatusName}]</strong> không?<br/><small class='text-danger'>Lưu ý: Các quy trình và tiến trình thuộc trạng thái này cũng sẽ bị ảnh hưởng.</small>";
            ViewBag.TargetType = "Status";
            ViewBag.TargetID = model.StatusID;
            ViewBag.TargetName = model.StatusName;
            ViewBag.DeleteAction = "DeleteStatus";
            return PartialView("_DeleteConfirm");
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteStatus(int id, string dummy = null)
        {
            var status = _workflowCache.GetStatusByID(id);
            if (status == null)
            {
                return Json(new { status = false, success = false, message = "Trạng thái không tồn tại." });
            }

            var result = _workflowCache.DeleteStatus(id, User.UserName);
            if (result > 0)
            {
                return Json(new
                {
                    status = true,
                    success = true,
                    businessType = status.BusinessType,
                    message = CreateMessage($"{_titleStatus} [{status.StatusName}]", EnumProcessType.Delete, EnumMsgIcon.Success)
                });
            }
            return Json(new { status = false, success = false, message = CreateMessage($"{_titleStatus}", EnumProcessType.Delete, EnumMsgIcon.Error) });
        }
        #endregion

        #region 2. Quy Trình (Process)
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetProcesses(int statusId)
        {
            var status = _workflowCache.GetStatusByID(statusId);
            var data = _workflowCache.GetProcesses(out int total, search: null, businessType: null, statusId: statusId, order: "0", orderDir: "ASC", pageIndex: 0, pageSize: 200);
            ViewBag.Status = status;
            ViewBag.StatusID = statusId;
            return PartialView("_ProcessList", data);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult AddProcess(int statusId)
        {
            var status = _workflowCache.GetStatusByID(statusId);
            var model = new RM_DigitalSalesProcessModel
            {
                StatusID = statusId,
                StatusName = status?.StatusName,
                BusinessType = status?.BusinessType ?? 1,
                IsActive = true,
                SortOrder = 1
            };
            return PartialView("_ProcessModal", model);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditProcess(int id)
        {
            var model = _workflowCache.GetProcessByID(id);
            if (model == null)
            {
                return Json(new { status = false, success = false, message = CreateMessage($"{_titleProcess}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) }, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_ProcessModal", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create | EnumActionType.Edit)]
        public ActionResult SaveProcess(RM_DigitalSalesProcessModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, success = false, message = "Vui lòng nhập đầy đủ thông tin bắt buộc." });
            }

            var result = _workflowCache.SaveProcess(model, User.UserName);
            if (result > 0)
            {
                var procType = model.ProcessID == 0 ? EnumProcessType.Add : EnumProcessType.Edit;
                return Json(new
                {
                    status = true,
                    success = true,
                    statusId = model.StatusID,
                    processId = result,
                    message = CreateMessage($"{_titleProcess} [{model.ProcessName}]", procType, EnumMsgIcon.Success)
                });
            }
            else if (result == -9)
            {
                return Json(new
                {
                    status = false,
                    success = false,
                    message = CreateMessage($"{_titleProcess} [{model.ProcessName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error)
                });
            }
            var errProcType = model.ProcessID == 0 ? EnumProcessType.Add : EnumProcessType.Edit;
            return Json(new { status = false, success = false, message = CreateMessage($"{_titleProcess}", errProcType, EnumMsgIcon.Error) });
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteProcess(int id)
        {
            var model = _workflowCache.GetProcessByID(id);
            if (model == null)
            {
                return Json(new { status = false, success = false, message = CreateMessage($"{_titleProcess}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.ConfirmMessage = $"Bạn có chắc chắn muốn xóa Quy trình <strong>[{model.ProcessName}]</strong> không?<br/><small class='text-danger'>Lưu ý: Các tiến trình thuộc quy trình này cũng sẽ bị ảnh hưởng.</small>";
            ViewBag.TargetType = "Process";
            ViewBag.TargetID = model.ProcessID;
            ViewBag.TargetName = model.ProcessName;
            ViewBag.ParentID = model.StatusID;
            ViewBag.DeleteAction = "DeleteProcess";
            return PartialView("_DeleteConfirm");
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteProcess(int id, string dummy = null)
        {
            var process = _workflowCache.GetProcessByID(id);
            if (process == null)
            {
                return Json(new { status = false, success = false, message = "Quy trình không tồn tại." });
            }

            var result = _workflowCache.DeleteProcess(id, User.UserName);
            if (result > 0)
            {
                return Json(new
                {
                    status = true,
                    success = true,
                    statusId = process.StatusID,
                    message = CreateMessage($"{_titleProcess} [{process.ProcessName}]", EnumProcessType.Delete, EnumMsgIcon.Success)
                });
            }
            return Json(new { status = false, success = false, message = CreateMessage($"{_titleProcess}", EnumProcessType.Delete, EnumMsgIcon.Error) });
        }
        #endregion

        #region 3. Tiến Trình (Progress)
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetProgresses(int processId)
        {
            var process = _workflowCache.GetProcessByID(processId);
            var data = _workflowCache.GetProgressesByProcess(processId);
            ViewBag.Process = process;
            ViewBag.ProcessID = processId;
            return PartialView("_ProgressList", data);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult AddProgress(int processId)
        {
            var process = _workflowCache.GetProcessByID(processId);
            var model = new RM_DigitalSalesProgressModel
            {
                ProcessID = processId,
                ProcessName = process?.ProcessName,
                DefaultDurationDays = 3,
                IsActive = true,
                SortOrder = 1
            };
            return PartialView("_ProgressModal", model);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditProgress(int id)
        {
            var model = _workflowCache.GetProgressByID(id);
            if (model == null)
            {
                return Json(new { status = false, success = false, message = CreateMessage($"{_titleProgress}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) }, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_ProgressModal", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create | EnumActionType.Edit)]
        public ActionResult SaveProgress(RM_DigitalSalesProgressModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, success = false, message = "Vui lòng nhập đầy đủ thông tin bắt buộc." });
            }

            var result = _workflowCache.SaveProgress(model, User.UserName);
            if (result > 0)
            {
                var procType = model.ProgressID == 0 ? EnumProcessType.Add : EnumProcessType.Edit;
                return Json(new
                {
                    status = true,
                    success = true,
                    processId = model.ProcessID,
                    progressId = result,
                    message = CreateMessage($"{_titleProgress} [{model.ProgressName}]", procType, EnumMsgIcon.Success)
                });
            }
            else if (result == -9)
            {
                return Json(new
                {
                    status = false,
                    success = false,
                    message = CreateMessage($"{_titleProgress} [{model.ProgressName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error)
                });
            }
            var errProcType = model.ProgressID == 0 ? EnumProcessType.Add : EnumProcessType.Edit;
            return Json(new { status = false, success = false, message = CreateMessage($"{_titleProgress}", errProcType, EnumMsgIcon.Error) });
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteProgress(int id)
        {
            var model = _workflowCache.GetProgressByID(id);
            if (model == null)
            {
                return Json(new { status = false, success = false, message = CreateMessage($"{_titleProgress}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.ConfirmMessage = $"Bạn có chắc chắn muốn xóa Tiến trình <strong>[{model.ProgressName}]</strong> không?";
            ViewBag.TargetType = "Progress";
            ViewBag.TargetID = model.ProgressID;
            ViewBag.TargetName = model.ProgressName;
            ViewBag.ParentID = model.ProcessID;
            ViewBag.DeleteAction = "DeleteProgress";
            return PartialView("_DeleteConfirm");
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteProgress(int id, string dummy = null)
        {
            var progress = _workflowCache.GetProgressByID(id);
            if (progress == null)
            {
                return Json(new { status = false, success = false, message = "Tiến trình không tồn tại." });
            }

            var result = _workflowCache.DeleteProgress(id, User.UserName);
            if (result > 0)
            {
                return Json(new
                {
                    status = true,
                    success = true,
                    processId = progress.ProcessID,
                    message = CreateMessage($"{_titleProgress} [{progress.ProgressName}]", EnumProcessType.Delete, EnumMsgIcon.Success)
                });
            }
            return Json(new { status = false, success = false, message = CreateMessage($"{_titleProgress}", EnumProcessType.Delete, EnumMsgIcon.Error) });
        }
        #endregion
    }
}
