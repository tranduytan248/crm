using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using Core.Cate.Biz;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_CostTypeCache : CacheLayer
    {
        private RM_CostTypeBiz _RM_CostTypeApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_CostTypeCache", "CENIT.APP.Cache" };

        private RM_CostTypeBiz Api => _RM_CostTypeApi ?? (_RM_CostTypeApi = new RM_CostTypeBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_CostTypeModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_CostType
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_CostTypeModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_CostType
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_CostTypeModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_CostType");
            var data = GetCacheItem(rawKey) as List<RM_CostTypeModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tinRM_CostType theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_CostTypeModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_CostTypeByID_", ID);
            var data = GetCacheItem(rawKey) as RM_CostTypeModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_CostType
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_CostTypeModel> Get(out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_CostType", UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_CostTypeModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
    }
}
