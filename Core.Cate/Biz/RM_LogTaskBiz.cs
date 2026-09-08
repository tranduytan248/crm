using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    /// <summary>
    /// Xử lý truy vấn và cập nhật dữ liệu nhật ký công việc cùng file đính kèm của nhật ký.
    /// </summary>
    public class RM_LogTaskBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";

        private readonly string _RM_LogTask_Get = "RM_LogTask_Get";
        private readonly string _RM_LogTask_GetByID = "RM_LogTask_GetByID";
        private readonly string _RM_LogTask_GetByProjectTaskID = "RM_LogTask_GetByProjectTaskID";
        private readonly string _RM_LogTask_Save = "RM_LogTask_Save";
        private readonly string _RM_LogTask_Delete = "RM_LogTask_Delete";

        private readonly string _RM_LogTaskFilePath_Get = "RM_LogTaskFilePath_Get";
        private readonly string _RM_LogTaskFilePath_GetByID = "RM_LogTaskFilePath_GetByID";
        private readonly string _RM_LogTaskFilePath_Save = "RM_LogTaskFilePath_Save";
        private readonly string _RM_LogTaskFilePath_Delete = "RM_LogTaskFilePath_Delete";

        /// <summary>
        /// Lấy danh sách nhật ký công việc theo điều kiện tìm kiếm và phân trang.
        /// </summary>
        public List<RM_LogTaskModel> LoadList(out int total, BaseSearchModel search, int? taskManagementID = null)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "0", OrderDir = "DESC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_LogTaskModel>(
                _RM_LogTask_Get, DATA_PROVIDER_NAME,
                search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize,
                taskManagementID);  // thêm param mới
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy danh sách nhật ký theo công việc dự án.
        /// </summary>
        public List<RM_LogTaskModel> GetByProjectTaskID(int projectTaskID)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_LogTaskModel>(
                _RM_LogTask_GetByProjectTaskID, DATA_PROVIDER_NAME, projectTaskID);
        }

        /// <summary>
        /// Lấy chi tiết nhật ký công việc theo mã dữ liệu.
        /// </summary>
        public RM_LogTaskModel LoadDetail(int id)
        {
            return AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_LogTaskModel>(
                _RM_LogTask_GetByID, DATA_PROVIDER_NAME, id);
        }

        /// <summary>
        /// Xóa nhật ký công việc.
        /// </summary>
        public int Delete(RM_LogTaskModel model, string username)
        {
            return AppProcessor.ProcedureProvider.Execute(
                _RM_LogTask_Delete, DATA_PROVIDER_NAME, model.LogTaskID, username).GetValueOrDefault(0);
        }

        /// <summary>
        /// Lưu thông tin nhật ký công việc.
        /// </summary>
        public int Save(RM_LogTaskModel model, string username)
        {
            // combine Date + Time
            var startDateTime = model.LogDate.Value.Date + model.StartTime.Value;
            var endDateTime = model.LogDate.Value.Date + model.EndTime.Value;

            // validate
            if (endDateTime <= startDateTime)
            {
                throw new Exception("Thời gian kết thúc phải lớn hơn thời gian bắt đầu");
            }

            return AppProcessor.ProcedureProvider.Execute(
                _RM_LogTask_Save,
                DATA_PROVIDER_NAME,
                model.LogTaskID,
                model.TaskManagementID,   
                model.Employee_ID,
                model.LogDate,
                startDateTime,            
                endDateTime,              
                model.Description,
                username
            ).GetValueOrDefault(0);
        }

        /// <summary>
        /// Lấy danh sách file đính kèm của nhật ký công việc theo điều kiện tìm kiếm.
        /// </summary>
        public List<RM_LogTaskFilePathModel> GetFilePaths(RM_LogTaskFilePathSearchModel model, out int total, BaseSearchModel search)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_LogTaskFilePathModel>(
                _RM_LogTaskFilePath_Get, DATA_PROVIDER_NAME,
                model.LogTaskID,
                model.TaskManagementID,  
                model.CommentID,         
                search?.Search,
                search?.Order ?? "0",
                search?.OrderDir ?? "ASC",
                search?.StartIndex ?? 0,
                search?.PageSize ?? -1);

            total = data?.Count ?? 0;
            return data;
        }
        /// <summary>
        /// Lấy chi tiết file đính kèm của nhật ký công việc theo mã dữ liệu.
        /// </summary>
        public RM_LogTaskFilePathModel GetFilePathById(int id)
        {
            return AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_LogTaskFilePathModel>(
                _RM_LogTaskFilePath_GetByID, DATA_PROVIDER_NAME, id);
        }

        /// <summary>
        /// Lưu thông tin file đính kèm của nhật ký công việc.
        /// </summary>
        public int SaveFilePath(RM_LogTaskFilePathModel model, string username)
        {
            return AppProcessor.ProcedureProvider.Execute(
                _RM_LogTaskFilePath_Save, DATA_PROVIDER_NAME,
                model.FilePathID, model.LogTaskID, model.TaskManagementID, model.CommentID, model.FilePath, username).GetValueOrDefault(0);
        }

        /// <summary>
        /// Xóa thông tin file đính kèm của nhật ký công việc.
        /// </summary>
        public int DeleteFilePath(RM_LogTaskFilePathModel model, string username)
        {
            return AppProcessor.ProcedureProvider.Execute(
                _RM_LogTaskFilePath_Delete, DATA_PROVIDER_NAME, model.FilePathID, username).GetValueOrDefault(0);
        }
    }
}
