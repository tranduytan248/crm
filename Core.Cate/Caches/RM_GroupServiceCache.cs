using Core.Cate.Biz;
using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_GroupServiceCache : CacheLayer
    {
        private RM_GroupServiceBiz _RM_GroupServiceApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_GroupServiceCach", "CENIT.APP.cache" };
        private RM_GroupServiceBiz Api => _RM_GroupServiceApi ?? (_RM_GroupServiceApi = new RM_GroupServiceBiz());
        /// <summary>
        /// Xóa GroupService
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_GroupServiceModel model, string username)
        {
            var result = Api.Delete(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        /// <summary>
        /// Lưu GroupService
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_GroupServiceModel model, string username)
        {
            var result = Api.Save(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách GroupService
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_GroupServiceModel> GetAll()
        {
            var rawKey = "GetAll_RM_GroupService";

            var data = GetCacheItem(rawKey) as List<RM_GroupServiceModel>;
            if (data != null) return data;

            data = Api.GetAll();
            AddCacheItem(rawKey, data);

            return data;
        }

        /// <summary>
        /// Lấy GroupService theo ID
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_GroupServiceModel GetById(int id)
        {
            if (id <= 0) return null;

            var rawKey = $"GetRM_GroupServiceByID_{id}";

            var data = GetCacheItem(rawKey) as RM_GroupServiceModel;
            if (data != null) return data;

            data = Api.LoadDetail(id);

            AddCacheItem(rawKey, data);

            return data;
        }

        /// <summary>
        /// Lấy danh sách GroupService theo search (Tree)
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_GroupServiceModel> Get(string search = "")
        {
            var rawKey = $"GetSearch_RM_GroupService_{search}";

            var data = GetCacheItem(rawKey) as List<RM_GroupServiceModel>;
            if (data != null) return data;

            data = Api.LoadList(search);

            AddCacheItem(rawKey, data);

            return data;
        }
    }
}
