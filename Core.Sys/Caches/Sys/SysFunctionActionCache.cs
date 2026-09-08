using System.Collections.Generic;
using System.ComponentModel;
using Core.Sys.Biz.Sys;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Caching;

namespace Core.Sys.Caches.Sys
{
    [DataObject]
    public class SysFunctionActionCache : CacheLayer
    {
        private SysFunctionActionBiz _functionActionApi;

        private SysFunctionActionBiz Api => _functionActionApi ?? (_functionActionApi = new SysFunctionActionBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "SysFunctionsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysFunctionActionModel> GetAll()
        {
            const string rawKey = "AllFunctionActions";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysFunctionActionModel> functionActions) return functionActions;
            // Item not found in cache - retrieve it and insert it into the cache
            functionActions = Api.GetAll();
            AddCacheItem(rawKey, functionActions);

            return functionActions;
        }
    }
}