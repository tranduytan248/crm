using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Utils;
namespace Core.Cate.Caches
{
    public class RM_TaskManagementCache : CacheLayer
    {
        private RM_TaskManagementBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_TaskManagementCache", "CENIT.APP.Cache" };
        private RM_TaskManagementBiz Api => _api ?? (_api = new RM_TaskManagementBiz());

        /// <summary>
        /// Lấy chi tiết quản lý công việc theo mã dữ liệu và cache theo từng bản ghi.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_TaskManagementModel GetById(int taskManagementID)
        {
            var key = $"GetRM_TaskManagementByID_{taskManagementID}";
            var data = GetCacheItem(key) as RM_TaskManagementModel;
            if (data != null) return data;
            data = Api.GetById(taskManagementID);
            AddCacheItem(key, data);
            return data;
        }

        /// <summary>
        /// lấy danh sách công việc theo ProductProjectID
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_TaskManagementModel> GetByProductProjectID(int productProjectID)
        {
            var key = $"GetRM_TaskManagementByProductProjectID_{productProjectID}";
            var data = GetCacheItem(key) as List<RM_TaskManagementModel>;
            if (data != null) return data;
            data = Api.GetByProductProjectID(productProjectID);
            AddCacheItem(key, data);
            return data;
        }

        /// <summary>
        /// Xóa mềm quản lý công việc và làm mới cache liên quan.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_TaskManagementModel model, string username)
        {
            var result = Api.Delete(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        /// <summary>
        /// Lưu hoặc cập nhật thông tin quản lý công việc và làm mới cache liên quan.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_TaskManagementModel model, string username)
        {
            var result = Api.Save(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_TaskManagementModel> Get(out int total, RM_TaskManagementSearchModel searchModel = null, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetRM_TaskManagement", UtilEncrypt.FromObject(searchModel), UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;

            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            var data = GetCacheItem(rawKey) as List<RM_TaskManagementModel>;
            if (data != null) return data;

            data = Api.LoadList(out total, searchModel, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);

            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_TaskManagementModel> GetAll(int ProjectID)
        {
            var rawKey = string.Concat("GetAll_TaskManagement_BByProjectID_", ProjectID);

            var data = GetCacheItem(rawKey) as List<RM_TaskManagementModel>;
            if (data != null) return data;

            data = Api.GetAll(ProjectID);
            AddCacheItem(rawKey, data);
            return data;
        }
    }
}
