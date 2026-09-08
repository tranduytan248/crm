using System;
using System.Collections.Generic;
using Core.Cate.Models;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_ProjectRevenueReportBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _reportProcedure = "RM_ProjectRevenueReport_Get";

        public List<RM_ProjectRevenueReportModel> GetReport(DateTime fromDate, DateTime toDate, string userName)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProjectRevenueReportModel>(
                _reportProcedure,
                DATA_PROVIDER_NAME,
                fromDate,
                toDate,
                userName);
        }
    }
}
