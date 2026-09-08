using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_TaskManagementBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_TaskManagement_Get = "RM_TaskManagement_Get";
        private readonly string _RM_TaskManagement_GetByID = "RM_TaskManagement_GetByID";
        private readonly string _RM_TaskManagement_GetByProductProjectID = "RM_TaskManagement_GetByProductProjectID";
        private readonly string _RM_TaskManagement_Delete = "RM_TaskManagement_Delete";
        private readonly string _RM_TaskManagement_Save = "RM_TaskManagement_Save";

        public List<RM_TaskManagementModel> LoadList(
            out int total,
            RM_TaskManagementSearchModel model,
            BaseSearchModel search = null)
        {
            search = search ?? new BaseSearchModel();

            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_TaskManagementModel>(
                _RM_TaskManagement_Get,
                DATA_PROVIDER_NAME,
                model.ProjectID,
                model.Keyword,                 // @Search
                                               // model?.StatusID,              // @StatusID
                                               // model?.PriorityID,            // @PriorityID
                                               // model?.TaskTypeID,            // @TaskTypeID
                                               // model?.ProductProjectID,      // @ProductProjectID
                                               // model?.BusinessOpportunityID, // @BusinessOpportunityID
                                               // model?.EmployeeID,            // @Employee_ID
                                               // model?.FromDate,              // @FromDate
                                               // model?.ToDate,                // @ToDate
                search.Order ?? "4",          // @Order
                search.OrderDir ?? "DESC",    // @OrderDir
                search.StartIndex,            // @PageIndex
                search.PageSize              // @PageSize
            );

            total = data?.FirstOrDefault()?.TotalRow ?? 0;
            return data;
        }
        public List<RM_TaskManagementModel> GetAll(int ProjectID)
        {
            int total = 0;
            return LoadList(out total, new RM_TaskManagementSearchModel { ProjectID = ProjectID });
        }

        /// <summary>
        /// Chuyển ngày nullable về chuỗi ISO để truyền ổn định qua lớp gọi stored procedure.
        /// </summary>
        private string ToSqlDateString(System.DateTime? value)
        {
            return value.HasValue
                ? value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                : null;
        }

        /// <summary>
        /// Chuyển số nullable về chuỗi invariant để tránh lệ thuộc locale khi lưu giờ.
        /// </summary>
        private string ToSqlDecimalString(decimal? value)
        {
            return value.HasValue
                ? value.Value.ToString(CultureInfo.InvariantCulture)
                : null;
        }

        /// <summary>
        /// Lấy chi tiết quản lý công việc theo mã công việc
        /// </summary>
        public RM_TaskManagementModel GetById(int taskManagementID)
        {
            return AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_TaskManagementModel>(
                _RM_TaskManagement_GetByID,
                DATA_PROVIDER_NAME,
                taskManagementID);
        }

        /// <summary>
        /// Lấy danh sách quản lý công việc theo sản phẩm dự án.
        /// </summary>
        public List<RM_TaskManagementModel> GetByProductProjectID(int productProjectID)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_TaskManagementModel>(
                _RM_TaskManagement_GetByProductProjectID,
                DATA_PROVIDER_NAME,
                productProjectID);
        }

        /// <summary>
        /// Xóa mềm dữ liệu quản lý công việc.
        /// </summary>
        public int Delete(RM_TaskManagementModel model, string username)
        {
            return AppProcessor.ProcedureProvider.Execute(
                _RM_TaskManagement_Delete,
                DATA_PROVIDER_NAME,
                model.TaskManagementID,
                username).GetValueOrDefault(0);
        }

        /// <summary>
        /// Lưu hoặc cập nhật thông tin quản lý công việc theo dự án và công việc nguồn.
        /// </summary>
        public int Save(RM_TaskManagementModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_TaskManagement_Save,
                DATA_PROVIDER_NAME,
                model.TaskManagementID,
                model.ProjectID,
                model.BusinessOpportunityID,
                model.TaskID,
                model.ParentTaskID,
                model.TaskName,
                model.StatusID,
                model.PriorityID,
                model.TaskTypeID,
                ToSqlDateString(model.StartDate),
                ToSqlDateString(model.EndDate),
                ToSqlDecimalString(model.EstimatedHours),
                ToSqlDecimalString(model.ActualHours),
                model.CompletionPercentage,
                model.Description,
                username);

            return result.GetValueOrDefault(0);
        }
    }
}
