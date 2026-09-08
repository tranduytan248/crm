using Core.API.Biz;
using Core.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Caching;

namespace Core.API.Caches
{
    public class KWC_MobileOTPCache : CacheLayer
    {
        private KWC_MobileOTPBiz _KWC_MobileOTPApi;
        protected override string[] MasterCacheKeyArray => new[] { "KWC_MobileOTPCache", "CENIT.APP.Cache" };
        private KWC_MobileOTPBiz Api => _KWC_MobileOTPApi ?? (_KWC_MobileOTPApi = new KWC_MobileOTPBiz());

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(KWC_MobileOTPModel model)
        {
            var login = Api.Save(model);
            if (login > 0) InvalidateCache();
            return login;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public KWC_MobileOTPModel GetById(long ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("KWC_MobileOTPModel_GetById_", ID);
            var data = GetCacheItem(rawKey) as KWC_MobileOTPModel;
            if (data != null) return data;
            data = Api.GetByID(ID);
            AddCacheItem(rawKey, data); return data;
        }
    }
}
