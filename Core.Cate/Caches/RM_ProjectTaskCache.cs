using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_ProjectTaskCache : CacheLayer
    {
        private RM_ProjectTaskBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_ProjectTaskCache", "CENIT.APP.Cache" };
        private RM_ProjectTaskBiz Api => _api ?? (_api = new RM_ProjectTaskBiz());

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_ProjectTaskModel model, string username)
        {
            var result = Api.Delete(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_ProjectTaskModel model, string username)
        {
            var result = Api.Save(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_ProjectTaskModel GetById(int id)
        {
            var key = $"GetRM_ProjectTaskByID_{id}";
            var data = GetCacheItem(key) as RM_ProjectTaskModel;
            if (data != null) return data;
            data = Api.LoadDetail(id);
            AddCacheItem(key, data);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProjectTaskModel> GetByProductProjectID(int productProjectID)
        {
            var key = $"GetRM_ProjectTaskByProductProjectID_{productProjectID}";
            var data = GetCacheItem(key) as List<RM_ProjectTaskModel>;
            if (data != null) return data;
            data = Api.GetByProductProjectID(productProjectID);
            AddCacheItem(key, data);
            return data;
        }
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProjectTaskModel> GetByProjectID(int projectID)
        {
            var key = $"GetByProjectID_{projectID}";
            var data = GetCacheItem(key) as List<RM_ProjectTaskModel>;
            if (data != null) return data;
            data = Api.GetByProjectID(projectID);
            AddCacheItem(key, data);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProjectTaskModel> Get(out int total, BaseSearchModel search = null)
        {
            var key = $"GetSearch_RM_ProjectTask{UtilEncrypt.FromObject(search)}";
            var keyTotal = $"{key}-Total";
            total = (int?)GetCacheItem(keyTotal) ?? 0;
            var data = GetCacheItem(key) as List<RM_ProjectTaskModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(key, data);
            AddCacheItem(keyTotal, total);
            return data;
        }
    }
}