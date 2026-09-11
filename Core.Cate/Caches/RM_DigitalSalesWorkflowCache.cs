using Core.Cate.Biz;
using Core.Cate.Models;
using System.Collections.Generic;
using System.ComponentModel;
using TSFramework.Libs.Models.Caching;

namespace Core.Cate.Caches
{
    public class RM_DigitalSalesWorkflowCache : CacheLayer
    {
        private RM_DigitalSalesWorkflowBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_DigitalSalesWorkflowCache", "CENIT.APP.Cache" };
        private RM_DigitalSalesWorkflowBiz Api => _api ?? (_api = new RM_DigitalSalesWorkflowBiz());

        #region Status
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_DigitalSalesStatusModel> GetStatuses(out int total, string search = null, byte? businessType = null, string order = "0", string orderDir = "ASC", int pageIndex = 0, int pageSize = 100)
        {
            var rawKey = string.Concat("RM_DigitalSalesStatus_Get_", search, "_", businessType, "_", order, "_", orderDir, "_", pageIndex, "_", pageSize);
            var rawKeyTotal = string.Concat(rawKey, "_Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_DigitalSalesStatusModel>;
            if (data != null) return data;

            data = Api.GetStatuses(out total, search, businessType, order, orderDir, pageIndex, pageSize);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }

        public List<RM_DigitalSalesStatusModel> GetAllStatuses(byte? businessType = null)
        {
            var rawKey = string.Concat("RM_DigitalSalesStatus_GetAll_", businessType);
            var data = GetCacheItem(rawKey) as List<RM_DigitalSalesStatusModel>;
            if (data != null) return data;

            data = Api.GetAllStatuses(businessType);
            AddCacheItem(rawKey, data);
            return data;
        }

        public RM_DigitalSalesStatusModel GetStatusByID(int statusId)
        {
            if (statusId <= 0) return null;
            var rawKey = string.Concat("RM_DigitalSalesStatus_GetByID_", statusId);
            var data = GetCacheItem(rawKey) as RM_DigitalSalesStatusModel;
            if (data != null) return data;

            data = Api.GetStatusByID(statusId);
            AddCacheItem(rawKey, data);
            return data;
        }

        public int SaveStatus(RM_DigitalSalesStatusModel model, string username)
        {
            var result = Api.SaveStatus(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public int DeleteStatus(int statusId, string username)
        {
            var result = Api.DeleteStatus(statusId, username);
            if (result > 0) InvalidateCache();
            return result;
        }
        #endregion

        #region Process
        public List<RM_DigitalSalesProcessModel> GetProcesses(out int total, string search = null, byte? businessType = null, int? statusId = null, string order = "0", string orderDir = "ASC", int pageIndex = 0, int pageSize = 100)
        {
            var rawKey = string.Concat("RM_DigitalSalesProcess_Get_", search, "_", businessType, "_", statusId, "_", order, "_", orderDir, "_", pageIndex, "_", pageSize);
            var rawKeyTotal = string.Concat(rawKey, "_Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_DigitalSalesProcessModel>;
            if (data != null) return data;

            data = Api.GetProcesses(out total, search, businessType, statusId, order, orderDir, pageIndex, pageSize);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }

        public RM_DigitalSalesProcessModel GetProcessByID(int processId)
        {
            if (processId <= 0) return null;
            var rawKey = string.Concat("RM_DigitalSalesProcess_GetByID_", processId);
            var data = GetCacheItem(rawKey) as RM_DigitalSalesProcessModel;
            if (data != null) return data;

            data = Api.GetProcessByID(processId);
            AddCacheItem(rawKey, data);
            return data;
        }

        public int SaveProcess(RM_DigitalSalesProcessModel model, string username)
        {
            var result = Api.SaveProcess(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public int DeleteProcess(int processId, string username)
        {
            var result = Api.DeleteProcess(processId, username);
            if (result > 0) InvalidateCache();
            return result;
        }
        #endregion

        #region Progress
        public List<RM_DigitalSalesProgressModel> GetProgressesByProcess(int processId)
        {
            if (processId <= 0) return new List<RM_DigitalSalesProgressModel>();
            var rawKey = string.Concat("RM_DigitalSalesProgress_GetByProcess_", processId);
            var data = GetCacheItem(rawKey) as List<RM_DigitalSalesProgressModel>;
            if (data != null) return data;

            data = Api.GetProgressesByProcess(processId);
            AddCacheItem(rawKey, data);
            return data;
        }

        public RM_DigitalSalesProgressModel GetProgressByID(int progressId)
        {
            if (progressId <= 0) return null;
            var rawKey = string.Concat("RM_DigitalSalesProgress_GetByID_", progressId);
            var data = GetCacheItem(rawKey) as RM_DigitalSalesProgressModel;
            if (data != null) return data;

            data = Api.GetProgressByID(progressId);
            AddCacheItem(rawKey, data);
            return data;
        }

        public int SaveProgress(RM_DigitalSalesProgressModel model, string username)
        {
            var result = Api.SaveProgress(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public int DeleteProgress(int progressId, string username)
        {
            var result = Api.DeleteProgress(progressId, username);
            if (result > 0) InvalidateCache();
            return result;
        }
        #endregion
    }
}
