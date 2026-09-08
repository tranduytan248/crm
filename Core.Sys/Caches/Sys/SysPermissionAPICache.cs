using System.Collections.Generic;
using System.ComponentModel;
using Core.Sys.Biz.Sys;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Sys.Caches.Sys
{
    [DataObject]
    public class SysPermissionAPICache : CacheLayer
    {
        private SysPermissionAPIBiz _permissionApi;

        private SysPermissionAPIBiz Api => _permissionApi ?? (_permissionApi = new SysPermissionAPIBiz());

        protected override string[] MasterCacheKeyArray =>
            new[] { "SysPermissionAPICache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysPermissionAPIModel GetById(int userID)
        {
            if (userID < 0) return null;

            var rawKey = string.Concat("SysPhanQuyenAPIGetByUserIDByID-", userID);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is SysPermissionAPIModel config) return config;
            // Item not found in cache - retrieve it and insert it into the cache
            config = Api.GetById(userID);
            if (config != null) AddCacheItem(rawKey, config);

            return config;
        }


        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(SysPermissionAPIModel model)
        {
            var permissionAPIId = Api.Save(model);
            if (permissionAPIId > 0)
                // Invalidate the cache
                InvalidateCache();
            return permissionAPIId;
        }
      
    }
}