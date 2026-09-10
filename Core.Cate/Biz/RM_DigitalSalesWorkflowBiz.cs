using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_DigitalSalesWorkflowBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";

        #region Status Procedures
        public List<RM_DigitalSalesStatusModel> GetStatuses(out int total, string search = null, byte? businessType = null, string order = "0", string orderDir = "ASC", int pageIndex = 0, int pageSize = 100)
        {
            total = 0;
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesStatusModel>(
                "RM_DigitalSalesStatus_Get",
                DATA_PROVIDER_NAME,
                string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
                businessType.HasValue ? (object)businessType.Value : DBNull.Value,
                order,
                orderDir,
                pageIndex,
                pageSize
            );

            if (data != null && data.Count > 0)
            {
                total = data.Count;
            }

            return data ?? new List<RM_DigitalSalesStatusModel>();
        }

        public List<RM_DigitalSalesStatusModel> GetAllStatuses(byte? businessType = null)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesStatusModel>(
                "RM_DigitalSalesStatus_GetAll",
                DATA_PROVIDER_NAME,
                businessType.HasValue ? (object)businessType.Value : DBNull.Value
            );
            return data ?? new List<RM_DigitalSalesStatusModel>();
        }

        public RM_DigitalSalesStatusModel GetStatusByID(int statusId)
        {
            if (statusId <= 0) return null;
            return AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_DigitalSalesStatusModel>(
                "RM_DigitalSalesStatus_GetByID",
                DATA_PROVIDER_NAME,
                statusId
            );
        }

        public int SaveStatus(RM_DigitalSalesStatusModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                "RM_DigitalSalesStatus_Save",
                DATA_PROVIDER_NAME,
                model.StatusID,
                model.BusinessType,
                model.StatusCode,
                model.StatusName,
                model.Description,
                model.SortOrder,
                model.IsDefault,
                model.IsActive,
                username
            );
            return result.GetValueOrDefault(0);
        }

        public int DeleteStatus(int statusId, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                "RM_DigitalSalesStatus_Delete",
                DATA_PROVIDER_NAME,
                statusId,
                username
            );
            return result.GetValueOrDefault(0);
        }
        #endregion

        #region Process Procedures
        public List<RM_DigitalSalesProcessModel> GetProcesses(out int total, string search = null, byte? businessType = null, int? statusId = null, string order = "0", string orderDir = "ASC", int pageIndex = 0, int pageSize = 100)
        {
            total = 0;
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesProcessModel>(
                "RM_DigitalSalesProcess_Get",
                DATA_PROVIDER_NAME,
                string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
                businessType.HasValue ? (object)businessType.Value : DBNull.Value,
                statusId.HasValue ? (object)statusId.Value : DBNull.Value,
                order,
                orderDir,
                pageIndex,
                pageSize
            );

            if (data != null && data.Count > 0)
            {
                total = data.First().TotalRow.GetValueOrDefault(0);
            }

            return data ?? new List<RM_DigitalSalesProcessModel>();
        }

        public RM_DigitalSalesProcessModel GetProcessByID(int processId)
        {
            if (processId <= 0) return null;
            return AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_DigitalSalesProcessModel>(
                "RM_DigitalSalesProcess_GetByID",
                DATA_PROVIDER_NAME,
                processId
            );
        }

        public int SaveProcess(RM_DigitalSalesProcessModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                "RM_DigitalSalesProcess_Save",
                DATA_PROVIDER_NAME,
                model.ProcessID,
                model.StatusID,
                model.ProcessCode,
                model.ProcessName,
                model.Description,
                model.SortOrder,
                model.IsActive,
                username
            );
            return result.GetValueOrDefault(0);
        }

        public int DeleteProcess(int processId, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                "RM_DigitalSalesProcess_Delete",
                DATA_PROVIDER_NAME,
                processId,
                username
            );
            return result.GetValueOrDefault(0);
        }
        #endregion

        #region Progress Procedures
        public List<RM_DigitalSalesProgressModel> GetProgressesByProcess(int processId)
        {
            if (processId <= 0) return new List<RM_DigitalSalesProgressModel>();
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesProgressModel>(
                "RM_DigitalSalesProgress_GetByProcess",
                DATA_PROVIDER_NAME,
                processId
            );
            return data ?? new List<RM_DigitalSalesProgressModel>();
        }

        public RM_DigitalSalesProgressModel GetProgressByID(int progressId)
        {
            if (progressId <= 0) return null;
            return AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_DigitalSalesProgressModel>(
                "RM_DigitalSalesProgress_GetByID",
                DATA_PROVIDER_NAME,
                progressId
            );
        }

        public int SaveProgress(RM_DigitalSalesProgressModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                "RM_DigitalSalesProgress_Save",
                DATA_PROVIDER_NAME,
                model.ProgressID,
                model.ProcessID,
                model.ProgressCode,
                model.ProgressName,
                model.Description,
                model.DefaultDurationDays <= 0 ? 3 : model.DefaultDurationDays,
                model.SortOrder,
                model.IsActive,
                username
            );
            return result.GetValueOrDefault(0);
        }

        public int DeleteProgress(int progressId, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                "RM_DigitalSalesProgress_Delete",
                DATA_PROVIDER_NAME,
                progressId,
                username
            );
            return result.GetValueOrDefault(0);
        }
        #endregion
    }
}
