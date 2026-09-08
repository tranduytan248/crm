using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Caching;

namespace Core.Cate.Caches
{
    public class RM_PriorityCache : CacheLayer
    {
        private RM_PriorityBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_PriorityCache", "CENIT.APP.Cache" };
        private RM_PriorityBiz Api => _api ?? (_api = new RM_PriorityBiz());

        /// <summary>
        /// Lấy toàn bộ danh sách độ ưu tiên từ cache hoặc nguồn dữ liệu.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_PriorityModel> GetAll()
        {
            const string key = "GetAllRM_Priority";
            var data = GetCacheItem(key) as List<RM_PriorityModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(key, data);
            return data;
        }
    }
}
