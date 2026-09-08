using Core.Cate.Models;
using System;
using System.Collections.Generic;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_BusinessOpportunityWeeklyReportBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _reportProcedure = "RM_BusinessOpportunity_WeeklyReport_Get";

        public List<RM_BusinessOpportunityWeeklyReportModel> GetReport(DateTime fromDate, DateTime toDate, int boPhanId, string employeeIds, string userName)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_BusinessOpportunityWeeklyReportModel>(
                _reportProcedure,
                DATA_PROVIDER_NAME,
                fromDate,
                toDate,
                boPhanId,
                employeeIds,
                userName);
        }
    }
}
