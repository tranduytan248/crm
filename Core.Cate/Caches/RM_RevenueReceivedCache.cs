using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using Core.Cate.Biz;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_RevenueReceivedCache : CacheLayer
    {
        private RM_RevenueReceivedBiz _RM_RevenueReceivedApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_RevenueReceivedCache", "CENIT.APP.Cache" };

        private RM_RevenueReceivedBiz Api => _RM_RevenueReceivedApi ?? (_RM_RevenueReceivedApi = new RM_RevenueReceivedBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_RevenueReceivedModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_RevenueReceived
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_RevenueReceivedModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_RevenueReceived
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_RevenueReceivedModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_RevenueReceived");
            var data = GetCacheItem(rawKey) as List<RM_RevenueReceivedModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tinRM_RevenueReceived theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_RevenueReceivedModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_RevenueReceivedByID_", ID);
            var data = GetCacheItem(rawKey) as RM_RevenueReceivedModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tin RM_RevenueReceived theo ProductProjectID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_RevenueReceivedModel> GetByProductProjectID(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("Get_RM_RevenueReceivedByProductProjectID_", ID);
            var data = GetCacheItem(rawKey) as List<RM_RevenueReceivedModel>;
            if (data != null) return data;
            data = Api.GetByProductProjectID(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_RevenueReceived
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_RevenueReceivedModel> Get(out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_RevenueReceived", UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_RevenueReceivedModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
    }
}
