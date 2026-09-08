using Core.Cate.Biz;
using Core.Cate.Models;
using Core.RM.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_CustomerCache : CacheLayer
    {
        private RM_CustomerBiz _RM_CustomerApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_CustomerCache", "CENIT.APP.Cache" };

        private RM_CustomerBiz Api => _RM_CustomerApi ?? (_RM_CustomerApi = new RM_CustomerBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_CustomerModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_CustomerModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Thêm mới Cơ hội kinh doanh
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int SaveCHKD(RM_BusinessOpportunityModel model, string username)
        {
            var isSaved = Api.SaveCHKD(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        ///// <summary>
        ///// Lấy danh sách Cơ hội kinh doanh theo Customer ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_BusinessOpportunityModel GetData_ByCustormerId(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_BusinessOpportunityByCustomerID_", ID);
            var data = GetCacheItem(rawKey) as RM_BusinessOpportunityModel;
            if (data != null) return data;
            data = Api.LoadData_ByCustormerId(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_Customer
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_CustomerModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_Customer");
            var data = GetCacheItem(rawKey) as List<RM_CustomerModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_CustomerModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_CustomerByID_", ID);
            var data = GetCacheItem(rawKey) as RM_CustomerModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data);
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_Customer
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_CustomerModel> Get(out int total, RM_CustomerSearchModel searchModel = null, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_Customer", UtilEncrypt.FromObject(searchModel), UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;

            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            var data = GetCacheItem(rawKey) as List<RM_CustomerModel>;
            if (data != null) return data;

            data = Api.LoadList(out total, searchModel, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }

        /// <summary>
        /// Import hàng loạt danh sách RM_Customer
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public RM_CustomerImportResultModel BulkImport(List<RM_CustomerImportRowModel> rows, string username)
        {
            var result = Api.BulkImport(rows, username);
            if (result.SuccessCount > 0) InvalidateCache();
            return result;
        }
    }
}
