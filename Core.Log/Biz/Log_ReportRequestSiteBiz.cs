using Core.Log.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Processors;

namespace Core.Log.Biz
{
    public class Log_ReportRequestSiteBiz
    {           
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Log";
        private readonly string _dl_Log_ReportRequestSite_Get = "Log_ReportRequestSite_Get";
        private readonly string _dl_Log_ReportRequestSite_Save = "Log_ReportRequestSite_Save";

        /// <summary>
        /// Get thong tin 
        /// </summary>
        /// <returns></returns>
        public Log_ReportRequestSiteTotalModel Get()
        {
            var data =
            AppProcessor.ProcedureProvider.ExecuteScalarObject<Log_ReportRequestSiteTotalModel>(_dl_Log_ReportRequestSite_Get, DATA_PROVIDER_NAME);
            return data;
        }

        /// <summary>
        /// luu thong tin vao database
        /// </summary>
        /// <returns></returns>
        public int Save(Log_ReportRequestSiteModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_dl_Log_ReportRequestSite_Save, DATA_PROVIDER_NAME,
                model.IPClient,
                model.BrowserClient
                );

            return result.GetValueOrDefault(0);
        }
    }
}
