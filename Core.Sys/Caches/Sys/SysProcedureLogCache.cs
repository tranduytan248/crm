using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Sys.Biz.Sys;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Caching;

namespace Core.Sys.Caches.Sys
{
    [DataObject]
    public class SysProcedureLogCache : CacheLayer
    {
        private SysProcedureLogBiz _procLogApi;

        private SysProcedureLogBiz Api => _procLogApi ?? (_procLogApi = new SysProcedureLogBiz());
        protected override string[] MasterCacheKeyArray => new[] { "SysProcedureLogCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysProcedureLogModel> GetAll(DateTime? fromMonth, DateTime? toMonth)
        {
            string rawKey = $"AllProcedureLog-{fromMonth}-{toMonth}";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysProcedureLogModel> procedureLogs) return procedureLogs;
            // Item not found in cache - retrieve it and insert it into the cache
            procedureLogs = Api.GetAll(fromMonth, toMonth);
            AddCacheItem(rawKey, procedureLogs);

            return procedureLogs;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysProcedureLogModel> Get(DateTime? fromMonth, DateTime? toMonth)
        {
            var rawKey = $"ListProcedureLogs-{fromMonth}-{toMonth}";
            // See if the item is in the cache

            if (GetCacheItem(rawKey) is List<SysProcedureLogModel> procedureLogs) return procedureLogs;
            // Item not found in cache - retrieve it and insert it into the cache
            procedureLogs = Api.GetList(fromMonth, toMonth);
            AddCacheItem(rawKey, procedureLogs);

            return procedureLogs;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysProcedureLogModel GetById(Guid? logId)
        {
            if (logId == null || logId == Guid.Empty) return null;

            var rawKey = string.Concat("SysProcedureLogByID-", logId);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is SysProcedureLogModel procedureLog) return procedureLog;
            // Item not found in cache - retrieve it and insert it into the cache
            procedureLog = Api.GetById(logId);
            if (procedureLog != null) AddCacheItem(rawKey, procedureLog);

            return procedureLog;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysProcedureLogModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool DeleteAll(DateTime? fromMonth, DateTime? toMonth)
        {
            var isDeleted = Api.DeleteAll(fromMonth, toMonth);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}