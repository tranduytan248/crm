using System;
using System.Collections.Generic;
using System.Linq;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    /// <summary>
    /// Xử lý truy vấn và cập nhật dữ liệu dự án cùng các tập dữ liệu xuất báo cáo của dự án.
    /// </summary>
    public class RM_ProjectBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_Project_Save = "RM_Project_Save";
        private readonly string _RM_Project_Convert = "RM_Project_Convert";
        private readonly string _RM_Project_Get = "RM_Project_Get";
        private readonly string _RM_Project_Delete = "RM_Project_Delete";
        private readonly string _RM_Project_GetByID = "RM_Project_GetByID";
        private readonly string _RM_Project_Export_Projects = "RM_Project_Export_Projects";
        private readonly string _RM_Project_Export_Products = "RM_Project_Export_Products";
        private readonly string _RM_Project_Export_Members = "RM_Project_Export_Members";
        private readonly string _RM_Project_Export_Tasks = "RM_Project_Export_Tasks";

        /// <summary>
        /// Lấy danh sách dự án theo điều kiện tìm kiếm và phân trang.
        /// </summary>
        public List<RM_ProjectModel> LoadList(out int total, RM_ProjectSearchModel model, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProjectModel>(_RM_Project_Get,
                DATA_PROVIDER_NAME,
                string.IsNullOrEmpty(model.Keyword) ? (object)DBNull.Value : model.Keyword,
                model.CustomerTypeID == 0 ? (object)DBNull.Value : model.CustomerTypeID,
                model.StatusIDs,
                model.Year == 0 ? (object)DBNull.Value : model.Year,
                model.BoPhanID,
                model.EmployeeID,
                model.SuccessRateFrom,
                model.SuccessRateTo,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize,
                model.UserName);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách dự án.
        /// </summary>
        public List<RM_ProjectModel> GetAll()
        {
            int total;
            var model = new RM_ProjectSearchModel();
            var list = LoadList(out total, model, null);
            return list;
        }

        /// <summary>
        /// Lấy chi tiết dự án theo mã dữ liệu.
        /// </summary>
        public RM_ProjectModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ProjectModel>(_RM_Project_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa thông tin dự án.
        /// </summary>
        public int Delete(RM_ProjectModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Project_Delete, DATA_PROVIDER_NAME, model.ProjectID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Lưu thông tin dự án.
        /// </summary>
        public int Save(RM_ProjectModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Project_Save, DATA_PROVIDER_NAME,
                model.ProjectID,
                model.ProjectName,
                model.ContractID,
                model.CustomerID,
                model.StartDate,
                model.Status,
                model.Note,
                model.SuccessRate,
                username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Chuyển cơ hội kinh doanh thành dự án.
        /// </summary>
        public int ConvertProject(RM_ProjectModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Project_Convert, DATA_PROVIDER_NAME,
                model.ProjectID,
                model.BusinessOpportunityID,
                model.ProjectName,
                model.ContractID,
                model.CustomerID,
                model.StartDate,
                model.Note,
                model.SuccessRate,
                username);
            return result.GetValueOrDefault(0);
        }
        private object[] BuildExportParams(RM_ProjectSearchModel model)
        {
            return new object[]
            {
                string.IsNullOrEmpty(model.Keyword)    ? (object)DBNull.Value : model.Keyword,
                model.CustomerTypeID == 0              ? (object)DBNull.Value : model.CustomerTypeID,
                model.StatusID == 0                    ? (object)DBNull.Value : model.StatusID,
                string.IsNullOrEmpty(model.UserName)   ? (object)DBNull.Value : model.UserName
            };
        }

        /// <summary>
        /// Lấy dữ liệu xuất danh sách dự án.
        /// </summary>
        public List<ProjectExportRow> ExportProjects(RM_ProjectSearchModel model)
        {
            var p = BuildExportParams(model);
            return AppProcessor.ProcedureProvider.ExecuteTypedList<ProjectExportRow>(
                _RM_Project_Export_Projects, DATA_PROVIDER_NAME, p[0], p[1], p[2], p[3]);
        }

        /// <summary>
        /// Lấy dữ liệu xuất danh sách sản phẩm dịch vụ của dự án.
        /// </summary>
        public List<ProductExportRow> ExportProducts(RM_ProjectSearchModel model)
        {
            var p = BuildExportParams(model);
            return AppProcessor.ProcedureProvider.ExecuteTypedList<ProductExportRow>(
                _RM_Project_Export_Products, DATA_PROVIDER_NAME, p[0], p[1], p[2], p[3]);
        }

        /// <summary>
        /// Lấy dữ liệu xuất danh sách thành viên dự án.
        /// </summary>
        public List<MemberExportRow> ExportMembers(RM_ProjectSearchModel model)
        {
            var p = BuildExportParams(model);
            return AppProcessor.ProcedureProvider.ExecuteTypedList<MemberExportRow>(
                _RM_Project_Export_Members, DATA_PROVIDER_NAME, p[0], p[1], p[2], p[3]);
        }

        /// <summary>
        /// Lấy dữ liệu xuất danh sách công việc dự án.
        /// </summary>
        public List<TaskExportRow> ExportTasks(RM_ProjectSearchModel model)
        {
            var p = BuildExportParams(model);
            return AppProcessor.ProcedureProvider.ExecuteTypedList<TaskExportRow>(
                _RM_Project_Export_Tasks, DATA_PROVIDER_NAME, p[0], p[1], p[2], p[3]);
        }
    }
}
