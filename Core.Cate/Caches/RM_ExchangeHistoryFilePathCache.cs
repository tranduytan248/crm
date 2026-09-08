using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using Core.Cate.Models;
using Core.Cate.Biz;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_ExchangeHistoryFilePathCache : CacheLayer
    {
        private RM_ExchangeHistoryFilePathBiz _RM_ExchangeHistoryFilePathApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_ExchangeHistoryFilePathCache", "CENIT.APP.Cache" };

        private RM_ExchangeHistoryFilePathBiz Api => _RM_ExchangeHistoryFilePathApi ?? (_RM_ExchangeHistoryFilePathApi = new RM_ExchangeHistoryFilePathBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(int fileID, string deletedBy)
        {
            var isDeleted = Api.Delete(fileID, deletedBy);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_ExchangeHistoryFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_ExchangeHistoryFilePathModel model, string savedBy)
        {
            var isSaved = Api.Save(model, savedBy);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ExchangeHistoryFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ExchangeHistoryFilePathModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_ExchangeHistoryFilePath");
            var data = GetCacheItem(rawKey) as List<RM_ExchangeHistoryFilePathModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy thông tinRM_ExchangeHistoryFilePath theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_ExchangeHistoryFilePathModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ExchangeHistoryFilePathByID_", ID);
            var data = GetCacheItem(rawKey) as RM_ExchangeHistoryFilePathModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ExchangeHistoryFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ExchangeHistoryFilePathModel> Get(RM_ExchangeHistoryFilePathSearchModel model, out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_ExchangeHistoryFilePath", UtilEncrypt.FromObject(model), UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_ExchangeHistoryFilePathModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.LoadList(model, out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
        ///// <summary>
        ///// Lấy thông tin RM_ExchangeHistoryFilePath theo EHID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ExchangeHistoryFilePathModel> GetByEHID(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ExchangeHistoryFilePath_GetByEHID_", ID);
            var data = GetCacheItem(rawKey) as List<RM_ExchangeHistoryFilePathModel>;
            if (data != null) return data;
            data = Api.GetByEHID(ID);
            AddCacheItem(rawKey, data); return data;
        }
    }
}
