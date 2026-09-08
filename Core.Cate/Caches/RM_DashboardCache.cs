using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Models;
using TSFramework.Libs.Models.Caching;
using Core.Cate.Biz;
using System.Linq;
using TSFramework.Libs.Utils;
using System;

namespace Core.Cate.Caches
{
    public class RM_DashboardCache : CacheLayer
    {
        private RM_DashboardBiz _RM_DashboardApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_DashboardCache", "CENIT.APP.Cache" };
        private RM_DashboardBiz Api => _RM_DashboardApi ?? (_RM_DashboardApi = new RM_DashboardBiz());

        private string BuildKey(string prefix, DashboardSearchModel search)
        {
            // Key rõ ràng theo từng tham số — tránh serialize object ra null nhầm
            var empKey = search.EmployeeIds ?? "ALL";
            return $"{prefix}_{search.FromDate:yyyyMMdd}_{search.ToDate:yyyyMMdd}_{empKey}_{search.Type}";
        }

        /// <summary>
        /// Danh sách cảnh báo được cache theo ngày và phạm vi nhân sự.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_StaleUpdateModel> GetStaleUpdates(string employeeIds)
        {
            var rawKey = $"StaleUpdates_{DateTime.Today:yyyyMMdd}_{employeeIds ?? "ALL"}";
            var data = GetCacheItem(rawKey) as List<RM_StaleUpdateModel>;
            if (data != null) return data;

            data = Api.GetStaleUpdates(employeeIds);
            AddCacheItem(rawKey, data);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_OverviewDashboardModel GetOverview(DashboardSearchModel search)
        {
            var rawKey = BuildKey("Overview", search);
            var data = GetCacheItem(rawKey) as RM_OverviewDashboardModel;
            if (data != null) return data;
            data = Api.GetOverview(search);
            AddCacheItem(rawKey, data);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<ProjectDashboardModel> GetProjects(out int total, DashboardSearchModel search)
        {
            var rawKey = BuildKey("Projects", search);
            var rawKeyTotal = rawKey + "-Total";
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<ProjectDashboardModel>;
            if (data != null) return data;
            data = Api.GetProjects(out total, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<ProjectDashboardModel> GetProjectsV2(out int total, DashboardSearchModel search)
        {
            var rawKey = string.Concat("ProjectsV2", UtilEncrypt.FromObject(search));
            var rawKeyTotal = rawKey + "-Total";
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<ProjectDashboardModel>;
            if (data != null) return data;
            data = Api.GetProjectsV2(out total, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<DataForPieChartModel> GetPieChart(int isProject, DashboardSearchModel search)
        {
            var rawKey = string.Concat("GetPieChart", isProject, "-", BuildKey("Pie", search));
            var data = GetCacheItem(rawKey) as List<DataForPieChartModel>;
            // Cache cả kết quả rỗng — nếu không, mỗi request đều đâm xuống DB khi không có dữ liệu
            if (data != null) return data;
            data = Api.GetPieChart(isProject, search);
            AddCacheItem(rawKey, data); return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<OpportunityDashboardModel> GetOpportunities(out int total, DashboardSearchModel search)
        {
            var rawKey = BuildKey("Opportunities", search);
            var rawKeyTotal = rawKey + "-Total";
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<OpportunityDashboardModel>;
            if (data != null) return data;
            data = Api.GetOpportunities(out total, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<OpportunityDashboardModel> GetOpportunitiesV2(out int total, DashboardSearchModel search)
        {
            var rawKey = string.Concat("OpportunitiesV2", UtilEncrypt.FromObject(search));
            var rawKeyTotal = rawKey + "-Total";
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<OpportunityDashboardModel>;
            if (data != null) return data;
            data = Api.GetOpportunitiesV2(out total, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }

        /// <summary>
        /// Lấy danh sách kế hoạch kinh doanh (có cache)
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<PlanDashboardModel> GetPlans(out int total, DashboardSearchModel search)
        {
            var rawKey = BuildKey("Plans", search);
            var rawKeyTotal = rawKey + "-Total";

            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            var data = GetCacheItem(rawKey) as List<PlanDashboardModel>;
            if (data != null) return data;

            data = Api.GetPlans(out total, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }

        /// <summary>
        /// Lấy danh sách kế hoạch kèm người liên quan (có cache; key gồm cả Username vì kết quả phụ thuộc người xem)
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<PlanDashboardModel> GetPlansV2(out int total, DashboardSearchModel search)
        {
            var rawKey = BuildKey("PlansV2", search) + "_" + (search.Username ?? "ANY");
            var rawKeyTotal = rawKey + "-Total";

            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            var data = GetCacheItem(rawKey) as List<PlanDashboardModel>;
            if (data != null) return data;

            data = Api.GetPlansV2(out total, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }

        /// <summary>
        /// Chart số cơ hội hoặc dự án theo nhóm dịch vụ (có cache).
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<GroupServiceChartModel> GetGroupServiceChart(int isProject, DashboardSearchModel search)
        {
            var rawKey = string.Concat("GroupServiceChart", isProject, "-", BuildKey("GS", search), "_", search.Username ?? "ANY");
            var data = GetCacheItem(rawKey) as List<GroupServiceChartModel>;
            if (data != null) return data;
            data = Api.GetGroupServiceChart(isProject, search);
            AddCacheItem(rawKey, data);
            return data;
        }

        /// <summary>
        /// Danh sách cơ hội theo nhóm dịch vụ (có cache; key gồm nhóm và người xem).
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<OpportunityDashboardModel> GetOpportunitiesByGroupService(out int total, DashboardSearchModel search)
        {
            var rawKey = BuildKey("OppByGS", search) + "_" + search.GroupServiceID + "_" + (search.Username ?? "ANY");
            var rawKeyTotal = rawKey + "-Total";

            total = (int?)GetCacheItem(rawKeyTotal) ?? 0;
            var data = GetCacheItem(rawKey) as List<OpportunityDashboardModel>;
            if (data != null) return data;

            data = Api.GetOpportunitiesByGroupService(out total, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }

        /// <summary>
        /// Danh sách dự án theo nhóm dịch vụ (có cache; key gồm nhóm và người xem).
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<ProjectDashboardModel> GetProjectsByGroupService(out int total, DashboardSearchModel search)
        {
            var rawKey = BuildKey("PrjByGS", search) + "_" + search.GroupServiceID + "_" + (search.Username ?? "ANY");
            var rawKeyTotal = rawKey + "-Total";

            total = (int?)GetCacheItem(rawKeyTotal) ?? 0;
            var data = GetCacheItem(rawKey) as List<ProjectDashboardModel>;
            if (data != null) return data;

            data = Api.GetProjectsByGroupService(out total, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }

        public void ClearCache()
        {
            InvalidateCache();
        }

    }
}
