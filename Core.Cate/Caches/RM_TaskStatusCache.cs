using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Caching;

namespace Core.Cate.Caches
{
    public class RM_TaskStatusCache : CacheLayer
    {
        private RM_TaskStatusBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_TaskStatusCache", "CENIT.APP.Cache" };
        private RM_TaskStatusBiz Api => _api ?? (_api = new RM_TaskStatusBiz());

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_TaskStatusModel> GetAll()
        {
            const string key = "GetAll_RM_TaskStatus";
            var data = GetCacheItem(key) as List<RM_TaskStatusModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(key, data);
            return data;
        }
    }
}