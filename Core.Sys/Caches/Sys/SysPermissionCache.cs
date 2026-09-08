using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Core.Sys.Biz.Sys;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Caching;

namespace Core.Sys.Caches.Sys
{
    [DataObject]
    public class SysPermissionCache : CacheLayer
    {
        private SysPermissionBiz _permissionApi;

        private SysPermissionBiz Api => _permissionApi ?? (_permissionApi = new SysPermissionBiz());

        protected override string[] MasterCacheKeyArray =>
            new[] { "SysPermissionsCache", "SysFunctionsCache", "SysRolesCache", "SysModuleCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysPermissionModel> GetByRoleId(int groupId)
        {
            if (groupId < 0) return null;

            var rawKey = string.Concat("PermissionByRoleID-", groupId);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysPermissionModel> permissions) return permissions;
            // Item not found in cache - retrieve it and insert it into the cache
            permissions = Api.GetByRoleId(groupId);
            AddCacheItem(rawKey, permissions);

            return permissions;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public bool Save(int groupId, DataTable permission, string separated)
        {
            var success = Api.Save(groupId, permission, separated);
            if (success)
                // Invalidate the cache
                InvalidateCache();
            return success;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public bool IsAllow(string userName, string areaName, string controllerName, string actionName)
        {
            // See if the item is in the cache

            // Item not found in cache - retrieve it and insert it into the cache
            return Api.IsAllow(userName, areaName, controllerName, actionName);
        }
    }
}