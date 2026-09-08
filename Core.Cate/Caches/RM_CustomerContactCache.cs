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
    public class RM_CustomerContactCache : CacheLayer
    {
        private RM_CustomerContactBiz _RM_CustomerContactApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_CustomerContactCache", "CENIT.APP.Cache" };

        private RM_CustomerContactBiz Api => _RM_CustomerContactApi ?? (_RM_CustomerContactApi = new RM_CustomerContactBiz());

        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_CustomerContactModel model, string username)
        {
            var isDeleted = Api.Delete(model.CustomerContactID, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_CustomerContact
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_CustomerContactModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_CustomerContact
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_CustomerContactModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_CustomerContact");
            var data = GetCacheItem(rawKey) as List<RM_CustomerContactModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tinRM_CustomerContact theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_CustomerContactModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_CustomerContactByID_", ID);
            var data = GetCacheItem(rawKey) as RM_CustomerContactModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_CustomerContact
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_CustomerContactModel> Get(out int total, int customerID,  BaseSearchModel search = null)
        {
            var rawKey = string.Concat(
                "GetSearch_RM_CustomerContact_",
                customerID, "_",
                UtilEncrypt.FromObject(search)
            );
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_CustomerContactModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, customerID, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
    }
}
