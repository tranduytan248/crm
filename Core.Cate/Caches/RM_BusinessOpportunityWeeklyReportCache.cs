using Core.Cate.Biz;
using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_BusinessOpportunityWeeklyReportCache : CacheLayer
    {
        private RM_BusinessOpportunityWeeklyReportBiz _api;

        protected override string[] MasterCacheKeyArray => new[] { "RM_BusinessOpportunityWeeklyReportCache", "CENIT.APP.Cache" };

        private RM_BusinessOpportunityWeeklyReportBiz Api => _api ?? (_api = new RM_BusinessOpportunityWeeklyReportBiz());

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_BusinessOpportunityWeeklyReportModel> GetReport(DateTime fromDate, DateTime toDate, int boPhanId, string employeeIds, string userName)
        {
            var rawKey = string.Concat(
                "GetRM_BusinessOpportunityWeeklyReport_",
                fromDate.ToString("yyyyMMdd"),
                "_",
                toDate.ToString("yyyyMMdd"),
                "_",
                boPhanId,
                "_",
                UtilEncrypt.FromObject(employeeIds),
                "_",
                userName);

            var data = GetCacheItem(rawKey) as List<RM_BusinessOpportunityWeeklyReportModel>;
            if (data != null)
            {
                return data;
            }

            data = Api.GetReport(fromDate, toDate, boPhanId, employeeIds, userName);
            AddCacheItem(rawKey, data);
            return data;
        }
    }
}
