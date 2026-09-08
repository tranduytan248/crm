using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using Core.Cate.Biz;
using TSFramework.Libs.Utils;
using System.Data;

namespace Core.Cate.Caches
{
    public class RM_Employee_BusinessPlanCache : CacheLayer
    {
        private RM_Employee_BusinessPlanBiz _RM_Employee_BusinessPlanApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_Employee_BusinessPlanCache", "CENIT.APP.Cache" };

        private RM_Employee_BusinessPlanBiz Api => _RM_Employee_BusinessPlanApi ?? (_RM_Employee_BusinessPlanApi = new RM_Employee_BusinessPlanBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_Employee_BusinessPlanModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_Employee_BusinessPlan
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_Employee_BusinessPlanModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_Employee_BusinessPlan
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_Employee_BusinessPlanModel> GetAll(int businessPlanID)
        {
            var rawKey = string.Concat("GetAllRM_Employee_BusinessPlan");
            var data = GetCacheItem(rawKey) as List<RM_Employee_BusinessPlanModel>;
            if (data != null) return data;
            data = Api.GetAll(businessPlanID);
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tinRM_Employee_BusinessPlan theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_Employee_BusinessPlanModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_Employee_BusinessPlanByID_", ID);
            var data = GetCacheItem(rawKey) as RM_Employee_BusinessPlanModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_Employee_BusinessPlan
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_Employee_BusinessPlanModel> Get(int businessPlanID, out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_Employee_BusinessPlan", businessPlanID, UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_Employee_BusinessPlanModel>;
            if (data != null) return data;
            data = Api.LoadList(businessPlanID, out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_Employee_BusinessPlanModel> GetData(int year, int? boPhanId, out int total)
        {
            var rawKey = string.Concat("RM_Employee_BusinessPlan_GetData", year, boPhanId);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_Employee_BusinessPlanModel>;
            if (data != null) return data;
            data = Api.LoadData(year, boPhanId, out total);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }

        /// <summary>
        /// Import thông tin RM_Employee_BusinessPlan
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Import(string key, string username)
        {
            var result = Api.SaveData(key, username);
            // Invalidate the cache
            if (result > 0) { InvalidateCache(); }
            return result;
        }

        /// <summary>
        /// Validate thông tin import
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<Import_Employee_BusinessPlanViewModel> ValidData(DataTable db, int businessPlanId, string key, string username)
        {
            return Api.ValidData(db, businessPlanId, key, username);
        }


        /// <summary>
        /// Lấy dữ liệu từ bảng tạm
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        public List<Import_Employee_BusinessPlanViewModel> GetData(string key)
        {
            var rawKey = string.Concat("ListEmployee_BusinessPlanImport-", key);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<Import_Employee_BusinessPlanViewModel> data) return data;
            // Item not found in cache - retrieve it and insert it into the cache
            data = Api.GetData(key);
            return data;
        }

        /// <summary>
        /// Xóa dữ liệ import từ bảng tạm
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Clear(string key)
        {
            var isDeleted = Api.Clear(key);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }
    }
}
