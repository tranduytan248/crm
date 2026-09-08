using Core.Sys.Biz.Sys;
using Core.Sys.Models.Sys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Sys.Caches.Sys
{
    public class SysNotificationsCache : CacheLayer
    {
        private SysNotificationsBiz _Sys_NotificationsApi;
        protected override string[] MasterCacheKeyArray => new[] { "SysNotificationsCache", "CENIT.APP.Cache" };
        private SysNotificationsBiz Api => _Sys_NotificationsApi ?? (_Sys_NotificationsApi = new SysNotificationsBiz());

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysNotificationsModel> GetByUser(string username, out int total, BaseSearchModel search = null)
        {
            var objectKey = UtilEncrypt.FromObject(search);

            var rawKey = string.Concat("ListThongBaos-", objectKey, username);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var datas = GetCacheItem(rawKey) as List<SysNotificationsModel>;
            if (datas != null) return datas;
            // Item not found in cache - retrieve it and insert it into the cache
            datas = Api.GetByUser(username, out total, search);
            if (datas == null) return null;
            AddCacheItem(rawKey, datas);
            AddCacheItem(rawKeyTotal, total);

            return datas;
        }

        /// <summary>
        /// Đánh dấu đã đọc thông báo
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int MarkAsRead(int id, string username)
        {
            var isDeleted = Api.MarkAsRead(id, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }
    }
}
