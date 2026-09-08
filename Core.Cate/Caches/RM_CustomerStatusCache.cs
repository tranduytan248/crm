using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using Core.Cate.Biz;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_CustomerStatusCache : CacheLayer
    {
        private RM_CustomerStatusBiz _RM_CustomerStatusApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_CustomerStatusCache", "CENIT.APP.Cache" };

        private RM_CustomerStatusBiz Api => _RM_CustomerStatusApi ?? (_RM_CustomerStatusApi = new RM_CustomerStatusBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_CustomerStatusModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_CustomerStatus
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_CustomerStatusModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_CustomerStatus
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_CustomerStatusModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_CustomerStatus");
            var data = GetCacheItem(rawKey) as List<RM_CustomerStatusModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tinRM_CustomerStatus theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_CustomerStatusModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_CustomerStatusByID_", ID);
            var data = GetCacheItem(rawKey) as RM_CustomerStatusModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_CustomerStatus
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_CustomerStatusModel> Get(out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_CustomerStatus", UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_CustomerStatusModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
    }
}
