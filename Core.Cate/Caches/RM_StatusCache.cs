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
    public class RM_StatusCache : CacheLayer
    {
        private RM_StatusBiz _RM_StatusApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_StatusCache", "CENIT.APP.Cache" };
        private RM_StatusBiz Api => _RM_StatusApi ?? (_RM_StatusApi = new RM_StatusBiz());


        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_StatusModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_ContactPersons
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_StatusModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ContactPersons
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_StatusModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_Status");
            var data = GetCacheItem(rawKey) as List<RM_StatusModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tinRM_ContactPersons theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_StatusModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_StatusByID_", ID);
            var data = GetCacheItem(rawKey) as RM_StatusModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ContactPersons
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_StatusModel> Get(out int total, string Searchkey, BaseSearchModel search = null)
        {
            var rawKey = string.Concat(
                "GetSearch_RM_Status_",
                UtilEncrypt.FromObject(search),
                "_",
                Searchkey
            );
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_StatusModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, search, Searchkey);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }


        ///// <summary>
        ///// Lấy thông tinRM_ContactPersons theo CustomerID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        public List<RM_StatusModel> GetStatusBySearchKey(string Searchkey)
        {
            int total;

            var rawKey = $"GetRM_StatusBySearchKey_{Searchkey}";
            var data = GetCacheItem(rawKey) as List<RM_StatusModel>;

            if (data != null)
                return data;

            data = Api.LoadList(out total, null , Searchkey) ?? new List<RM_StatusModel>();

            AddCacheItem(rawKey, data);

            return data;
        }
    }
}
