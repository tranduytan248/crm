using Core.Sys.BaseApp;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Attributes;
using Core.Cate.Caches;
using Core.Cate.Models;
using System.Web.Razor.Tokenizer.Symbols;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;
using System.Web.Services.Description;
using DocumentFormat.OpenXml.Office2010.Excel;
using Core.Sys.Caches.Sys;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using System.Web.Caching;
using Core.Sys.Models.Sys;
using DocumentFormat.OpenXml.EMMA;
using Core.Cate.Services;
using Core.Services;


namespace Modules.Cate.Areas.Cate.Controllers
{
    public class TaskManagementController : AppController
    {
        #region Fields & Constructor

        private readonly RM_TaskManagementCache _taskManagementCache;
        private readonly RM_ProjectCache _projectCache;
        private readonly RM_ProductProjectCache _productProjectCache;
        private readonly RM_TaskTypeCache _taskTypeCache;
        private readonly RM_PriorityCache _priorityCache;
        private readonly RM_ProjectMemberCache _projectMemberCache;
        private readonly RM_StatusCache _taskStatusCache;
        private readonly SysUserCache _sysUserCache;
        private readonly RM_TaskAssigneeCache _taskAssigneeCache;
        private readonly SysConfigCache _configsCache;
        private readonly TaskActivityService _taskActivityService;
        private readonly ProjectTaskMailService _projectTaskMailService;
        private readonly TaskFileService _fileService;


        private readonly string _TaskTitle = AppProcessor.Messagor.GetMessage("Task_TaskTitle");

        public TaskManagementController()
        {
            _taskManagementCache = new RM_TaskManagementCache();
            _projectCache = new RM_ProjectCache();
            _productProjectCache = new RM_ProductProjectCache();
            _taskTypeCache = new RM_TaskTypeCache();
            _priorityCache = new RM_PriorityCache();
            _taskStatusCache = new RM_StatusCache();
            _projectMemberCache = new RM_ProjectMemberCache();
            _sysUserCache = new SysUserCache();
            _taskAssigneeCache = new RM_TaskAssigneeCache();
            _configsCache = new SysConfigCache();
            _fileService = new TaskFileService();
            _projectTaskMailService = new ProjectTaskMailService();
            _taskActivityService = new TaskActivityService();
        }

        #endregion

        #region 

        /// <summary>
        /// Tạo danh sách nhân sự có thể gán việc theo thành viên hiện có của sản phẩm dự án.
        /// </summary>
        //private List<SelectListItem> GetEmployeeSelectList(int productProjectID)
        //{
        //    var allUsers = _sysUserCache.GetAll() ?? new List<SysUserModel>();

        //    return (_projectMemberCache.GetByProductProjectID(productProjectID)
        //            ?? new List<RM_ProjectMemberModel>())
        //        .Select(x =>
        //        {
        //            var user = allUsers.FirstOrDefault(u => u.UserId == x.Employee_ID);

        //            return new SelectListItem
        //            {
        //                Value = x.Employee_ID.ToString(),
        //                Text = x.FullName
        //            };
        //        })
        //        .GroupBy(x => x.Value)
        //        .Select(x => x.First())
        //        .ToList();
        //}

        private void LoadDropdown(RM_TaskManagementModel model)
        {
            model.TaskTypes = _taskTypeCache.GetAll() ?? new List<RM_TaskTypeModel>();

            ViewBag.TaskTypeList = model.TaskTypes.Select(x => new SelectListItem
            {
                Value = x.TaskTypeID.ToString(),
                Text = x.TaskTypeName
            }).ToList();

            model.Priorities = _priorityCache.GetAll() ?? new List<RM_PriorityModel>();

            ViewBag.Priorities = model.Priorities.Select(x => new SelectListItem
            {
                Value = x.PriorityID.ToString(),
                Text = x.PriorityName
            }).ToList();

            model.ListTaskStatus = _taskStatusCache.GetStatusBySearchKey("Task") ?? new List<RM_StatusModel>();

            ViewBag.ListStatus = model.ListTaskStatus.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.StatusName
            }).ToList();

            var allMember = _projectMemberCache.GetAll(model.ProjectID);
            model.ListEmployee = allMember.Select(x=>new SelectListItem { Value = x.Employee_ID.ToString(), Text = x.FullName}).ToList();
        }

        #endregion

        public ActionResult Index(int projectId)
        {
            ViewBag.ProjectId = projectId;
            return PartialView();
        }

        public ActionResult Detail(int id)
        {
            var modal = _taskManagementCache.GetById(id) ?? new RM_TaskManagementModel();
            var assignees = _taskAssigneeCache.GetByTaskManagementID(id);
            modal.AssigneeIDList = assignees
                    .Select(x => x.Employee_ID)
                    .ToList();
            //var Product = _productProjectCache.GetById(modal.ProductProjectID);
            var project = _projectCache.GetById(modal.ProjectID);
            modal.FilePaths = _fileService.GetFilePaths(
                taskManagementId: modal.TaskManagementID
            );
            ViewBag.ProjectId = project.ProjectID;
            ViewBag.ProjectName = project.ProjectName;
            modal.IsEdit = false;

            return View(modal);
        }

        public ActionResult ViewPartial(int id)
        {
            var model = _taskManagementCache.GetById(id);
            return PartialView("_TaskDetailView", model);
        }

        public ActionResult EditPartial(int id)
        {
            var model = _taskManagementCache.GetById(id);
            LoadDropdown(model);

            return PartialView("_TaskDetailForm", model);
        }


        #region TaskManagement - CRUD

        /// <summary>
        /// Trả danh sách công việc theo điều kiện tìm kiếm và phân trang.
        /// </summary>
        /// <param name="searchModel">Điều kiện tìm kiếm công việc.</param>
        /// <returns>Dữ liệu JSON cho lưới danh sách công việc.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(RM_TaskManagementSearchModel searchModel)
        {
            var draw = Request.Form.GetValues("draw")?[0];
            var order = Request.Form.GetValues("order[0][column]")?[0];
            var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0]);
            var keyword = Request.Form.GetValues("filterKeyword")?.FirstOrDefault();

            var dataSearch = new BaseSearchModel
            {
                Search = string.IsNullOrWhiteSpace(keyword) ? null : keyword,
                Order = order,
                OrderDir = string.IsNullOrEmpty(orderDir) ? "DESC" : orderDir.ToUpper(),
                StartIndex = startRec,
                PageSize = pageSize
            };

            var data = _taskManagementCache.Get(out var total, searchModel, dataSearch);
            if (data != null && data.Any())
            {
                var currentUser = User.UserName;
                foreach (var item in data)
                {
                    item.AssigneeNames = item.AssigneeNames ?? "";
                    item.AssigneeAvatars = item.AssigneeAvatars ?? "";
                    item.AssigneeIDs = item.AssigneeIDs ?? "";

                    item.CompletionPercentage = item.CompletionPercentage ?? 0;
                    item.CanDelete = !item.HasComment && string.Equals(item.CreatedBy, currentUser, StringComparison.OrdinalIgnoreCase);
                }
            }
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = total,
                recordsFiltered = total,
                data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(int id, int? parentTaskId)
        {
            var model = new RM_TaskManagementModel
            {
                ProjectID = id,
                IsEdit = false,
                PriorityID = 2,
                ParentTaskID = parentTaskId  // ← thêm dòng này
            };

            //var searchProductProject = new RM_TaskManagementSearchModel
            //{
            //    ProductProjectID = id
            //};
            var ListTask = _taskManagementCache.GetAll(id);

            var taskSelectList = ListTask
                .Select(x => new SelectListItem
                {
                    Value = x.TaskManagementID.ToString(),
                    Text = x.TaskName
                })
                .ToList();

            //var product = _productProjectCache.GetById(id);
            var project = _projectCache.GetById(id);
            ViewBag.ListTask = taskSelectList;
            ViewBag.ProjectId = project.ProjectID;
            ViewBag.ProjectName = project.ProjectName;
            LoadDropdown(model);

            return View("Detail", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_TaskManagementModel model)
        {
            if (!ModelState.IsValid)
            {
                var ListTask = _taskManagementCache.GetAll(model.ProjectID);

                var taskSelectList = ListTask
                    .Select(x => new SelectListItem
                    {
                        Value = x.TaskManagementID.ToString(),
                        Text = x.TaskName
                    })
                    .ToList();
                //var product = _productProjectCache.GetById(model.ProductProjectID);
                var project = _projectCache.GetById(model.ProjectID);
                ViewBag.ListTask = taskSelectList;
                ViewBag.ProjectId = project.ProjectID;
                ViewBag.ProjectName = project.ProjectName;
                LoadDropdown(model);
                return PartialView("_TaskDetailForm", model);
            }
            string response;
            var result = _taskManagementCache.Save(model, User.UserName);
            if (result > 0)
            {
                var employeeIds = model.AssigneeIDList != null ? string.Join(",", model.AssigneeIDList) : string.Empty;

                _taskAssigneeCache.Save(result, employeeIds, User.UserName);

                if (model.AssigneeIDList != null && model.AssigneeIDList.Any())
                {
                    // Người nhận (To)
                    var toUserNames = _sysUserCache.GetAll()
                        .Where(x => x.UserId.HasValue
                            && model.AssigneeIDList.Contains(x.UserId.Value))
                        .Select(x => x.UserName)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    // Người quản lý (CC)
                    var ccUserNames = new List<string>();

                    bool sendMailToManager = bool.TryParse(_configsCache.GetViaKey("CONFIG_SEND_MAIL_TO_MANAGER")?.ConfigValue, out bool enabled) && enabled;

                    if (sendMailToManager)
                    {
                        ccUserNames = (_sysUserCache.GetManagement(employeeIds) ?? Enumerable.Empty<SysUserModel>())
                            .Select(x => x.UserName)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .Except(toUserNames, StringComparer.OrdinalIgnoreCase)
                            .ToList();
                    }

                    if (toUserNames.Any())
                    {
                        _projectTaskMailService.QueueSendProjectTaskCreatedMail(
                            result,
                            toUserNames,
                            User.UserName,
                            ccUserNames);
                    }
                }

                var dbRequest = new SaveFilePathRequest
                {
                    TaskManagementID = result
                };

                _fileService.SaveImagePathsFromHtml(model.Description, dbRequest, User.UserName);

                if (model.Files != null && model.Files.Any())
                {
                    _fileService.UploadAndSave(
                        model.Files,
                        dbRequest,
                        User.UserName,
                        subFolder: "task");
                }
            }

            if (result == 0)
                response = CreateMessage($"{_TaskTitle} [{model.TaskName}]", EnumProcessType.Add, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_TaskTitle} [{model.TaskName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_TaskTitle} [{model.TaskName}]", EnumProcessType.Add, EnumMsgIcon.Success);

            return Json(new { status = true, message = response, projectId = model.ProjectID }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _taskManagementCache.GetById(id);
            var ListTask = _taskManagementCache.GetAll(model.ProjectID);

            var taskSelectList = ListTask
                .Where(x => x.TaskManagementID != model.TaskManagementID
                         && x.ParentTaskID != model.TaskManagementID)
                .Select(x => new SelectListItem
                {
                    Value = x.TaskManagementID.ToString(),
                    Text = x.TaskName
                })
                .ToList();

            var assignees = _taskAssigneeCache.GetByTaskManagementID(id);
            model.FilePaths = _fileService.GetFilePaths(
                taskManagementId: model.TaskManagementID
            );

            model.AssigneeIDList = assignees
                    .Select(x => x.Employee_ID)
                    .ToList();
            model.IsEdit = true;
            if (model == null)
                return Json(new { status = true, message = CreateMessage($"{_TaskTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            // var product = _productProjectCache.GetById(model.ProductProjectID);
            var project = _projectCache.GetById(model.ProjectID);
            ViewBag.ListTask = taskSelectList;
            ViewBag.ProjectId = project.ProjectID;
            ViewBag.ProjectName = project.ProjectName;
            LoadDropdown(model);

            return View("Detail", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_TaskManagementModel model)
        {
            if (!ModelState.IsValid)
            {
                var ListTask = _taskManagementCache.GetAll(model.ProjectID);

                var taskSelectList = ListTask
                    .Where(x => x.TaskManagementID != model.TaskManagementID
                             && x.ParentTaskID != model.TaskManagementID)
                    .Select(x => new SelectListItem
                    {
                        Value = x.TaskManagementID.ToString(),
                        Text = x.TaskName
                    })
                    .ToList();

                var assignees = _taskAssigneeCache.GetByTaskManagementID(model.TaskManagementID);
                model.FilePaths = _fileService.GetFilePaths(
                    taskManagementId: model.TaskManagementID
                );

                model.AssigneeIDList = assignees
                        .Select(x => x.Employee_ID)
                        .ToList();
                model.IsEdit = true;
                if (model == null)
                    return Json(new { status = true, message = CreateMessage($"{_TaskTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
                // var product = _productProjectCache.GetById(model.ProductProjectID);
                var project = _projectCache.GetById(model.ProjectID);
                ViewBag.ListTask = taskSelectList;
                ViewBag.ProjectId = project.ProjectID;
                ViewBag.ProjectName = project.ProjectName;
                LoadDropdown(model);

                return PartialView("_TaskDetailForm", model);
            }
            var oldData = _taskManagementCache.GetById(model.TaskManagementID);
            var oldAssignees = _taskAssigneeCache.GetByTaskManagementID(model.TaskManagementID);
            oldData.AssigneeIDs = string.Join(",", oldAssignees.Select(x => x.Employee_ID));
            oldData.AssigneeNames = string.Join(", ", oldAssignees.Select(x => x.FullName));
            string response;
            var result = _taskManagementCache.Save(model, User.UserName);
            var newData = _taskManagementCache.GetById(result);
            if (result > 0)
            {
                var employeeIds = model.AssigneeIDList != null
                    ? string.Join(",", model.AssigneeIDList)
                    : "";

                _taskAssigneeCache.Save(result, employeeIds, User.UserName);
                var newAssignees = _taskAssigneeCache.GetByTaskManagementID(result);
                newData.AssigneeIDs = string.Join(",", newAssignees.Select(x => x.Employee_ID));
                newData.AssigneeNames = string.Join(", ", newAssignees.Select(x => x.FullName));
                var dbRequest = new SaveFilePathRequest
                {
                    TaskManagementID = result  // ← result là ID mới
                };
                _fileService.SaveImagePathsFromHtml(model.Description, dbRequest, User.UserName);

                var files = model.Files?.Where(f => f != null && f.ContentLength > 0).ToList();
                if (files != null && files.Any())
                    _fileService.UploadAndSave(files, dbRequest, User.UserName, subFolder: "task");

                if (model.AssigneeIDList != null && model.AssigneeIDList.Any())
                {
                    // Người nhận (To)
                    var toUserNames = _sysUserCache.GetAll()
                        .Where(x => x.UserId.HasValue
                            && model.AssigneeIDList.Contains(x.UserId.Value))
                        .Select(x => x.UserName)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    // Thêm người tạo task
                    if (!string.IsNullOrWhiteSpace(oldData.CreatedBy))
                    {
                        toUserNames.Add(oldData.CreatedBy);
                    }

                    toUserNames = toUserNames
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    // Người quản lý (CC)
                    var ccUserNames = new List<string>();

                    bool sendMailToManager = bool.TryParse(_configsCache.GetViaKey("CONFIG_SEND_MAIL_TO_MANAGER")?.ConfigValue, out bool enabled) && enabled;

                    if (sendMailToManager)
                    {
                        ccUserNames = (_sysUserCache.GetManagement(employeeIds) ?? Enumerable.Empty<SysUserModel>())
                            .Select(x => x.UserName)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .Except(toUserNames, StringComparer.OrdinalIgnoreCase)
                            .ToList();
                    }

                    if (toUserNames.Any())
                    {
                        _projectTaskMailService.QueueSendProjectTaskUpdatedMail(
                            result,
                            toUserNames,
                            User.UserName,
                            ccUserNames);
                    }
                }

                _taskActivityService.LogChanges(
                    result,
                    oldData,
                    newData,
                    "TASK_UPDATED",
                    User.UserName,
                    User.UserId,
                    "TaskName",
                    "ParentTaskID",
                    "StatusID",
                    "PriorityID",
                    "TaskTypeID",
                    "AssigneeIDs",
                    "StartDate",
                    "EndDate",
                    "EstimatedHours",
                    "CompletionPercentage"
                );
            }
            if (result == 0)
                response = CreateMessage($"{_TaskTitle} [{model.TaskName}]", EnumProcessType.Edit, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_TaskTitle} [{model.TaskName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_TaskTitle} [{model.TaskName}]", EnumProcessType.Edit, EnumMsgIcon.Success);

            return Json(new { status = true, message = response, projectId = model.ProjectID }, JsonRequestBehavior.AllowGet);
        }

        // DELETE
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _taskManagementCache.GetById(id);
            if (model == null)
                return Json(new { status = true, message = CreateMessage($"{_TaskTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_TaskTitle} [{model.TaskName}]");
            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_TaskManagementModel model)
        {
            var deleted = _taskManagementCache.Delete(model, User.UserName);
            var response = CreateMessage($"{_TaskTitle} [{model.TaskName}]",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
        #endregion
    }
}
