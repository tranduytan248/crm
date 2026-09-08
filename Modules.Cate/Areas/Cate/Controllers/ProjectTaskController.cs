using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using System.Web.Mvc;
using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Cate.Services;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class ProjectTaskController : AppController
    {
        private readonly RM_ProjectTaskCache _cache;
        private readonly RM_TaskCache _taskCache;
        private readonly RM_TaskGroupCache _taskGroupCache;
        private readonly RM_TaskTypeCache _taskTypeCache;
        private readonly RM_PriorityCache _priorityCache;
        private readonly RM_ProjectMemberCache _projectMemberCache;
        private readonly RM_TaskManagementCache _taskManagementCache;
        private readonly string _Title = AppProcessor.Messagor.GetMessage("ProjectTask_Title");
        private readonly RM_StatusCache _taskStatusCache;
        private readonly SysUserCache _sysUserCache;
        private readonly RM_TaskAssigneeCache _taskAssigneeCache;
        private readonly SysConfigCache _configsCache;
        private readonly ProjectTaskMailService _projectTaskMailService;
        private readonly TaskFileService _fileService;



        public ProjectTaskController()
        {
            _cache = new RM_ProjectTaskCache();
            _taskCache = new RM_TaskCache();
            _taskGroupCache = new RM_TaskGroupCache();
            _taskTypeCache = new RM_TaskTypeCache();
            _priorityCache = new RM_PriorityCache();
            _taskStatusCache = new RM_StatusCache();
            _projectMemberCache = new RM_ProjectMemberCache();
            _taskManagementCache = new RM_TaskManagementCache();
            _sysUserCache = new SysUserCache();
            _taskAssigneeCache = new RM_TaskAssigneeCache();
            _configsCache = new SysConfigCache();
            _projectTaskMailService = new ProjectTaskMailService();
            _fileService = new TaskFileService();
        }

        /// <summary>
        /// Lấy tập TaskID đã tồn tại trong dự án từ cả công việc dự án và quản lý công việc để chống trùng khi thêm mới.
        /// </summary>
        //private HashSet<int> GetExistingTaskIds(int productProjectID)
        //{
        //    var projectTaskIds = (_cache.GetByProductProjectID(productProjectID) ?? new List<RM_ProjectTaskModel>())
        //        .Where(x => !x.IsDeleted.GetValueOrDefault())
        //        .Select(x => x.TaskID);

        //    var taskManagementIds = (_taskManagementCache.GetByProductProjectID(productProjectID) ?? new List<RM_TaskManagementModel>())
        //        .Where(x => !x.IsDeleted.GetValueOrDefault())
        //        .Select(x => x.TaskID);

        //    return new HashSet<int>(projectTaskIds.Concat(taskManagementIds));
        //}


        // v2 dùng ProjectId
        private HashSet<int> GetExistingTaskIds_v2(int projectID)
        {
            var projectTaskIds = (_cache.GetByProjectID(projectID) ?? new List<RM_ProjectTaskModel>())
                .Where(x => !x.IsDeleted.GetValueOrDefault())
                .Select(x => x.TaskID);

            var taskManagementIds = (_taskManagementCache.GetAll(projectID) ?? new List<RM_TaskManagementModel>())
                .Where(x => !x.IsDeleted.GetValueOrDefault())
                .Select(x => x.TaskID);

            return new HashSet<int>(projectTaskIds.Concat(taskManagementIds));
        }

        /*
        /// <summary>
        /// Kiểm tra task đang thao tác có trùng với task khác trong cùng dự án hay không.
        /// </summary>
        private bool HasDuplicateTask(int productProjectID, int taskID, int? excludedProjectTaskID = null, int? excludedTaskManagementID = null)
        {
            var projectTasks = (_cache.GetByProductProjectID(productProjectID) ?? new List<RM_ProjectTaskModel>())
                .Where(x => !x.IsDeleted.GetValueOrDefault() && x.TaskID == taskID);

            if (excludedProjectTaskID.HasValue)
            {
                projectTasks = projectTasks.Where(x => x.ProjectTaskID != excludedProjectTaskID.Value);
            }

            if (projectTasks.Any())
            {
                return true;
            }

            var taskManagements = (_taskManagementCache.GetByProductProjectID(productProjectID) ?? new List<RM_TaskManagementModel>())
                .Where(x => !x.IsDeleted.GetValueOrDefault() && x.TaskID == taskID);

            if (excludedTaskManagementID.HasValue)
            {
                taskManagements = taskManagements.Where(x => x.TaskManagementID != excludedTaskManagementID.Value);
            }

            return taskManagements.Any();
        }
        */

        // V2 dùng ProjectID
        private bool HasDuplicateTask_v2(int ProjectID, int taskID, int? excludedProjectTaskID = null, int? excludedTaskManagementID = null)
        {
            var projectTasks = (_cache.GetByProjectID(ProjectID) ?? new List<RM_ProjectTaskModel>())
                .Where(x => !x.IsDeleted.GetValueOrDefault() && x.TaskID == taskID);

            if (excludedProjectTaskID.HasValue)
            {
                projectTasks = projectTasks.Where(x => x.ProjectTaskID != excludedProjectTaskID.Value);
            }

            if (projectTasks.Any())
            {
                return true;
            }

            var taskManagements = (_taskManagementCache.GetAll(ProjectID) ?? new List<RM_TaskManagementModel>())
                .Where(x => !x.IsDeleted.GetValueOrDefault() && x.TaskID == taskID);

            if (excludedTaskManagementID.HasValue)
            {
                taskManagements = taskManagements.Where(x => x.TaskManagementID != excludedTaskManagementID.Value);
            }

            return taskManagements.Any();
        }

        /// <summary>
        /// Tạo danh sách nhân sự có thể gán việc theo thành viên hiện có của sản phẩm dự án.
        /// </summary>
        private List<SelectListItem> GetEmployeeSelectList(int productProjectID)
        {
            var allUsers = _sysUserCache.GetAll() ?? new List<Core.Sys.Models.Sys.SysUserModel>();
            return (_projectMemberCache.GetByProductProjectID(productProjectID) ?? new List<RM_ProjectMemberModel>())
                .Select(x =>
                {
                    var user = allUsers.FirstOrDefault(u => u.UserId == x.Employee_ID);
                    return new SelectListItem
                    {
                        Value = user != null && user.UserId.HasValue
                            ? user.UserId.Value.ToString()
                            : x.Employee_ID.ToString(),
                        Text = x.FullName
                    };
                })
                .GroupBy(x => x.Value)
                .Select(x => x.First())
                .ToList();
        }

        /// <summary>
        /// Ánh xạ dữ liệu quản lý công việc sang model form hiện có của màn hình công việc dự án.
        /// </summary>
        private RM_ProjectTaskModel MapTaskManagementToProjectTaskModel(RM_TaskManagementModel item)
        {
            return new RM_ProjectTaskModel
            {
                ProjectTaskID = 0,
                TaskManagementID = item.TaskManagementID,
                IsTaskManagementSource = true,
                ProductProjectID = item.ProductProjectID,
                TaskID = item.TaskID,
                TaskName = item.TaskName,
                StartDate = item.StartDate,
                CompletedDate = item.EndDate,
                Status = item.StatusID.HasValue ? (byte?)item.StatusID.Value : null,
                CompletionPercentage = item.CompletionPercentage,
                Note = item.Description,
                IsDeleted = item.IsDeleted,
                CreatedBy = item.CreatedBy,
                CreatedDate = item.CreatedDate,
                LastModifiedBy = item.UpdatedBy,
                LastModifiedDate = item.UpdatedDate
            };
        }

        /// <summary>
        /// Dựng model lưu xuống bảng quản lý công việc từ form công việc dự án.
        /// </summary>
        private RM_TaskManagementModel BuildTaskManagementSaveModel(RM_ProjectTaskModel model, RM_TaskModel task)
        {
            return new RM_TaskManagementModel
            {
                TaskManagementID = model.TaskManagementID ?? 0,
                ProductProjectID = model.ProductProjectID,
                TaskID = model.TaskID,
                TaskCode = task != null ? task.TaskCode : null,
                TaskName = task != null ? task.TaskName : model.TaskName,
                StatusID = model.Status.HasValue ? (byte?)model.Status.Value : null,
                TaskTypeID = task != null ? task.TaskTypeID : null,
                StartDate = model.StartDate,
                EndDate = model.CompletedDate,
                CompletionPercentage = model.CompletionPercentage,
                Description = model.Note
            };
        }

        /// <summary>
        /// Nạp dữ liệu dropdown cho form thêm hoặc cập nhật một công việc dự án.
        /// </summary>
        private void PopulateLists(RM_ProjectTaskModel model)
        {
            model.ListTask = (_taskCache.GetAll() ?? new List<RM_TaskModel>())
                .Select(x => new SelectListItem { Value = x.TaskID.ToString(), Text = x.TaskName })
                .ToList();

            var projectId = model.ProductProjectID;
            if (projectId == 0)
            {
                if (model.TaskManagementID.HasValue && model.TaskManagementID.Value > 0)
                {
                    var existingTaskManagement = _taskManagementCache.GetById(model.TaskManagementID.Value);
                    if (existingTaskManagement != null)
                    {
                        projectId = existingTaskManagement.ProductProjectID;
                    }
                }
                else if (model.ProjectTaskID > 0)
                {
                    var existing = _cache.GetById(model.ProjectTaskID);
                    if (existing != null)
                    {
                        projectId = existing.ProductProjectID;
                    }
                }
            }

            model.ListEmployee = GetEmployeeSelectList(projectId);
            model.ListTaskStatus = _taskStatusCache.GetStatusBySearchKey("Task");
        }
        /*
        /// <summary>
        /// Dựng danh sách công việc nguồn từ bộ công việc để hiển thị trong popup thêm nhanh.
        /// </summary>
        private List<RM_ProjectTaskBulkAddItemModel> BuildBulkAddItems(int productProjectID)
        {
            var tasks = _taskCache.GetAll() ?? new List<RM_TaskModel>();
            var taskTypeLookup = (_taskTypeCache.GetAll() ?? new List<RM_TaskTypeModel>())
                .ToDictionary(x => x.TaskTypeID, x => x.TaskTypeName);
            var existingTaskIds = GetExistingTaskIds(productProjectID);

            return tasks
                .Select(task =>
                {
                    var detail = _taskCache.GetById(task.TaskID);
                    var taskTypeId = detail != null && detail.TaskTypeID.HasValue
                        ? detail.TaskTypeID
                        : task.TaskTypeID;
                    var sortOrder = detail != null && detail.SortOrder.HasValue
                        ? detail.SortOrder
                        : task.SortOrder;

                    return new RM_ProjectTaskBulkAddItemModel
                    {
                        TaskID = task.TaskID,
                        TaskCode = task.TaskCode,
                        TaskName = task.TaskName,
                        TaskParentsID = task.TaskParentsID,
                        ParentsTask = task.ParentsTask,
                        TaskGroupID = task.TaskGroupID,
                        TaskGroupName = task.TaskGroupName,
                        TaskTypeID = taskTypeId,
                        TaskTypeName = taskTypeId.HasValue && taskTypeLookup.ContainsKey(taskTypeId.Value)
                            ? taskTypeLookup[taskTypeId.Value]
                            : null,
                        SortOrder = sortOrder,
                        IsExistingProjectTask = existingTaskIds.Contains(task.TaskID)
                    };
                })
                .OrderBy(x => x.TaskGroupName)
                .ThenBy(x => x.SortOrder ?? int.MaxValue)
                .ThenBy(x => x.TaskCode)
                .ThenBy(x => x.TaskName)
                .ToList();
        }
        */

        // v2 dùng projectID 
        private List<RM_ProjectTaskBulkAddItemModel> BuildBulkAddItems_v2(int projectID)
        {
            var tasks = _taskCache.GetAll() ?? new List<RM_TaskModel>();
            var taskTypeLookup = (_taskTypeCache.GetAll() ?? new List<RM_TaskTypeModel>())
                .ToDictionary(x => x.TaskTypeID, x => x.TaskTypeName);
            var existingTaskIds = GetExistingTaskIds_v2(projectID);

            return tasks
                .Select(task =>
                {
                    var detail = _taskCache.GetById(task.TaskID);
                    var taskTypeId = detail != null && detail.TaskTypeID.HasValue
                        ? detail.TaskTypeID
                        : task.TaskTypeID;
                    var sortOrder = detail != null && detail.SortOrder.HasValue
                        ? detail.SortOrder
                        : task.SortOrder;

                    return new RM_ProjectTaskBulkAddItemModel
                    {
                        TaskID = task.TaskID,
                        TaskCode = task.TaskCode,
                        TaskName = task.TaskName,
                        TaskParentsID = task.TaskParentsID,
                        ParentsTask = task.ParentsTask,
                        TaskGroupID = task.TaskGroupID,
                        TaskGroupName = task.TaskGroupName,
                        TaskTypeID = taskTypeId,
                        TaskTypeName = taskTypeId.HasValue && taskTypeLookup.ContainsKey(taskTypeId.Value)
                            ? taskTypeLookup[taskTypeId.Value]
                            : null,
                        SortOrder = sortOrder,
                        IsExistingProjectTask = existingTaskIds.Contains(task.TaskID)
                    };
                })
                .OrderBy(x => x.TaskGroupName)
                .ThenBy(x => x.SortOrder ?? int.MaxValue)
                .ThenBy(x => x.TaskCode)
                .ThenBy(x => x.TaskName)
                .ToList();
        }
        /// <summary>
        /// Nạp dữ liệu nền cho popup thêm công việc từ bộ công việc.
        /// </summary>
        private void PopulateBulkAddModel(RM_ProjectTaskBulkAddModel model)
        {
            model.TaskGroups = _taskGroupCache.GetAll() ?? new List<RM_TaskGroupModel>();
            model.TaskTypes = _taskTypeCache.GetAll() ?? new List<RM_TaskTypeModel>();
            model.Priorities = _priorityCache.GetAll() ?? new List<RM_PriorityModel>();
            model.ListTaskStatus = _taskStatusCache.GetStatusBySearchKey("Task") ?? new List<RM_StatusModel>();
            var allMemb = _projectMemberCache.GetAll(model.ProjectID);
            model.ListEmployee = allMemb.Select(x => new SelectListItem { Value = x.Employee_ID.ToString(), Text = x.FullName }).ToList();//  GetEmployeeSelectList(model.ProductProjectID);

            if (model.Items == null || model.Items.Count == 0)
            {
                //model.Items = BuildBulkAddItems(model.ProductProjectID);
                model.Items = BuildBulkAddItems_v2(model.ProjectID);
            }
        }

        /// <summary>
        /// Hiển thị popup thêm mới một công việc dự án.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(int id)
        {
            var model = new RM_ProjectTaskModel { ProductProjectID = id };
            PopulateLists(model);
            return PartialView("_Add", model);
        }

        /// <summary>
        /// Hiển thị popup thêm nhiều công việc từ bộ công việc cho sản phẩm dự án.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult AddFromTaskGroup(int id)
        {
            var model = new RM_ProjectTaskBulkAddModel
            {
                ProjectID = id
            };

            PopulateBulkAddModel(model);
            return PartialView("_AddFromTaskGroup", model);
        }
        /*
        /// <summary>
        /// Lưu một công việc dự án được tạo thủ công từ popup thêm mới vào bảng quản lý công việc.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(RM_ProjectTaskModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateLists(model);
                return PartialView("_Add", model);
            }

            var task = (_taskCache.GetAll() ?? new List<RM_TaskModel>()).FirstOrDefault(x => x.TaskID == model.TaskID);
            var taskName = task != null ? task.TaskName : _Title;

            if (HasDuplicateTask(model.ProjectID, model.TaskID))
            {
                var duplicatedResponse = CreateMessage($"{_Title} [{taskName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = duplicatedResponse }, JsonRequestBehavior.AllowGet);
            }

            var result = _taskManagementCache.Save(BuildTaskManagementSaveModel(model, task), User.UserName);
            string response;
            if (result == 0) response = CreateMessage($"{_Title} [{taskName}]", EnumProcessType.Add, EnumMsgIcon.Error);
            else response = CreateMessage($"{_Title} [{taskName}]", EnumProcessType.Add, EnumMsgIcon.Success);

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }
        */
        /// <summary>
        /// Lưu nhiều công việc từ bộ công việc vào bảng quản lý công việc và bỏ qua các công việc đã tồn tại.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult AddFromTaskGroup(RM_ProjectTaskBulkAddModel model)
        {
            if (!model.TaskGroupID.HasValue || model.TaskGroupID.Value <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.TaskGroupID),
                    AppProcessor.Messagor.GetMessage("ProjectTask_AddFromTaskGroup_Message_TaskGroupRequired"));
            }

            var selectedItems = (model.Items ?? new List<RM_ProjectTaskBulkAddItemModel>())
                .Where(x => x.IsSelected)
                .GroupBy(x => x.TaskID)
                .Select(x => x.First())
                .ToList();

            if (selectedItems.Count == 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    AppProcessor.Messagor.GetMessage("ProjectTask_AddFromTaskGroup_Message_TaskRequired"));
            }

            if (!ModelState.IsValid)
            {
                PopulateBulkAddModel(model);
                return PartialView("_AddFromTaskGroupForm", model);
            }

            var existingTaskIds = GetExistingTaskIds_v2(model.ProjectID);
            var savedCount = 0;
            var duplicateCount = 0;
            var errorCount = 0;

            foreach (var item in selectedItems)
            {
                if (!item.StatusID.HasValue)
                {
                    ModelState.AddModelError(string.Empty, AppProcessor.Messagor.GetMessage("Validate_Status"));
                    PopulateBulkAddModel(model);
                    return PartialView("_AddFromTaskGroupForm", model);
                }

                if (existingTaskIds.Contains(item.TaskID))
                {
                    duplicateCount++;
                    continue;
                }

                var description = string.IsNullOrWhiteSpace(item.Description)
                    ? item.Note
                    : item.Description;

                var taskManagementResult = _taskManagementCache.Save(
                    new RM_TaskManagementModel
                    {
                        ProjectID = model.ProjectID,
                        TaskID = item.TaskID,
                        TaskName = item.TaskName,
                        StatusID = item.StatusID.HasValue
                            ? (byte?)item.StatusID.Value
                            : null,
                        PriorityID = item.PriorityID,
                        TaskTypeID = item.TaskTypeID,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        EstimatedHours = item.EstimatedHours,
                        ActualHours = item.ActualHours,
                        CompletionPercentage = item.CompletionPercentage,
                        Description = description,
                    },
                    User.UserName);

                if (taskManagementResult > 0)
                {
                    // Parse AssigneeIDs -> AssigneeIDList nếu chưa có
                    if (!string.IsNullOrWhiteSpace(item.AssigneeIDs) &&
                        (item.AssigneeIDList == null || !item.AssigneeIDList.Any()))
                    {
                        item.AssigneeIDList = item.AssigneeIDs
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => int.TryParse(x.Trim(), out var id) ? id : 0)
                            .Where(x => x > 0)
                            .ToList();
                    }

                    _taskAssigneeCache.Save(
                        taskManagementResult,
                        item.AssigneeIDs,
                        User.UserName);

                    if (item.AssigneeIDList != null && item.AssigneeIDList.Any())
                    {
                        // Người nhận (To)
                        var toUserNames = _sysUserCache.GetAll()
                            .Where(x => x.UserId.HasValue &&
                                        item.AssigneeIDList.Contains(x.UserId.Value))
                            .Select(x => x.UserName)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();

                        // Người quản lý (CC)
                        var ccUserNames = new List<string>();

                        bool sendMailToManager =
                            bool.TryParse(
                                _configsCache.GetViaKey("CONFIG_SEND_MAIL_TO_MANAGER")?.ConfigValue,
                                out bool enabled)
                            && enabled;

                        if (sendMailToManager)
                        {
                            var assigneeIds = string.Join(";", item.AssigneeIDList);

                            ccUserNames = (_sysUserCache.GetManagement(assigneeIds) ?? Enumerable.Empty<SysUserModel>())
                                .Select(x => x.UserName)
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .Except(toUserNames, StringComparer.OrdinalIgnoreCase)
                                .ToList();
                        }

                        if (toUserNames.Any())
                        {
                            _projectTaskMailService.QueueSendProjectTaskCreatedMail(
                                taskManagementResult,
                                toUserNames,
                                User.UserName,
                                ccUserNames);
                        }
                    }

                    var dbRequest = new SaveFilePathRequest
                    {
                        TaskManagementID = taskManagementResult
                    };

                    _fileService.SaveImagePathsFromHtml(
                        description,
                        dbRequest,
                        User.UserName);

                    // Upload attached files
                    if (!string.IsNullOrWhiteSpace(item.FileIndexes)
                        && model.Files != null
                        && model.Files.Any())
                    {
                        var fileIndexes = item.FileIndexes
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => int.TryParse(x, out var idx) ? idx : -1)
                            .Where(x => x >= 0)
                            .Distinct()
                            .ToList();

                        var files = fileIndexes
                            .Where(x => x < model.Files.Count())
                            .Select(x => model.Files.ElementAt(x))
                            .Where(f => f != null && f.ContentLength > 0)
                            .ToList();

                        if (files.Any())
                        {
                            _fileService.UploadAndSave(
                                files,
                                dbRequest,
                                User.UserName,
                                subFolder: "task");
                        }
                    }

                    savedCount++;
                    existingTaskIds.Add(item.TaskID);
                }
                else
                {
                    errorCount++;
                }
            }

            string response;
            if (savedCount == 0 && duplicateCount > 0 && errorCount == 0)
            {
                response = CreateMessage(
                    AppProcessor.Messagor.GetMessage("ProjectTask_AddFromTaskGroup_Message_AllTasksExist"),
                    EnumProcessType.NonFormat,
                    EnumMsgIcon.Error);
            }
            else if (savedCount == 0)
            {
                response = CreateMessage(_Title, EnumProcessType.Add, EnumMsgIcon.Error);
            }
            else if (duplicateCount > 0 || errorCount > 0)
            {
                response = CreateMessage(
                    string.Format(
                        AppProcessor.Messagor.GetMessage("ProjectTask_AddFromTaskGroup_Message_SaveSummary"),
                        savedCount,
                        duplicateCount,
                        errorCount),
                    EnumProcessType.NonFormat,
                    EnumMsgIcon.Warning);
            }
            else
            {
                response = CreateMessage(
                    string.Format(
                        AppProcessor.Messagor.GetMessage("ProjectTask_AddFromTaskGroup_Message_SaveSuccess"),
                        savedCount),
                    EnumProcessType.NonFormat,
                    EnumMsgIcon.Success);
            }

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị popup cập nhật thông tin của một công việc thuộc bảng công việc dự án hoặc quản lý công việc.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id, bool isTaskManagement = false)
        {
            RM_ProjectTaskModel model;
            if (isTaskManagement)
            {
                var taskManagement = _taskManagementCache.GetById(id);
                model = taskManagement != null ? MapTaskManagementToProjectTaskModel(taskManagement) : null;
            }
            else
            {
                model = _cache.GetById(id);
            }

            if (model == null)
                return Json(new { status = true, message = CreateMessage(_Title, EnumProcessType.DataNotExist, EnumMsgIcon.Error) });

            PopulateLists(model);
            return PartialView("_Edit", model);
        }

        /// <summary>
        /// Lưu thông tin cập nhật của công việc dự án hoặc công việc quản lý dự án.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(RM_ProjectTaskModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateLists(model);
                return PartialView("_ProjectTask", model);
            }

            var task = (_taskCache.GetAll() ?? new List<RM_TaskModel>()).FirstOrDefault(x => x.TaskID == model.TaskID);
            var taskName = task != null ? task.TaskName : _Title;

            if (model.TaskManagementID.HasValue && model.TaskManagementID.Value > 0)
            {
                if (HasDuplicateTask_v2(model.ProjectID, model.TaskID, null, model.TaskManagementID))
                {
                    var duplicatedResponse = CreateMessage($"{_Title} [{taskName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
                    return Json(new { status = true, message = duplicatedResponse }, JsonRequestBehavior.AllowGet);
                }

                var taskManagementResult = _taskManagementCache.Save(BuildTaskManagementSaveModel(model, task), User.UserName);
                var taskManagementResponse = taskManagementResult > 0
                    ? CreateMessage($"{_Title} [{taskName}]", EnumProcessType.Edit, EnumMsgIcon.Success)
                    : CreateMessage($"{_Title} [{taskName}]", EnumProcessType.Edit, EnumMsgIcon.Error);

                return Json(new { status = true, message = taskManagementResponse }, JsonRequestBehavior.AllowGet);
            }

            if (HasDuplicateTask_v2(model.ProjectID, model.TaskID, model.ProjectTaskID, null))
            {
                var duplicatedProjectTaskResponse = CreateMessage($"{_Title} [{taskName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = duplicatedProjectTaskResponse }, JsonRequestBehavior.AllowGet);
            }

            var result = _cache.Save(model, User.UserName);
            string response;
            if (result == 0) response = CreateMessage($"{_Title} [{taskName}]", EnumProcessType.Edit, EnumMsgIcon.Error);
            else if (result == -9) response = CreateMessage($"{_Title} [{taskName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else response = CreateMessage($"{_Title} [{taskName}]", EnumProcessType.Edit, EnumMsgIcon.Success);

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị popup xác nhận xóa công việc dự án hoặc công việc quản lý dự án.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id, bool isTaskManagement = false)
        {
            RM_ProjectTaskModel model;
            if (isTaskManagement)
            {
                var taskManagement = _taskManagementCache.GetById(id);
                model = taskManagement != null ? MapTaskManagementToProjectTaskModel(taskManagement) : null;
            }
            else
            {
                model = _cache.GetById(id);
            }

            if (model == null)
                return Json(new { status = true, message = CreateMessage(_Title, EnumProcessType.DataNotExist, EnumMsgIcon.Error) });

            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_Title} [{model.TaskName}]");
            return PartialView("_Delete", model);
        }

        /// <summary>
        /// Thực hiện xóa mềm công việc dự án hoặc công việc quản lý dự án theo yêu cầu người dùng.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_ProjectTaskModel model)
        {
            if (model.TaskManagementID.HasValue && model.TaskManagementID.Value > 0)
            {
                var existingTaskManagement = _taskManagementCache.GetById(model.TaskManagementID.Value);
                var taskName = existingTaskManagement != null ? existingTaskManagement.TaskName : _Title;
                var deletedTaskManagement = _taskManagementCache.Delete(
                    new RM_TaskManagementModel { TaskManagementID = model.TaskManagementID.Value },
                    User.UserName);
                var taskManagementResponse = CreateMessage($"{_Title} [{taskName}]",
                    EnumProcessType.Delete, deletedTaskManagement > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
                return Json(new { status = true, message = taskManagementResponse });
            }

            var existing = _cache.GetById(model.ProjectTaskID);
            var legacyTaskName = existing != null ? existing.TaskName : _Title;
            var deleted = _cache.Delete(model, User.UserName);
            var response = CreateMessage($"{_Title} [{legacyTaskName}]",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
    }
}
