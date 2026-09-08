using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Core.Cate.Models;
using Core.Cate.Biz;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_ProductServiceFilePathCache : CacheLayer
    {
        private RM_ProductServiceFilePathBiz _RM_ProductServiceFilePathApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_ProductServiceFilePathCache", "CENIT.APP.Cache" };

        private RM_ProductServiceFilePathBiz Api => _RM_ProductServiceFilePathApi ?? (_RM_ProductServiceFilePathApi = new RM_ProductServiceFilePathBiz());
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
        /// Lấy toàn bộ danh sách RM_ProductServiceFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProductServiceFilePathModel> GetAll(int GroupFilePathID)
        {
            var rawKey = string.Concat("GetAll_RM_ProductServiceFilePathModel_ByGroupFilePathID_", GroupFilePathID);
            var data = GetCacheItem(rawKey) as List<RM_ProductServiceFilePathModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.GetAll(GroupFilePathID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy thông tinRM_ProductServiceFilePath theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_ProductServiceFilePathModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ProductServiceFilePathByID_", ID);
            var data = GetCacheItem(rawKey) as RM_ProductServiceFilePathModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ProductServiceFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProductServiceFilePathModel> Get(out int total,RM_ProductServiceGroupFilePathModel model, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_ProductServiceFilePathModel", UtilEncrypt.FromObject(model), UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_ProductServiceFilePathModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.LoadList(out total, model, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
    }
}
