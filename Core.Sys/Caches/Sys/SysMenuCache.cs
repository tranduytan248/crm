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
    public class SysMenuCache : CacheLayer
    {
        private SysMenuBiz _menuApi;

        private SysMenuBiz Api => _menuApi ?? (_menuApi = new SysMenuBiz());

        protected override string[] MasterCacheKeyArray =>
            new[]
            {
                "SysMenusCache", "SysFunctionsCache", "SysModuleCache", "SysPermissionsCache", "SysRolesCache",
                "CENIT.APP.Cache"
            };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysMenuModel> GetAll()
        {
            const string rawKey = "AllMenus";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysMenuModel> menus) return menus;
            // Item not found in cache - retrieve it and insert it into the cache
            menus = Api.GetAll();
            AddCacheItem(rawKey, menus);

            return menus;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysMenuModel> Get(out int total, BaseSearchModel search = null)
        {
            var objectKey = UtilEncrypt.FromObject(search);
            var rawKey = string.Concat("ListMenus-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysMenuModel> menus) return menus;
            // Item not found in cache - retrieve it and insert it into the cache
            menus = Api.GetList(out total, search);
            AddCacheItem(rawKey, menus);
            AddCacheItem(rawKeyTotal, total);

            return menus;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysMenuModel> GetByUserId(int userId)
        {
            if (userId < 0) return null;

            var rawKey = string.Concat("MenusByUserID-", userId);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysMenuModel> menus) return menus;
            // Item not found in cache - retrieve it and insert it into the cache
            menus = Api.GetByUserId(userId);
            if (menus != null) AddCacheItem(rawKey, menus);

            return menus;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysMenuModel> GetByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var rawKey = string.Concat("MenusByUserName-", userName);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysMenuModel> menus) return menus;
            // Item not found in cache - retrieve it and insert it into the cache
            menus = Api.GetByUserName(userName);
            if (menus != null) AddCacheItem(rawKey, menus);

            return menus;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysMenuModel> GetByUserAndArea(string userName, string areaName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var rawKey = $"MenusByUserName-{userName}-Area-{areaName}";

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysMenuModel> menus) return menus;
            // Item not found in cache - retrieve it and insert it into the cache
            menus = Api.GetByUserAndArea(userName, areaName);
            if (menus != null) AddCacheItem(rawKey, menus);

            return menus;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysMenuModel> GetMenuChilds(int menuId)
        {
            if (menuId <= 0) return null;

            var rawKey = string.Concat("MenuChildssByID-", menuId);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysMenuModel> menus) return menus;
            // Item not found in cache - retrieve it and insert it into the cache
            menus = Api.GetMenuChilds(menuId);
            if (menus != null) AddCacheItem(rawKey, menus);

            return menus;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysMenuModel GetById(int menuId)
        {
            if (menuId < 0) return null;

            var rawKey = string.Concat("MenuByID-", menuId);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is SysMenuModel menu) return menu;
            // Item not found in cache - retrieve it and insert it into the cache
            menu = Api.GetById(menuId);
            if (menu != null) AddCacheItem(rawKey, menu);

            return menu;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(SysMenuModel model)
        {
            var menuId = Api.Save(model);
            if (menuId > 0)
                // Invalidate the cache
                InvalidateCache();
            return menuId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysMenuModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}