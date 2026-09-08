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
    public class RM_BusinessOpportunityFilePathCache : CacheLayer
    {
        private RM_BusinessOpportunityFilePathBiz _RM_BusinessOpportunityFilePathApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_BusinessOpportunityFilePathCache", "CENIT.APP.Cache" };

        private RM_BusinessOpportunityFilePathBiz Api => _RM_BusinessOpportunityFilePathApi ?? (_RM_BusinessOpportunityFilePathApi = new RM_BusinessOpportunityFilePathBiz());
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
        /// Lưu thông tin RM_BusinessOpportunityFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_BusinessOpportunityFilePathModel model, string savedBy)
        {
            var isSaved = Api.Save(model, savedBy);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_BusinessOpportunityFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_BusinessOpportunityFilePathModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_BusinessOpportunityFilePath");
            var data = GetCacheItem(rawKey) as List<RM_BusinessOpportunityFilePathModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy thông tinRM_BusinessOpportunityFilePath theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_BusinessOpportunityFilePathModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_BusinessOpportunityFilePathByID_", ID);
            var data = GetCacheItem(rawKey) as RM_BusinessOpportunityFilePathModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_BusinessOpportunityFilePath
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_BusinessOpportunityFilePathModel> Get(RM_BusinessOpportunityFilePathSearchModel model, out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_BusinessOpportunityFilePath", UtilEncrypt.FromObject(model), UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_BusinessOpportunityFilePathModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.LoadList(model, out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
        ///// <summary>
        ///// Lấy thông tin RM_BusinessOpportunityFilePath theo BOID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_BusinessOpportunityFilePathModel> GetByBOID(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_BusinessOpportunityFilePath_GetByBOID_", ID);
            var data = GetCacheItem(rawKey) as List<RM_BusinessOpportunityFilePathModel>;
            if (data != null) return data;
            data = Api.GetByBOID(ID);
            AddCacheItem(rawKey, data); return data;
        }
    }
}
