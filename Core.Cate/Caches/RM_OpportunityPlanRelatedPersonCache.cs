using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    /// <summary>
    /// Cache dữ liệu người liên quan của kế hoạch cơ hội kinh doanh.
    /// </summary>
    public class RM_OpportunityPlanRelatedPersonCache : CacheLayer
    {
        private RM_OpportunityPlanRelatedPersonBiz _api;
        private RM_DashboardCache _dashboardCache;
        protected override string[] MasterCacheKeyArray => new[] { "RM_OpportunityPlanRelatedPersonCache", "CENIT.APP.Cache" };
        private RM_OpportunityPlanRelatedPersonBiz Api => _api ?? (_api = new RM_OpportunityPlanRelatedPersonBiz());
        private RM_DashboardCache DashboardCache => _dashboardCache ?? (_dashboardCache = new RM_DashboardCache());


        /// <summary>
        /// Xóa người liên quan khỏi kế hoạch và đồng bộ xóa cache dashboard liên quan.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_OpportunityPlanRelatedPersonModel model, string deletedBy)
        {
            var result = Api.Delete(model.Id, deletedBy);
            if (result > 0)
            {
                InvalidateCache();
                DashboardCache.ClearCache();
            }
            return result;
        }

        /// <summary>
        /// Lưu nhiều người liên quan cho một kế hoạch và đồng bộ làm mới cache dashboard.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int SaveMulti(RM_OpportunityPlanRelatedPersonFormModel model, string savedBy)
        {
            var result = Api.SaveMulti(model, savedBy);
            if (result > 0)
            {
                InvalidateCache();
                DashboardCache.ClearCache();
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách người liên quan theo kế hoạch và điều kiện tìm kiếm.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_OpportunityPlanRelatedPersonModel> GetByOpportunityPlanID(int planID, out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_OpportunityPlanRelatedPerson_", UtilEncrypt.FromObject(search), planID);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_OpportunityPlanRelatedPersonModel>;
            if (data != null && data.Count > 0) return data;
            data = Api.GetByOpportunityPlanID(planID, out total, search);
            AddCacheItem(rawKey, data); 
            AddCacheItem(rawKeyTotal, total);
            return data;
        }
    }
}
