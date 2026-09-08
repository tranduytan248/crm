using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using Core.Cate.Biz;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_ProductServiceGroupFilePathCache : CacheLayer
    {
        private RM_ProductServiceGroupFilePathBiz _RM_ProductServiceGroupFilePathApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_ProductServiceGroupFilePathCache", "CENIT.APP.Cache" };

        private RM_ProductServiceGroupFilePathBiz Api => _RM_ProductServiceGroupFilePathApi ?? (_RM_ProductServiceGroupFilePathApi = new RM_ProductServiceGroupFilePathBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_ProductServiceGroupFilePathModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_ProductServiceGroupFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_ProductServiceGroupFilePathModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ProductServiceGroupFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProductServiceGroupFilePathModel> GetAll(int ProductServiceID)
        {
            var rawKey = string.Concat("GetAll_RM_ProductServiceGroupFilePathModel_By_ProductServiceID", ProductServiceID);
            var data = GetCacheItem(rawKey) as List<RM_ProductServiceGroupFilePathModel>;
            if (data != null) return data;
            data = Api.GetAll(ProductServiceID);
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tinRM_ProductServiceGroupFilePath theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_ProductServiceGroupFilePathModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ProductServiceGroupFilePathByID_", ID);
            var data = GetCacheItem(rawKey) as RM_ProductServiceGroupFilePathModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

    
        /// <summary>
        /// Lấy toàn bộ danh sách RM_ProductServiceGroupFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProductServiceGroupFilePathModel> Get(out int total, Cate_ProductServiceModel model, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_ProductServiceGroupFilePath", UtilEncrypt.FromObject(model), UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_ProductServiceGroupFilePathModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, model, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
    }
}
