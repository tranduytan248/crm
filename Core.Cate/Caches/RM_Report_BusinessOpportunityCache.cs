using Core.Cate.Biz;
using Core.Cate.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_Report_BusinessOpportunityCache : CacheLayer
    {
        private RM_Report_BusinessOpportunityBiz _RM_AnniversaryTypeApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_BusinessOpportunity_ReportCache", "CENIT.APP.Cache" };

        private RM_Report_BusinessOpportunityBiz Api => _RM_AnniversaryTypeApi ?? (_RM_AnniversaryTypeApi = new RM_Report_BusinessOpportunityBiz());

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_Report_BusinessOpportunityModel> Get(out int total, RM_Report_BusinessOpportunitySearchModel model, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetRM_BusinessOpportunity_Report", UtilEncrypt.FromObject(search), UtilEncrypt.FromObject(model));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_Report_BusinessOpportunityModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, model, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_Report_BusinessOpportunityModel> GetAll(RM_Report_BusinessOpportunitySearchModel model)
        {
            var rawKey = string.Concat("GetGetAllRM_BusinessOpportunity_Report");
            var data = GetCacheItem(rawKey) as List<RM_Report_BusinessOpportunityModel>;
            if (data != null) return data;
            data = Api.GetAll(model);
            AddCacheItem(rawKey, data);
            return data;
        }
    }
}
