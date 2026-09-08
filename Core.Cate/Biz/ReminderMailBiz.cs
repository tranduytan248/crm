using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web.Helpers;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class ReminderMailBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _reminder_Get = "Project_Opportunity_GetReminder";


        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_CostType</returns>
        public List<ReminderProjectOpportunityModel> GetReminderList(DateTime fromDate, DateTime toDate)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<ReminderProjectOpportunityModel>(
                _reminder_Get,
                DATA_PROVIDER_NAME,
                fromDate,
                toDate);
        }
    }
}
