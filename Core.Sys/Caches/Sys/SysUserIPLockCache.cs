using Core.Sys.Biz.Sys;
using Core.Sys.Models.Sys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Caching;

namespace Core.Sys.Caches.Sys
{
    [DataObject]
    public class SysUserIPLockCache : CacheLayer
    {
        private SysUserIPLockBiz _functionApi;

        private SysUserIPLockBiz Api => _functionApi ?? (_functionApi = new SysUserIPLockBiz());

        protected override string[] MasterCacheKeyArray => new[] { "SysUserIPLockCache", "CENIT.APP.Cache" };




        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysUserIPLockModel GetByIP(string IP)
        {
            if (string.IsNullOrEmpty(IP)) return null;

            var rawKey = string.Concat("SysUserIPLockModel-GetByIP-", IP);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is SysUserIPLockModel result) return result;
            // Item not found in cache - retrieve it and insert it into the cache
            result = Api.GetByIP(IP);
            if (result != null) AddCacheItem(rawKey, result);

            return result;
        }

        /// <summary>
        /// Lock IP
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public int LockIP(string IP)
        {
            var result = Api.Save(IP, true);
            if (result > 0)
                InvalidateCache();
            return result;
        }

        /// <summary>
        /// UnLock IP
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int UnlockIP(string IP)
        {
            var result = Api.Save(IP, false);
            if (result > 0)
                InvalidateCache();
            return result;
        }
    }
}
