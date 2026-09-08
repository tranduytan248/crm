using Core.Sys.Biz.Sys;
using Core.Sys.Models.Sys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Caching;

namespace Core.Sys.Caches.Sys
{
    public class SysInstructCache : CacheLayer
    {
        private SysInstructBiz _instructApi;
        private SysInstructBiz Api => _instructApi ?? (_instructApi = new SysInstructBiz());
        protected override string[] MasterCacheKeyArray => new[] { "SysInstructCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysInstructModel> Get(out int total, TSFramework.Libs.Models.Base.BaseSearchModel search = null)
        {
            var objectKey = TSFramework.Libs.Utils.UtilEncrypt.FromObject(search);
            var rawKey = string.Concat("ListInstructs-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            // See if the item is in the cache
            var instructs = GetCacheItem(rawKey) as List<SysInstructModel>;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0; 

            if (instructs != null)
            {
                total = cacheTotal ?? instructs.Count;
                return instructs;
            }
            // Item not found in cache - retrieve it and insert it into the cache
            instructs = Api.Get(out total, search);
            AddCacheItem(rawKey, instructs);
            AddCacheItem(rawKeyTotal, total);

            return instructs;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysInstructModel GetById(int instructId)
        {
            if (instructId < 0) return null;

            var rawKey = string.Concat("SysInstructByID-", instructId);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is SysInstructModel instruct) return instruct;
            // Item not found in cache - retrieve it and insert it into the cache
            instruct = Api.LoadDetail(instructId);
            AddCacheItem(rawKey, instruct);

            return instruct;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysInstructModel> GetAll()
        {
            const string rawKey = "AllInstructs";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysInstructModel> instructs) return instructs;
            // Item not found in cache - retrieve it and insert it into the cache
            instructs = Api.Get(out _, null);
            AddCacheItem(rawKey, instructs);

            return instructs;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(SysInstructModel model, string username)
        {
            var result = Api.Delete(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int Save(SysInstructModel model, string username)
        {
            var instructId = Api.Save(model, username);
            // Invalidate the cache
            InvalidateCache();
            return instructId;
        }
    }
}
