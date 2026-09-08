using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Core.Log.Biz;
using Core.Log.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Log.Caches
{
    public class AccessHistoryCache : CacheLayer
    {
        private AccessHistoryBiz _dbAccessHistoryBiz;
        protected override string[] MasterCacheKeyArray => new[]
        {
            "AccessHistoryCache","CENIT.APP.Cache"
        };

        private AccessHistoryBiz Api => _dbAccessHistoryBiz ?? (_dbAccessHistoryBiz = new AccessHistoryBiz());

      
        /// <summary>
        /// Luu hanh chinh tinh vao database
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public int Save(AccessHistoryModel model)
        {
            var configId = Api.Save(model);
            if (configId > 0)
                // Invalidate the cache
                InvalidateCache();
            return configId;
        }
        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<AccessHistoryModel> Get(out int total, AccessHistorySearchModel searchModel, BaseSearchModel search = null)
        {
            var objectKey = UtilEncrypt.FromObject(search);
            var objectKey2 = UtilEncrypt.FromObject(searchModel);

            var rawKey = string.Concat("ListChucVus-", objectKey, objectKey2);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var lhds = GetCacheItem(rawKey) as List<AccessHistoryModel>;
            if (lhds != null) return lhds;
            // Item not found in cache - retrieve it and insert it into the cache
            lhds = Api.GetList(out total, searchModel, search);
            if (lhds == null) return null;
            AddCacheItem(rawKey, lhds);
            AddCacheItem(rawKeyTotal, total);

            return lhds;
        }
    }
}