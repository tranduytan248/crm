using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using Core.Cate.Models;
using Core.Cate.Biz;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_SalesTeamMembersCache : CacheLayer
    {
        private RM_SalesTeamMembersBiz _RM_SalesTeamMembersApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_SalesTeamMembersCache", "CENIT.APP.Cache" };

        private RM_SalesTeamMembersBiz Api => _RM_SalesTeamMembersApi ?? (_RM_SalesTeamMembersApi = new RM_SalesTeamMembersBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_SalesTeamMembersModel model, string deletedBy)
        {
            var isDeleted = Api.Delete(model.MemberID, deletedBy);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin RM_SalesTeamMembers
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_SalesTeamMembersModel model, string savedBy)
        {
            var isSaved = Api.Save(model, savedBy);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lưu thông tin RM_SalesTeamMembers
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int SaveMulti(RM_SalesTeamMembersFormModel model, string savedBy)
        {
            var isSaved = Api.SaveMulti(model, savedBy);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_SalesTeamMembers
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_SalesTeamMembersModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_SalesTeamMembers");
            var data = GetCacheItem(rawKey) as List<RM_SalesTeamMembersModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_SalesTeamMembers
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_SalesTeamMembersModel> GetByBusinessOpportunityID(int businessOpportunityID, out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_SalesTeamMembersByBusinessOpportunityID", UtilEncrypt.FromObject(search), businessOpportunityID);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_SalesTeamMembersModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.GetByBusinessOpportunityID(businessOpportunityID, out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }

        /// <summary>
        /// Lấy thông tinRM_SalesTeamMembers theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_SalesTeamMembersModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_SalesTeamMembersByID_", ID);
            var data = GetCacheItem(rawKey) as RM_SalesTeamMembersModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_SalesTeamMembers
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_SalesTeamMembersModel> Get(out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_SalesTeamMembers", UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_SalesTeamMembersModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }

        /// <summary>
        /// Lấy danh sách user quản lý SalesTeamMembers theo businessOpportunityID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_SalesTeamMembersModel> GetManagementByBOID(int businessOpportunityID)
        {
            var rawKey = string.Concat("GetManagementByBusinessOpportunityID", businessOpportunityID);
            var data = GetCacheItem(rawKey) as List<RM_SalesTeamMembersModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.GetManagementByBOID(businessOpportunityID);
            AddCacheItem(rawKey, data); return data;
        }
    }
}
