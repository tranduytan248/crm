using Core.Cate.Caches;
using Core.Cate.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Cate.Services
{
    public class TaskActivityService
    {
        private readonly RM_TaskActivityCache _cache;

        public TaskActivityService()
        {
            _cache = new RM_TaskActivityCache();
        }
        private void ResolveDisplayNames(List<TaskActivityChange> changes, RM_TaskManagementModel oldData, RM_TaskManagementModel newData)
        {
            foreach (var change in changes)
            {
                switch (change.ActivityType)
                {
                    case "FIELD_TASKNAME":
                        change.OldValue = string.IsNullOrEmpty(change.OldValue) ? "—" : change.OldValue;
                        change.NewValue = string.IsNullOrEmpty(change.NewValue) ? "—" : change.NewValue;
                        change.Description = $"Tên công việc: {change.OldValue} → {change.NewValue}";
                        break;

                    case "FIELD_PARENTTASKID":
                        change.OldValue = string.IsNullOrEmpty(oldData?.ParentTaskName) ? "—" : oldData.ParentTaskName;
                        change.NewValue = string.IsNullOrEmpty(newData?.ParentTaskName) ? "—" : newData.ParentTaskName;
                        change.Description = $"Task cha: {change.OldValue} → {change.NewValue}";
                        break;

                    case "FIELD_STATUSID":
                        change.OldValue = string.IsNullOrEmpty(oldData?.StatusName) ? "—" : oldData.StatusName;
                        change.NewValue = string.IsNullOrEmpty(newData?.StatusName) ? "—" : newData.StatusName;
                        change.Description = $"Trạng thái: {change.OldValue} → {change.NewValue}";
                        break;

                    case "FIELD_PRIORITYID":
                        change.OldValue = string.IsNullOrEmpty(oldData?.PriorityName) ? "Bình thường" : oldData.PriorityName;
                        change.NewValue = string.IsNullOrEmpty(newData?.PriorityName) ? "Bình thường" : newData.PriorityName;
                        change.Description = $"Độ ưu tiên: {change.OldValue} → {change.NewValue}";
                        break;

                    case "FIELD_TASKTYPEID":
                        change.OldValue = string.IsNullOrEmpty(oldData?.TaskTypeName) ? "—" : oldData.TaskTypeName;
                        change.NewValue = string.IsNullOrEmpty(newData?.TaskTypeName) ? "—" : newData.TaskTypeName;
                        change.Description = $"Loại công việc: {change.OldValue} → {change.NewValue}";
                        break;

                    case "FIELD_ASSIGNEEIDS":
                        change.OldValue = string.IsNullOrEmpty(oldData?.AssigneeNames) ? "—" : oldData.AssigneeNames;
                        change.NewValue = string.IsNullOrEmpty(newData?.AssigneeNames) ? "—" : newData.AssigneeNames;
                        change.Description = $"Người thực hiện: {change.OldValue} → {change.NewValue}";
                        break;

                    case "FIELD_STARTDATE":
                        change.OldValue = string.IsNullOrEmpty(change.OldValue) ? "—" : Convert.ToDateTime(change.OldValue).ToString("dd/MM/yyyy");
                        change.NewValue = string.IsNullOrEmpty(change.NewValue) ? "—" : Convert.ToDateTime(change.NewValue).ToString("dd/MM/yyyy");
                        change.Description = $"Ngày bắt đầu: {change.OldValue} → {change.NewValue}";
                        break;

                    case "FIELD_ENDDATE":
                        change.OldValue = string.IsNullOrEmpty(change.OldValue) ? "—" : Convert.ToDateTime(change.OldValue).ToString("dd/MM/yyyy");
                        change.NewValue = string.IsNullOrEmpty(change.NewValue) ? "—" : Convert.ToDateTime(change.NewValue).ToString("dd/MM/yyyy");
                        change.Description = $"Ngày kết thúc: {change.OldValue} → {change.NewValue}";
                        break;

                    case "FIELD_ESTIMATEDHOURS":
                        change.OldValue = string.IsNullOrEmpty(change.OldValue) ? "0" : change.OldValue;
                        change.NewValue = string.IsNullOrEmpty(change.NewValue) ? "0" : change.NewValue;
                        change.Description = $"Giờ ước tính: {change.OldValue} → {change.NewValue}";
                        break;

                    case "FIELD_COMPLETIONPERCENTAGE":
                        change.OldValue = string.IsNullOrEmpty(change.OldValue) ? "0" : change.OldValue;
                        change.NewValue = string.IsNullOrEmpty(change.NewValue) ? "0" : change.NewValue;
                        change.Description = $"Tiến độ: {change.OldValue}% → {change.NewValue}%";
                        break;
                }
            }
        }


        // 1. COMPARE + LOG (MAIN METHOD)
        public void LogChanges<T>(
            int taskId,
            T oldData,
            T newData,
            string actionType,
            string username,
            int employeeId = 0,
            params string[] ignoreFields)
        {
            var changes = CompareObjects(oldData, newData, ignoreFields);

            if (changes == null || !changes.Any())
                return;
            ResolveDisplayNames(
                changes,
                oldData as RM_TaskManagementModel,
                newData as RM_TaskManagementModel
            );
            SaveGroup(taskId, actionType, changes, username, employeeId);
        }

        // 2. REFLECTION COMPARE
        private List<TaskActivityChange> CompareObjects<T>(
            T oldObj,
            T newObj,
            params string[] ignoreFields)
        {
            var changes = new List<TaskActivityChange>();

            if (oldObj == null || newObj == null)
                return changes;

            var ignore = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Nếu có truyền trackFields thì chỉ so sánh những field đó
            var propsToCheck = ignoreFields != null && ignoreFields.Length > 0
                ? ignore.Where(p => ignoreFields.Contains(p.Name))
                : ignore; // ✅ không truyền = so sánh tất cả

            foreach (var prop in propsToCheck)
            {
                if (!prop.CanRead) continue;

                var oldValue = prop.GetValue(oldObj)?.ToString();
                var newValue = prop.GetValue(newObj)?.ToString();

                if (oldValue != newValue)
                {
                    changes.Add(new TaskActivityChange
                    {
                        ActivityType = $"FIELD_{prop.Name.ToUpper()}",
                        OldValue = oldValue,
                        NewValue = newValue,
                        Description = $"{prop.Name}: {oldValue} → {newValue}"
                    });
                }
            }

            return changes;
        }

        // 3. SAVE GROUP (1 ACTION = 1 JSON)
        public void SaveGroup(
            int taskId,
            string actionType,
            List<TaskActivityChange> changes,
            string username,
            int employeeId = 0
            )
        {
            var payload = new
            {
                TaskId = taskId,
                ActionType = actionType,
                ChangedBy = username,
                ChangedDate = DateTime.Now,
                Changes = changes
            };

            var model = new RM_TaskActivityModel
            {
                TaskManagementID = taskId,
                Employee_ID = employeeId,
                ActivityType = actionType,
                Description = JsonConvert.SerializeObject(payload),
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            _cache.Save(model, username);
        }

        // ================================
        // 4. MANUAL LOG (OPTIONAL)
        // ================================
        public void LogSingle(
            int taskId,
            string type,
            string oldValue,
            string newValue,
            string description,
            string username)
        {
            var model = new RM_TaskActivityModel
            {
                TaskManagementID = taskId,
                ActivityType = type,
                OldValue = oldValue,
                NewValue = newValue,
                Description = description,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            _cache.Save(model, username);
        }
    }
}

