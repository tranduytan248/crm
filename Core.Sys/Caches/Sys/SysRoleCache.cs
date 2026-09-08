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
    public class SysRoleCache : CacheLayer
    {
        private SysRoleBiz _roleApi;

        private SysRoleBiz Api => _roleApi ?? (_roleApi = new SysRoleBiz());

        protected override string[] MasterCacheKeyArray =>
            new[] { "SysRolesCache", "SysPermissionsCache", "SysMenusCache", "SysModuleCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysRoleModel> GetAll()
        {
            const string rawKey = "AllRoles";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysRoleModel> roles) return roles;
            // Item not found in cache - retrieve it and insert it into the cache
            roles = Api.GetAll();
            AddCacheItem(rawKey, roles);

            return roles;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysRoleModel> Get(out int total, BaseSearchModel search = null)
        {
            var objectKey = UtilEncrypt.FromObject(search);
            var rawKey = string.Concat("ListRoles-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysRoleModel> roles) return roles;
            // Item not found in cache - retrieve it and insert it into the cache
            roles = Api.GetList(out total, search);
            AddCacheItem(rawKey, roles);
            AddCacheItem(rawKeyTotal, total);

            return roles;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysRoleModel GetById(int roleId)
        {
            if (roleId < 0) return null;

            var rawKey = string.Concat("RoleByID-", roleId);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is SysRoleModel role) return role;
            // Item not found in cache - retrieve it and insert it into the cache
            role = Api.GetById(roleId);
            if (role != null) AddCacheItem(rawKey, role);

            return role;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(SysRoleModel model)
        {
            var roleId = Api.Save(model);
            if (roleId > 0)
                // Invalidate the cache
                InvalidateCache();
            return roleId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysRoleModel model)
        {
            var isSuccess = Api.Delete(model);
            if (isSuccess)
                // Invalidate the cache
                InvalidateCache();
            return isSuccess;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? AddUsers(SysRoleModel model)
        {
            var roleId = Api.AddUsers(model);
            if (roleId > 0)
                // Invalidate the cache
                InvalidateCache();
            return roleId;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? AddUser(SysRoleModel model)
        {
            var roleId = Api.AddUser(model);
            if (roleId > 0)
                // Invalidate the cache
                InvalidateCache();
            return roleId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool RemoveUser(int roleId, int userId)
        {
            var isSuccess = Api.RemoveUser(roleId, userId);
            if (isSuccess)
                // Invalidate the cache
                InvalidateCache();
            return isSuccess;
        }
    }
}