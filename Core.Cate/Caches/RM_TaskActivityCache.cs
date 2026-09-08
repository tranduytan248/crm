using Core.Cate.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Caching;
using System.ComponentModel;
using Core.Cate.Models;


namespace Core.Cate.Caches
{
    public class RM_TaskActivityCache : CacheLayer
    {
        private RM_TaskActivityBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_TaskActivityCache", "CENIT.APP.Cache" };
        private RM_TaskActivityBiz Api => _api ?? (_api = new RM_TaskActivityBiz());

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_TaskActivityModel> GetTaskActiveByTaskManagementID(int taskManagementID)
        {
            var key = $"GetTaskActiveByTaskManagementID{taskManagementID}";
            var data = GetCacheItem(key) as List<RM_TaskActivityModel>;
            if (data != null) return data;
            data = Api.GetByTaskManagementID(taskManagementID);
            AddCacheItem(key, data);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_TaskActivityModel model, string username)
        {
            var result = Api.Save(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }


    }
}
