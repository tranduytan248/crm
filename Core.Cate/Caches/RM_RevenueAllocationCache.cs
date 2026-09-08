using Core.Cate.Biz;
using Core.Cate.Models;
using System.Collections.Generic;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Processors;

namespace Core.Cate.Caches
{
    public class RM_RevenueAllocationCache : CacheLayer
    {
        private RM_RevenueAllocationBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_RevenueAllocationCache", "CENIT.APP.Cache" };
        private RM_RevenueAllocationBiz Api => _api ?? (_api = new RM_RevenueAllocationBiz());

        public List<RM_RevenueAllocationModel> GetByProductProjectID(int productProjectId)
        {
            var key = $"GetRM_RevenueAllocationByProductProject_{productProjectId}";
            var data = GetCacheItem(key) as List<RM_RevenueAllocationModel>;
            if (data != null) return data;
            data = Api.GetByProductProjectID(productProjectId);
            AddCacheItem(key, data);
            return data;
        }

        public int Save(RM_RevenueAllocationModel model, string username)
        {
            var result = Api.Save(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }
    }
}