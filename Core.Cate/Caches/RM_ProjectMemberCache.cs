using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Core.Cate.Models;
using Core.Cate.Biz;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_ProjectMemberCache : CacheLayer
    {
        private RM_ProjectMemberBiz _RM_ProjectMemberApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_ProjectMemberCache", "CENIT.APP.Cache" };

        private RM_ProjectMemberBiz Api => _RM_ProjectMemberApi ?? (_RM_ProjectMemberApi = new RM_ProjectMemberBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_ProjectMemberModel model, string deletedBy)
        {
            var isDeleted = Api.Delete(model.ProjectMemberID, deletedBy);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_ProjectMember
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_ProjectMemberModel model, string savedBy)
        {
            var isSaved = Api.Save(model, savedBy);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lưu thông tin RM_ProjectMember
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int SaveMulti(RM_ProjectMemberFormModel model, string savedBy)
        {
            var isSaved = Api.SaveMulti(model, savedBy);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ProjectMember
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProjectMemberModel> GetByProductProjectID(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("Get_RM_ProjectMember_GetByProductProjectID_", ID);
            var data = GetCacheItem(rawKey) as List<RM_ProjectMemberModel>;
            if (data != null) return data;
            data = Api.GetByProductProjectID(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy thông tinRM_ProjectMember theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_ProjectMemberModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ProjectMemberByID_", ID);
            var data = GetCacheItem(rawKey) as RM_ProjectMemberModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ProjectMember
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        // [DataObjectMethod(DataObjectMethodType.Select, true)]
        // public List<RM_ProjectMemberModel> Get(out int total, int ProjectID, BaseSearchModel search = null)
        // {
        //     var rawKey = string.Concat("GetSearch_RM_ProjectMember", UtilEncrypt.FromObject(search), "_project_", ProjectID);
        //     var rawKeyTotal = string.Concat(rawKey, "-Total");
        //     total = 0;
        //     var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
        //     var data = GetCacheItem(rawKey) as List<RM_ProjectMemberModel>;
        //     if (data != null && data.Count() > 0) return data;
        //     data = Api.LoadList(out total, ProjectID, search);
        //     AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        // }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ProjectMemberModel> GetAll(int ProjectID)
        {
            var rawKey = string.Concat("GetAll_RM_ProjectMemberByProjectID_", ProjectID);
            var data = GetCacheItem(rawKey) as List<RM_ProjectMemberModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.GetAll(ProjectID);
            AddCacheItem(rawKey, data);
            return data;
        }
    }
}
