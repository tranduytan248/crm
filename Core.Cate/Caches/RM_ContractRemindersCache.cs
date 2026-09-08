using Core.Cate.Biz;
using Core.Cate.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_ContractRemindersCache : CacheLayer
    {
        private RM_ContractRemindersBiz _RM_ContractRemindersApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_ContractRemindersCache", "CENIT.APP.Cache" };
        private RM_ContractRemindersBiz Api => _RM_ContractRemindersApi ?? (_RM_ContractRemindersApi = new RM_ContractRemindersBiz());


        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_ContractRemindersModel model, string username)
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
        public int Save(RM_ContractRemindersModel model, DataTable dt, string username)
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
        public List<RM_ContractRemindersModel> GetAll(int ContractID)
        {
            var rawKey = string.Concat("GetAllRM_ContractReminders_", ContractID);
            var data = GetCacheItem(rawKey) as List<RM_ContractRemindersModel>;
            if (data != null) return data;
            data = Api.GetAll(ContractID);
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tinRM_ContactPersons theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_ContractRemindersModel GetById(string ID)
        {
            if (string.IsNullOrEmpty(ID)) return null;
            var rawKey = string.Concat("GetRM_ContractRemindersByID_", ID);
            var data = GetCacheItem(rawKey) as RM_ContractRemindersModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ContactPersons
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ContractRemindersModel> Get(out int total, int ContractID, BaseSearchModel search = null)
        {
            var rawKey = string.Concat(
                "GetSearch_RM_ContractReminders_",
                UtilEncrypt.FromObject(search),
                "_",
                ContractID
            );
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_ContractRemindersModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, ContractID, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
    }
}
