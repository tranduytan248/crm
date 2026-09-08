using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using Core.Cate.Biz;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_ProductProjectCache : CacheLayer
    {
        private RM_ProductProjectBiz _RM_ProductProjectApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_ProductProjectCache", "CENIT.APP.Cache" };

        private RM_ProductProjectBiz Api => _RM_ProductProjectApi ?? (_RM_ProductProjectApi = new RM_ProductProjectBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_ProductProjectModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_ProductProject
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_ProductProjectModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lưu thông tin RM_ProductProject
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Convert(RM_ProductProjectModel model, int businessOpportunityID, string username)
        {
            var isSaved = Api.Convert(model, businessOpportunityID, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ProductProject
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProductProjectModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_ProductProject");
            var data = GetCacheItem(rawKey) as List<RM_ProductProjectModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tin RM_ProductProject theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_ProductProjectModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ProductProjectByID_", ID);
            var data = GetCacheItem(rawKey) as RM_ProductProjectModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tin RM_ProductProject theo ProjectID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProductProjectModel> GetByProjectID(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ProductProjectByProjectID_", ID);
            var data = GetCacheItem(rawKey) as List<RM_ProductProjectModel>;
            if (data != null) return data;
            data = Api.GetByProjectID(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ProductProject
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProductProjectModel> Get(out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_ProductProject", UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_ProductProjectModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
    }
}
