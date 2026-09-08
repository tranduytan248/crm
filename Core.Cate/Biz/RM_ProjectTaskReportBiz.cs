using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_ProjectTaskReportBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _reportProcedure = "RM_Project_TaskReport_Get";

        public List<RM_ProjectTaskReportModel> GetReport(DateTime fromDate, DateTime toDate, string userName)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProjectTaskReportModel>(
                _reportProcedure,
                DATA_PROVIDER_NAME,
                fromDate,
                toDate,
                userName);
        }
    }
}
