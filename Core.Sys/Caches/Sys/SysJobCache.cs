using System.Collections.Generic;
using System.ComponentModel;
using Core.Sys.Biz.Sys;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Sys.Caches.Sys
{
    [DataObject]
    public class SysJobCache : CacheLayer
    {
        private SysJobBiz _configsApi;

        private SysJobBiz Api => _configsApi ?? (_configsApi = new SysJobBiz());
        protected override string[] MasterCacheKeyArray => new[] { "SysJobCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysJobModel> GetAll()
        {
            const string rawKey = "AllJob";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysJobModel> configs) return configs;
            // Item not found in cache - retrieve it and insert it into the cache
            configs = Api.GetAll();
            AddCacheItem(rawKey, configs);

            return configs;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysJobModel> Get(out int total, BaseSearchModel search = null)
        {
            var objectKey = UtilEncrypt.FromObject(search);
            var rawKey = string.Concat("ListJobs-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            // See if the item is in the cache
            var configs = GetCacheItem(rawKey) as List<SysJobModel>;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            if (configs != null) return configs;
            // Item not found in cache - retrieve it and insert it into the cache
            configs = Api.GetList(out total, search);
            AddCacheItem(rawKey, configs);
            AddCacheItem(rawKeyTotal, total);

            return configs;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysJobModel GetById(string configId)
        {
            if (string.IsNullOrEmpty(configId)) return null;

            var rawKey = string.Concat("SysJobByID-", configId);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is SysJobModel config) return config;
            // Item not found in cache - retrieve it and insert it into the cache
            config = Api.GetById(configId);
            if (config != null) AddCacheItem(rawKey, config);

            return config;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(SysJobModel model)
        {
            var configId = Api.Save(model);
            if (configId > 0)
                // Invalidate the cache
                InvalidateCache();
            return configId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysJobModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool ChangeStatus(SysJobModel model)
        {
            var isSuccess = Api.ChangeStatus(model);
            if (isSuccess)
                // Invalidate the cache
                InvalidateCache();
            return isSuccess;
        }
    }
}