using Core.Cate.Biz;
using Core.Cate.Models;
using System.Collections.Generic;
using System.ComponentModel;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    /// <summary>
    /// Cache dữ liệu nhật ký công việc và file đính kèm của nhật ký công việc.
    /// </summary>
    public class RM_LogTaskCache : CacheLayer
    {
        private RM_LogTaskBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_LogTaskCache", "CENIT.APP.Cache" };
        private RM_LogTaskBiz Api => _api ?? (_api = new RM_LogTaskBiz());

        /// <summary>
        /// Lấy danh sách nhật ký công việc theo điều kiện tìm kiếm và phân trang.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_LogTaskModel> Get(out int total, BaseSearchModel search = null, int? taskManagementID = null)
        {
            var key = $"GetSearch_RM_LogTask{UtilEncrypt.FromObject(search)}_{taskManagementID}";
            var keyTotal = $"{key}-Total";
            total = (int?)GetCacheItem(keyTotal) ?? 0;
            var data = GetCacheItem(key) as List<RM_LogTaskModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, search, taskManagementID);
            AddCacheItem(key, data); AddCacheItem(keyTotal, total);
            return data;
        }

        // Hàm tiện ích gọi nhanh không cần phân trang
        public List<RM_LogTaskModel> GetByTaskManagementID(int taskManagementID)
        {
            int total;
            return Get(out total, new BaseSearchModel
            {
                PageSize = -1,
                OrderDir = "DESC",
                Order = "0"
            }, taskManagementID);
        }

        /// <summary>
        /// Lấy danh sách nhật ký theo công việc dự án.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_LogTaskModel> GetByProjectTaskID(int projectTaskID)
        {
            var key = $"GetRM_LogTaskByProjectTaskID_{projectTaskID}";
            var data = GetCacheItem(key) as List<RM_LogTaskModel>;
            if (data != null) return data;
            data = Api.GetByProjectTaskID(projectTaskID);
            AddCacheItem(key, data);
            return data;
        }

        /// <summary>
        /// Lấy chi tiết nhật ký công việc theo mã dữ liệu.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_LogTaskModel GetById(int id)
        {
            var key = $"GetRM_LogTaskByID_{id}";
            var data = GetCacheItem(key) as RM_LogTaskModel;
            if (data != null) return data;
            data = Api.LoadDetail(id);
            AddCacheItem(key, data);
            return data;
        }

        /// <summary>
        /// Xóa thông tin nhật ký công việc.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_LogTaskModel model, string username)
        {
            var result = Api.Delete(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        /// <summary>
        /// Lưu thông tin nhật ký công việc.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_LogTaskModel model, string username)
        {
            var result = Api.Save(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        // ── FilePath ──────────────────────────────────────────────────────────

        /// <summary>
        /// Lấy danh sách file đính kèm của nhật ký công việc theo điều kiện tìm kiếm.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_LogTaskFilePathModel> GetFilePaths(RM_LogTaskFilePathSearchModel model, out int total, BaseSearchModel search = null)
        {
            var key = $"GetRM_LogTaskFilePath{UtilEncrypt.FromObject(model)}{UtilEncrypt.FromObject(search)}";
            var keyTotal = $"{key}-Total";
            total = (int?)GetCacheItem(keyTotal) ?? 0;
            var data = GetCacheItem(key) as List<RM_LogTaskFilePathModel>;
            if (data != null) return data;
            data = Api.GetFilePaths(model, out total, search);
            AddCacheItem(key, data); AddCacheItem(keyTotal, total);
            return data;
        }

        /// <summary>
        /// Lấy chi tiết file đính kèm của nhật ký công việc theo mã dữ liệu.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_LogTaskFilePathModel GetFilePathById(int id)
        {
            var key = $"GetRM_LogTaskFilePathByID_{id}";
            var data = GetCacheItem(key) as RM_LogTaskFilePathModel;
            if (data != null) return data;
            data = Api.GetFilePathById(id);
            AddCacheItem(key, data);
            return data;
        }

        /// <summary>
        /// Lưu đường dẫn file đính kèm của nhật ký công việc.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int SaveFilePath(RM_LogTaskFilePathModel model, string username)
        {
            var result = Api.SaveFilePath(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        /// <summary>
        /// Xóa thông tin file đính kèm của nhật ký công việc.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int DeleteFilePath(RM_LogTaskFilePathModel model, string username)
        {
            var result = Api.DeleteFilePath(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }
    }
}
