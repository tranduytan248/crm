using Core.Cate.Biz;
using Core.Cate.Models;
using System.Collections.Generic;
using System.ComponentModel;
using TSFramework.Libs.Models.Caching;

namespace Core.Cate.Caches
{
    /// <summary>
    /// Cache dữ liệu kế hoạch cơ hội kinh doanh.
    /// </summary>
    public class RM_OpportunityPlanCache : CacheLayer
    {
        private RM_OpportunityPlanBiz _api;

        protected override string[] MasterCacheKeyArray => new[] { "RM_OpportunityPlanCache", "CENIT.APP.Cache" };

        private RM_OpportunityPlanBiz Api => _api ?? (_api = new RM_OpportunityPlanBiz());

        /// <summary>
        /// Lấy danh sách kế hoạch theo BusinessOpportunityID
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_OpportunityPlanModel> GetByOpportunityId(int businessOpportunityId)
        {
            var key = $"GetRM_OpportunityPlanByOpportunityId_{businessOpportunityId}";
            var data = GetCacheItem(key) as List<RM_OpportunityPlanModel>;
            if (data != null) return data;
            data = Api.GetByOpportunityId(businessOpportunityId);
            AddCacheItem(key, data);
            return data;
        }

        /// <summary>
        /// Lấy chi tiết kế hoạch theo Id
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_OpportunityPlanModel GetById(int id)
        {
            if (id <= 0) return null;
            var key = $"GetRM_OpportunityPlanById_{id}";
            var data = GetCacheItem(key) as RM_OpportunityPlanModel;
            if (data != null) return data;
            data = Api.GetById(id);
            AddCacheItem(key, data);
            return data;
        }

        /// <summary>
        /// Thêm mới / Cập nhật kế hoạch
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_OpportunityPlanModel model, string username)
        {
            var result = Api.Save(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        /// <summary>
        /// Xóa mềm kế hoạch theo Id
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(int id, string username)
        {
            var result = Api.Delete(id, username);
            if (result > 0) InvalidateCache();
            return result;
        }
    }
}
