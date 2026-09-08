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
    public class RM_BusinessOpportunityCache : CacheLayer
    {
        private RM_BusinessOpportunityBiz _RM_BusinessOpportunityApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_BusinessOpportunityCache", "CENIT.APP.Cache" };

        private RM_BusinessOpportunityBiz Api => _RM_BusinessOpportunityApi ?? (_RM_BusinessOpportunityApi = new RM_BusinessOpportunityBiz());

        /// <summary>
        /// Lấy toàn bộ danh sách RM_BusinessOpportunity
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_BusinessOpportunityModel> Get(out int total, RM_BusinessOpportunitySearchModel model, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_BusinessOpportunity", UtilEncrypt.FromObject(model), UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_BusinessOpportunityModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, model, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_BusinessOpportunity
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_BusinessOpportunityModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_BusinessOpportunity");
            var data = GetCacheItem(rawKey) as List<RM_BusinessOpportunityModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tin RM_BusinessOpportunity theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_BusinessOpportunityModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_BusinessOpportunityByID_", ID);
            var data = GetCacheItem(rawKey) as RM_BusinessOpportunityModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_BusinessOpportunityModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Thêm mới Cơ hội kinh doanh
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_BusinessOpportunityModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }
    }
}
