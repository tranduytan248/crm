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
    public class RM_ContactPersonsCache : CacheLayer
    {
        private RM_ContactPersonsBiz _RM_ContactPersonsApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_ContactPersonsCache", "CENIT.APP.Cache" };

        private RM_ContactPersonsBiz Api => _RM_ContactPersonsApi ?? (_RM_ContactPersonsApi = new RM_ContactPersonsBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_ContactPersonsModel model, string username)
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
        public int Save(RM_ContactPersonsModel model, string username)
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
        public List<RM_ContactPersonsModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_ContactPersons");
            var data = GetCacheItem(rawKey) as List<RM_ContactPersonsModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tinRM_ContactPersons theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_ContactPersonsModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ContactPersonsByID_", ID);
            var data = GetCacheItem(rawKey) as RM_ContactPersonsModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ContactPersons
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ContactPersonsModel> Get(out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_ContactPersons", UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_ContactPersonsModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }

        ///// <summary>
        ///// Lấy thông tin RM_ContactPersonsModel theo CustomerID(
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ContactPersonsModel> GetByCustomerID(int? ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ContactPersonsByCustomerID_", ID);
            var data = GetCacheItem(rawKey) as List<RM_ContactPersonsModel>;
            if (data != null) return data;
            data = Api.GetByCustomerID(ID);
            AddCacheItem(rawKey, data); return data;
        }

        public RM_ContactPersonsImportResultModel BulkImport(List<RM_ContactPersonsImportRowModel> rows, string username)
        {
            var result = Api.BulkImport(rows, username);
            if (result.SuccessCount > 0) InvalidateCache();
            return result;
        }
    }
}
