using Core.Cate.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Caching;

namespace Core.Cate.Caches
{
    public class RM_ProjectWeeklyTaskReportCache : CacheLayer
    {
        private RM_ProjectWeeklyTaskReportBiz _api;

        protected override string[] MasterCacheKeyArray => new[] { "RM_ProjectWeeklyTaskReportCache", "CENIT.APP.Cache" };

        private RM_ProjectWeeklyTaskReportBiz Api => _api ?? (_api = new RM_ProjectWeeklyTaskReportBiz());

        public List<Models.RM_ProjectWeeklyTaskReportModel> GetReport(DateTime fromDate, DateTime toDate, string userName)
        {
            var rawKey = string.Concat(
                "GetRM_ProjectWeeklyTaskReport_",
                fromDate.ToString("yyyyMMdd"),
                "_",
                toDate.ToString("yyyyMMdd"),
                "_",
                userName);

            var data = GetCacheItem(rawKey) as List<Models.RM_ProjectWeeklyTaskReportModel>;
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
