using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_TaskCache : CacheLayer
    {
        private RM_TaskBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_TaskCache", "CENIT.APP.Cache" };
        private RM_TaskBiz Api => _api ?? (_api = new RM_TaskBiz());

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_TaskModel model, string username)
        {
            var result = Api.Delete(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_TaskModel model, string username)
        {
            var result = Api.Save(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_TaskModel> GetAll()
        {
            var key = "GetAllRM_Task";
            var data = GetCacheItem(key) as List<RM_TaskModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(key, data);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_TaskModel GetById(int id)
        {
            var key = $"GetRM_TaskByID_{id}";
            var data = GetCacheItem(key) as RM_TaskModel;
            if (data != null) return data;
            data = Api.LoadDetail(id);
            AddCacheItem(key, data);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_TaskModel> Get(out int total, RM_TaskSearchModel model, BaseSearchModel search = null)
        {
            var key = $"GetSearch_RM_Task{UtilEncrypt.FromObject(search)}{UtilEncrypt.FromObject(model)}";
            var keyTotal = $"{key}-Total";
            total = (int?)GetCacheItem(keyTotal) ?? 0;
            var data = GetCacheItem(key) as List<RM_TaskModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, model, search);
            AddCacheItem(key, data);
            AddCacheItem(keyTotal, total);
            return data;
        }
    }
}
