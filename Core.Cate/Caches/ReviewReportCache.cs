using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using System.ComponentModel;
using TSFramework.Libs.Utils;


namespace Core.Cate.Caches
{
    public class ReviewReportCache : CacheLayer
    {
        private ReviewReportBiz _ReviewReportApi;
        protected override string[] MasterCacheKeyArray => new[] { "ReviewReportCache", "CENIT.APP.Cache" };
        private ReviewReportBiz Api => _ReviewReportApi ?? (_ReviewReportApi = new ReviewReportBiz());

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<ReviewReportModel> Get(out int total, ReviewReportSearchModel searchModel = null, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetReviewReport_", UtilEncrypt.FromObject(searchModel), UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            var data = GetCacheItem(rawKey) as List<ReviewReportModel>;
            if (data != null) return data;

            data = Api.LoadList(out total, searchModel, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }
    }
}
