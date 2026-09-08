using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using Core.Cate.Biz;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class Cate_CustomerCache : CacheLayer
    {
        private Cate_CustomerBiz _Cate_CustomerApi;
        protected override string[] MasterCacheKeyArray => new[] { "Cate_CustomerCache", "CENIT.APP.Cache" };

        private Cate_CustomerBiz Api => _Cate_CustomerApi ?? (_Cate_CustomerApi = new Cate_CustomerBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(Cate_CustomerModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin Cate_Customer
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(Cate_CustomerModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách Cate_Customer
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<Cate_CustomerModel> GetAll()
        {
            var rawKey = string.Concat("GetAllCate_Customer");
            var data = GetCacheItem(rawKey) as List<Cate_CustomerModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tinCate_Customer theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public KWC_AccountMobileModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetCate_CustomerByID_", ID);
            var data = GetCacheItem(rawKey) as KWC_AccountMobileModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy thông tinCate_Customer theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public Cate_CustomerModel GetByUsername(string Username)
        {
            if (string.IsNullOrEmpty(Username)) return null;
            var rawKey = string.Concat("GetByUsername", Username);
            var data = GetCacheItem(rawKey) as Cate_CustomerModel;
            if (data != null) return data;
            data = Api.LoadDetail(Username);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách Cate_Customer
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<Cate_CustomerModel> Get(out int total, Cate_CustomerSearchModel model, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_Cate_Customer", UtilEncrypt.FromObject(search), UtilEncrypt.FromObject(model));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<Cate_CustomerModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, model, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
    }
}
