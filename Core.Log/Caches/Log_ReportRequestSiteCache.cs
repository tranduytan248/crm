using Core.Log.Biz;
using Core.Log.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Caching;

namespace Core.Log.Caches
{
    public class Log_ReportRequestSiteCache : CacheLayer
    {
        private Log_ReportRequestSiteBiz _categoryApi;

        private Log_ReportRequestSiteBiz Api => _categoryApi ?? (_categoryApi = new Log_ReportRequestSiteBiz());

        protected override string[] MasterCacheKeyArray =>
            new[] { "Log_ReportRequestSiteCache", "CENIT.APP.Cache" };

        /// <summary>
        /// luu thong tin vao database
        /// </summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(Log_ReportRequestSiteModel model)
        {
            var categoryId = Api.Save(model);
            if (categoryId > 0)
                // Invalidate the cache
                InvalidateCache();
            return categoryId;
        }

        /// <summary>
        /// Get thong tin 
        /// </summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public Log_ReportRequestSiteTotalModel Get()
        {
            // Item not found in cache - retrieve it and insert it into the cache
            var data = Api.Get();
            return data;
        }
    }
}
