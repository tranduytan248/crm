using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using Core.Cate.Biz;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    /// <summary>
    /// Cache dữ liệu dự án và các tập dữ liệu xuất báo cáo liên quan đến dự án.
    /// </summary>
    public class RM_ProjectCache : CacheLayer
    {
        private RM_ProjectBiz _RM_ProjectApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_ProjectCache", "CENIT.APP.Cache" };
        private RM_ProjectBiz Api => _RM_ProjectApi ?? (_RM_ProjectApi = new RM_ProjectBiz());

        /// <summary>
        /// Xóa thông tin dự án.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_ProjectModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin dự án.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_ProjectModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Chuyển cơ hội kinh doanh thành dự án.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int ConvertProject(RM_ProjectModel model, string username)
        {
            var isSaved = Api.ConvertProject(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách dự án.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProjectModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_Project");
            var data = GetCacheItem(rawKey) as List<RM_ProjectModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy chi tiết dự án theo mã dữ liệu.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_ProjectModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ProjectByID_", ID);
            var data = GetCacheItem(rawKey) as RM_ProjectModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy danh sách dự án theo điều kiện tìm kiếm và phân trang.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProjectModel> Get(out int total, RM_ProjectSearchModel model, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_Project", UtilEncrypt.FromObject(model), UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_ProjectModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, model, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }
        /// <summary>
        /// Lấy dữ liệu xuất danh sách dự án.
        /// </summary>
        public List<ProjectExportRow> ExportProjects(RM_ProjectSearchModel model)
            => Api.ExportProjects(model);

        /// <summary>
        /// Lấy dữ liệu xuất danh sách sản phẩm dịch vụ của dự án.
        /// </summary>
        public List<ProductExportRow> ExportProducts(RM_ProjectSearchModel model)
            => Api.ExportProducts(model);

        /// <summary>
        /// Lấy dữ liệu xuất danh sách thành viên dự án.
        /// </summary>
        public List<MemberExportRow> ExportMembers(RM_ProjectSearchModel model)
            => Api.ExportMembers(model);

        /// <summary>
        /// Lấy dữ liệu xuất danh sách công việc dự án.
        /// </summary>
        public List<TaskExportRow> ExportTasks(RM_ProjectSearchModel model)
            => Api.ExportTasks(model);
    }
}
