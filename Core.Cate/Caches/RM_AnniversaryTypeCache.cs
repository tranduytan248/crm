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
    public class RM_AnniversaryTypeCache : CacheLayer
    {
        private RM_AnniversaryTypeBiz _RM_AnniversaryTypeApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_AnniversaryTypeCache", "CENIT.APP.Cache" };

        private RM_AnniversaryTypeBiz Api => _RM_AnniversaryTypeApi ?? (_RM_AnniversaryTypeApi = new RM_AnniversaryTypeBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_AnniversaryTypeModel model, string deletedBy)
        {
            var isDeleted = Api.Delete(model.AnniversaryType_ID, deletedBy);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_AnniversaryType
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_AnniversaryTypeModel model, string savedBy)
        {
            var isSaved = Api.Save(model, savedBy);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_AnniversaryType
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_AnniversaryTypeModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_AnniversaryType");
            var data = GetCacheItem(rawKey) as List<RM_AnniversaryTypeModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy thông tinRM_AnniversaryType theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_AnniversaryTypeModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_AnniversaryTypeByID_", ID);
            var data = GetCacheItem(rawKey) as RM_AnniversaryTypeModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_AnniversaryType
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_AnniversaryTypeModel> Get(out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_AnniversaryType", UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_AnniversaryTypeModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }

    }
}
