using Core.Cate.Biz;
using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_InvoicesCache : CacheLayer
    {
        private RM_InvoicesBiz _RM_InvoicesApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_InvoicesCache", "CENIT.APP.Cache" };
        private RM_InvoicesBiz Api => _RM_InvoicesApi ?? (_RM_InvoicesApi = new RM_InvoicesBiz());


        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_InvoicesModel model, string username)
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
        public int Save(RM_InvoicesModel model, DataTable dt, string username)
        {
            var isSaved = Api.Save(model, dt, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ContactPersons
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_InvoicesModel> GetAll(int ContractID)
        {
            var rawKey = string.Concat("GetAllRM_Invoices_", ContractID);
            var data = GetCacheItem(rawKey) as List<RM_InvoicesModel>;
            if (data != null) return data;
            data = Api.GetAll(ContractID);
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tinRM_ContactPersons theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_InvoicesModel GetById(string ID)
        {
            if (string.IsNullOrEmpty(ID)) return null;
            var rawKey = string.Concat("GetRM_InvoicesByID_", ID);
            var data = GetCacheItem(rawKey) as RM_InvoicesModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ContactPersons
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_InvoicesModel> Get(out int total, int ContractID, BaseSearchModel search = null)
        {
            var rawKey = string.Concat(
                "GetSearch_RM_Invoices_",
                UtilEncrypt.FromObject(search),
                "_",
                ContractID
            );
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_InvoicesModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, ContractID, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
        #region FilePath
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int DeleteFilePath(RM_InvoicesFilePathModel model, string username)
        {
            var isDeleted = Api.DeleteFilePath(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_InvoicesFilePathModel> GetFilePaths(string invoice_id)
        {
            var rawKey = string.Concat("GetFilePaths_by_invoice_id", invoice_id);
            var data = GetCacheItem(rawKey) as List<RM_InvoicesFilePathModel>;
            if (data != null) return data;
            data = Api.GetFilePaths(invoice_id);
            AddCacheItem(rawKey, data); return data;
        }
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_InvoicesFilePathModel GetFilePathByID(string ID)
        {
            if (string.IsNullOrEmpty(ID)) return null;
            var rawKey = string.Concat("GetRM_InvoicesFilePathBy_ID_", ID);
            var data = GetCacheItem(rawKey) as RM_InvoicesFilePathModel;
            if (data != null) return data;
            data = Api.GetFilePathByID(ID);
            AddCacheItem(rawKey, data); return data;
        }
        #endregion
    }
}
