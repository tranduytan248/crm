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
    public class RM_ExchangeHistoryCache : CacheLayer
    {
        private RM_ExchangeHistoryBiz _RM_ExchangeHistoryApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_ExchangeHistoryCache", "CENIT.APP.Cache" };

        private RM_ExchangeHistoryBiz Api => _RM_ExchangeHistoryApi ?? (_RM_ExchangeHistoryApi = new RM_ExchangeHistoryBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_ExchangeHistoryModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_ExchangeHistory
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_ExchangeHistoryModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ExchangeHistory
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ExchangeHistoryModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_ExchangeHistory");
            var data = GetCacheItem(rawKey) as List<RM_ExchangeHistoryModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tinRM_ExchangeHistory theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_ExchangeHistoryModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ExchangeHistoryByID_", ID);
            var data = GetCacheItem(rawKey) as RM_ExchangeHistoryModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ExchangeHistory
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ExchangeHistoryModel> Get(RM_ExchangeHistorySearchModel model, out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_ExchangeHistory", UtilEncrypt.FromObject(search), UtilEncrypt.FromObject(model));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_ExchangeHistoryModel>;
            if (data != null) return data;
            data = Api.LoadList(model, out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
    }
}
