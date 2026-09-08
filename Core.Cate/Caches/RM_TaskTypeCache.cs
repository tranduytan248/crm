using Core.Cate.Biz;
using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Caching;

namespace Core.Cate.Caches
{
    public class RM_TaskTypeCache : CacheLayer
    {
        private RM_TaskTypeBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_TaskTypeCache", "CENIT.APP.Cache" };
        private RM_TaskTypeBiz Api => _api ?? (_api = new RM_TaskTypeBiz());

        /// <summary>
        /// Lấy toàn bộ danh sách loại công việc từ cache hoặc nạp từ nguồn dữ liệu.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_TaskTypeModel> GetAll()
        {
            const string key = "GetAllRM_TaskType";
            var data = GetCacheItem(key) as List<RM_TaskTypeModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(key, data);
            return data;
        }
    }
}
