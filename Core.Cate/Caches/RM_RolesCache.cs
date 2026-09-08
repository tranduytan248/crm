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
    public class RM_RolesCache : CacheLayer
    {
        private RM_RolesBiz _RM_RolesApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_RolesCache", "CENIT.APP.Cache" };

        private RM_RolesBiz Api => _RM_RolesApi ?? (_RM_RolesApi = new RM_RolesBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_RolesModel model, string deletedBy)
        {
            var isDeleted = Api.Delete(model.RoleID, deletedBy);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_Roles
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_RolesModel model, string savedBy)
        {
            var isSaved = Api.Save(model, savedBy);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_Roles
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_RolesModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_Roles");
            var data = GetCacheItem(rawKey) as List<RM_RolesModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy thông tinRM_Roles theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_RolesModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_RolesByID_", ID);
            var data = GetCacheItem(rawKey) as RM_RolesModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_Roles
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_RolesModel> Get(out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_Roles", UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_RolesModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }

    }
}
