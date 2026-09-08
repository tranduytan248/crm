using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using System.ComponentModel;
using TSFramework.Libs.Utils;


namespace Core.Cate.Caches
{
    public class OpportunityProjectWithoutPGPCache : CacheLayer
    {
        private OpportunityProjectWithoutPGPBiz _OpportunityProjectWithoutPGPApi;
        protected override string[] MasterCacheKeyArray => new[] { "OpportunityProjectWithoutPGPCache", "CENIT.APP.Cache" };
        private OpportunityProjectWithoutPGPBiz Api => _OpportunityProjectWithoutPGPApi ?? (_OpportunityProjectWithoutPGPApi = new OpportunityProjectWithoutPGPBiz());

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<OpportunityProjectWithoutPGPModel> Get(out int total, OpportunityProjectWithoutPGSearchPModel searchModel = null, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetOpportunityProjectWithoutPGP_", UtilEncrypt.FromObject(searchModel), UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            var data = GetCacheItem(rawKey) as List<OpportunityProjectWithoutPGPModel>;
            if (data != null) return data;

            data = Api.LoadList(out total, searchModel, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }
    }
}
