using System.ComponentModel;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Caching;

namespace Core.Cate.Caches
{
    public class RM_OpportunityDashboardCache : CacheLayer
    {
        private RM_OpportunityDashboardBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_OpportunityDashboardCache", "CENIT.APP.Cache" };
        private RM_OpportunityDashboardBiz Api => _api ?? (_api = new RM_OpportunityDashboardBiz());

        private string BuildKey(DashboardSearchModel search)
        {
            var empKey = search.EmployeeIds ?? "ALL";
            return $"Dashboard_ViewModel_{search.FromDate:yyyyMMdd}_{search.ToDate:yyyyMMdd}_{empKey}";
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_OpportunityDashboardViewModel GetViewModel(DashboardSearchModel search)
        {
            var rawKey = BuildKey(search);
            var data = GetCacheItem(rawKey) as RM_OpportunityDashboardViewModel;
            if (data != null) return data;
            data = Api.GetViewModel(search);
            AddCacheItem(rawKey, data);
            return data;
        }
    }
}