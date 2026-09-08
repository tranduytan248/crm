using Core.Cate.Biz;
using Core.Cate.Models;
using System.Collections.Generic;
using TSFramework.Libs.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_TaskGroupCache : CacheLayer
    {
        private RM_TaskGroupBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_TaskGroupCache", "CENIT.APP.Cache" };
        private RM_TaskGroupBiz Api => _api ?? (_api = new RM_TaskGroupBiz());

        public int Delete(RM_TaskGroupModel model, string username)
        {
            var result = Api.Delete(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public int Save(RM_TaskGroupModel model, string username)
        {
            var result = Api.Save(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public List<RM_TaskGroupModel> GetAll()
        {
            var key = "GetAllRM_TaskGroup";
            var data = GetCacheItem(key) as List<RM_TaskGroupModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(key, data);
            return data;
        }

        public RM_TaskGroupModel GetById(int id)
        {
            var key = $"GetRM_TaskGroupByID_{id}";
            var data = GetCacheItem(key) as RM_TaskGroupModel;
            if (data != null) return data;
            data = Api.LoadDetail(id);
            AddCacheItem(key, data);
            return data;
        }

        public List<RM_TaskGroupModel> Get(out int total, BaseSearchModel search = null)
        {
            var key = $"GetSearch_RM_TaskGroup{UtilEncrypt.FromObject(search)}";
            var keyTotal = $"{key}-Total";
            total = (int?)GetCacheItem(keyTotal) ?? 0;
            var data = GetCacheItem(key) as List<RM_TaskGroupModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(key, data);
            AddCacheItem(keyTotal, total);
            return data;
        }
    }
}