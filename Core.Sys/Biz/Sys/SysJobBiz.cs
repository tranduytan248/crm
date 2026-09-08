using System.Collections.Generic;
using System.Linq;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysJobBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _sysJobChangeStatus = "Sys_Job_ChangeStatus";
        private readonly string _sysJobDelete = "Sys_Job_Delete";
        private readonly string _sysJobGet = "Sys_Job_Get";
        private readonly string _sysJobGetByID = "Sys_Job_GetByID";
        private readonly string _sysJobSave = "Sys_Job_Save";

        private List<SysJobModel> Get(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listJob = AppProcessor.ProcedureProvider.ExecuteTypedList<SysJobModel>(_sysJobGet, DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listJob != null && listJob.Count > 0)
                total = int.Parse(listJob.First()?.TotalRow.ToString() ?? "0");
            return listJob;
        }

        private SysJobModel LoadDetail(string configId)
        {
            var dataJob =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysJobModel>(_sysJobGetByID, DATA_PROVIDER_NAME,
                    configId);
            return dataJob;
        }

        public bool Delete(SysJobModel model)
        {
            var result =
                AppProcessor.ProcedureProvider.Execute(_sysJobDelete, DATA_PROVIDER_NAME, model.JobId, model.SavedBy);
            return result == 1;
        }

        public List<SysJobModel> GetAll()
        {
            var listJob = Get(out _, null);
            return listJob;
        }

        public SysJobModel GetById(string configId)
        {
            var configs = LoadDetail(configId);
            return configs;
        }


        public List<SysJobModel> GetList(out int total, BaseSearchModel search = null)
        {
            var listJob = Get(out total, search);
            return listJob;
        }

        public int Save(SysJobModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysJobSave, DATA_PROVIDER_NAME,
                model.JobId,
                model.JobName,
                model.JobDescription,
                model.CronExpression,
                model.JobLibrary,
                model.JobParrams,
                model.IsActive,
                model.SavedBy);

            return result.GetValueOrDefault(0);
        }

        public bool ChangeStatus(SysJobModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysJobChangeStatus, DATA_PROVIDER_NAME, model.JobId,
                model.IsActive, model.SavedBy);
            return result == 1;
        }
    }
}