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
    public class RM_BusinessPlanFilePathCache : CacheLayer
    {
        private RM_BusinessPlanFilePathBiz _RM_BusinessPlanFilePathApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_BusinessPlanFilePathCache", "CENIT.APP.Cache" };

        private RM_BusinessPlanFilePathBiz Api => _RM_BusinessPlanFilePathApi ?? (_RM_BusinessPlanFilePathApi = new RM_BusinessPlanFilePathBiz());
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
        /// Lưu thông tin RM_BusinessPlanFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_BusinessPlanFilePathModel model, string savedBy)
        {
            var isSaved = Api.Save(model, savedBy);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_BusinessPlanFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_BusinessPlanFilePathModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_BusinessPlanFilePath");
            var data = GetCacheItem(rawKey) as List<RM_BusinessPlanFilePathModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy thông tinRM_BusinessPlanFilePath theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_BusinessPlanFilePathModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_BusinessPlanFilePathByID_", ID);
            var data = GetCacheItem(rawKey) as RM_BusinessPlanFilePathModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_BusinessPlanFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_BusinessPlanFilePathModel> Get(out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_BusinessPlanFilePath", UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_BusinessPlanFilePathModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
        ///// <summary>
        ///// Lấy thông tin RM_BusinessPlanFilePath theo BusinessPlanID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_BusinessPlanFilePathModel> GetByBusinessPlanID(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_BusinessPlanFilePath_GetByBusinessPlanID_", ID);
            var data = GetCacheItem(rawKey) as List<RM_BusinessPlanFilePathModel>;
            if (data != null) return data;
            data = Api.GetByBusinessPlanID(ID);
            AddCacheItem(rawKey, data); return data;
        }
    }
}
