using System;
using System.Collections.Generic;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_ProjectRevenueReportCache : CacheLayer
    {
        private RM_ProjectRevenueReportBiz _api;

        protected override string[] MasterCacheKeyArray => new[] { "RM_ProjectRevenueReportCache", "CENIT.APP.Cache" };

        private RM_ProjectRevenueReportBiz Api => _api ?? (_api = new RM_ProjectRevenueReportBiz());

        public List<RM_ProjectRevenueReportModel> GetReport(DateTime fromDate, DateTime toDate, string userName)
        {
            var rawKey = string.Concat(
                "GetRM_ProjectRevenueReport_",
                fromDate.ToString("yyyyMMdd"),
                "_",
                toDate.ToString("yyyyMMdd"),
                "_",
                UtilEncrypt.FromObject(userName));

            var data = GetCacheItem(rawKey) as List<RM_ProjectRevenueReportModel>;
            if (data != null)
            {
                return data;
            }

            data = Api.GetReport(fromDate, toDate, userName);
            AddCacheItem(rawKey, data);
            return data;
        }
    }
}
