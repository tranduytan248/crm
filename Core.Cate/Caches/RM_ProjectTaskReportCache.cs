using Core.Cate.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Caching;

namespace Core.Cate.Caches
{
    public class RM_ProjectTaskReportCache : CacheLayer
    {
        private RM_ProjectTaskReportBiz _api;

        protected override string[] MasterCacheKeyArray => new[] { "RM_ProjectTaskReportCache", "CENIT.APP.Cache" };

        private RM_ProjectTaskReportBiz Api => _api ?? (_api = new RM_ProjectTaskReportBiz());

        public List<Models.RM_ProjectTaskReportModel> GetReport(DateTime fromDate, DateTime toDate, string userName)
        {
            var rawKey = string.Concat(
                "GetRM_ProjectTaskReport_",
                fromDate.ToString("yyyyMMdd"),
                "_",
                toDate.ToString("yyyyMMdd"),
                "_",
                userName);

            var data = GetCacheItem(rawKey) as List<Models.RM_ProjectTaskReportModel>;
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
