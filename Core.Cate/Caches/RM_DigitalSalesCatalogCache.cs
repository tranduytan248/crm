using System.Collections.Generic;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_DigitalSalesStatusCache : CacheLayer
    {
        private readonly RM_DigitalSalesStatusBiz _biz = new RM_DigitalSalesStatusBiz();
        protected override string[] MasterCacheKeyArray => new[] { "RM_DigitalSalesCatalogCache", "CENIT.APP.Cache" };

        public List<RM_DigitalSalesStatusModel> Get(out int total, RM_DigitalSalesStatusSearchModel search)
        {
            var key = "DigitalSalesStatus_Get_" + UtilEncrypt.FromObject(search);
            var totalKey = key + "_Total";
            var cached = GetCacheItem(key) as List<RM_DigitalSalesStatusModel>;
            total = (int?)GetCacheItem(totalKey) ?? 0;
            if (cached != null) return cached;
            cached = _biz.Get(out total, search);
            AddCacheItem(key, cached); AddCacheItem(totalKey, total);
            return cached;
        }

        public List<RM_DigitalSalesStatusModel> GetAll(int? businessType = null)
        {
            var key = "DigitalSalesStatus_GetAll_" + (businessType?.ToString() ?? "all");
            var cached = GetCacheItem(key) as List<RM_DigitalSalesStatusModel>;
            if (cached != null) return cached;
            cached = _biz.GetAll(businessType); AddCacheItem(key, cached); return cached;
        }

        public RM_DigitalSalesStatusModel GetById(int id) => _biz.GetById(id);
        public int Save(RM_DigitalSalesStatusModel model, string username) { var result = _biz.Save(model, username); if (result > 0) InvalidateCache(); return result; }
        public int Delete(int id, string username) { var result = _biz.Delete(id, username); if (result > 0) InvalidateCache(); return result; }
    }

    public class RM_DigitalSalesProcessCache : CacheLayer
    {
        private readonly RM_DigitalSalesProcessBiz _biz = new RM_DigitalSalesProcessBiz();
        protected override string[] MasterCacheKeyArray => new[] { "RM_DigitalSalesCatalogCache", "CENIT.APP.Cache" };

        public List<RM_DigitalSalesProcessModel> Get(out int total, RM_DigitalSalesProcessSearchModel search)
        {
            var key = "DigitalSalesProcess_Get_" + UtilEncrypt.FromObject(search);
            var totalKey = key + "_Total";
            var cached = GetCacheItem(key) as List<RM_DigitalSalesProcessModel>;
            total = (int?)GetCacheItem(totalKey) ?? 0;
            if (cached != null) return cached;
            cached = _biz.Get(out total, search); AddCacheItem(key, cached); AddCacheItem(totalKey, total); return cached;
        }

        public RM_DigitalSalesProcessModel GetById(int id) => _biz.GetById(id);
        public int Save(RM_DigitalSalesProcessModel model, string username) { var result = _biz.Save(model, username); if (result > 0) InvalidateCache(); return result; }
        public int Delete(int id, string username) { var result = _biz.Delete(id, username); if (result > 0) InvalidateCache(); return result; }
    }

    public class RM_DigitalSalesProgressCache : CacheLayer
    {
        private readonly RM_DigitalSalesProgressBiz _biz = new RM_DigitalSalesProgressBiz();
        protected override string[] MasterCacheKeyArray => new[] { "RM_DigitalSalesCatalogCache", "CENIT.APP.Cache" };

        public List<RM_DigitalSalesProgressModel> GetByProcess(int processId) => _biz.GetByProcess(processId);
        public RM_DigitalSalesProgressModel GetById(int id) => _biz.GetById(id);
        public int Save(RM_DigitalSalesProgressModel model, string username) { var result = _biz.Save(model, username); if (result > 0) InvalidateCache(); return result; }
        public int Delete(int id, string username) { var result = _biz.Delete(id, username); if (result > 0) InvalidateCache(); return result; }
    }
}
