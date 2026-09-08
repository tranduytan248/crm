using System;
using System.Collections.Generic;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysProcedureLogBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _sysProcedureLogDelete = "Sys_ProcedureLog_Delete";
        private readonly string _sysProcedureLogDeleteAll = "Sys_ProcedureLog_DeleteAll";
        private readonly string _sysProcedureLogGet = "Sys_ProcedureLog_Get";
        private readonly string _sysProcedureLogGetById = "Sys_ProcedureLog_GetByID";

        private List<SysProcedureLogModel> Get(DateTime? fromMonth, DateTime? toMonth)
        {
            var listProcedureLogs = AppProcessor.ProcedureProvider.ExecuteTypedList<SysProcedureLogModel>(_sysProcedureLogGet, DATA_PROVIDER_NAME,
                fromMonth, toMonth);

            return listProcedureLogs;
        }

        private SysProcedureLogModel LoadDetail(Guid? logId)
        {
            var dataProcedureLog =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysProcedureLogModel>(_sysProcedureLogGetById, DATA_PROVIDER_NAME,
                    logId);
            return dataProcedureLog;
        }

        public bool Delete(SysProcedureLogModel model)
        {
            var result =
                AppProcessor.ProcedureProvider.Execute(_sysProcedureLogDelete, DATA_PROVIDER_NAME, model.LogId);
            return result == 1;
        }

        public bool DeleteAll(DateTime? fromMonth, DateTime? toMonth)
        {
            var result =
                AppProcessor.ProcedureProvider.Execute(_sysProcedureLogDeleteAll, DATA_PROVIDER_NAME, fromMonth, toMonth);
            return result == 1;
        }

        public List<SysProcedureLogModel> GetAll(DateTime? fromMonth, DateTime? toMonth)
        {
            var listProcedureLogs = Get(fromMonth, toMonth);
            return listProcedureLogs;
        }

        public SysProcedureLogModel GetById(Guid? logId)
        {
            var listProcedureLogs = LoadDetail(logId);
            return listProcedureLogs;
        }


        public List<SysProcedureLogModel> GetList(DateTime? fromMonth, DateTime? toMonth)
        {
            var listProcedureLogs = Get(fromMonth, toMonth);
            return listProcedureLogs;
        }
    }
}