using System.Collections.Generic;
using System.ComponentModel;
using Core.Sys.Biz.Sys;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Sys.Caches.Sys
{
    [DataObject]
    public class SysSMSTemplateCache : CacheLayer
    {
        private SysSMSTemplateBiz _smsTemplateApi;

        private SysSMSTemplateBiz Api => _smsTemplateApi ?? (_smsTemplateApi = new SysSMSTemplateBiz());

        protected override string[] MasterCacheKeyArray =>
            new[] { "SysSMSTemplatesCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysSMSTemplateModel> GetAll()
        {
            const string rawKey = "AllSMSTemplate";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysSMSTemplateModel> smsTemplate) return smsTemplate;
            // Item not found in cache - retrieve it and insert it into the cache
            smsTemplate = Api.GetAll();
            AddCacheItem(rawKey, smsTemplate);

            return smsTemplate;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysSMSTemplateModel> Get(out int total, BaseSearchModel search = null)
        {
            var objectKey = UtilEncrypt.FromObject(search);
            var rawKey = string.Concat("ListSMSTemplate-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            // See if the item is in the cache
            var smsTemplate = GetCacheItem(rawKey) as List<SysSMSTemplateModel>;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            if (smsTemplate != null) return smsTemplate;
            // Item not found in cache - retrieve it and insert it into the cache
            smsTemplate = Api.GetList(out total, search);
            AddCacheItem(rawKey, smsTemplate);
            AddCacheItem(rawKeyTotal, total);

            return smsTemplate;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysSMSTemplateModel GetById(int id)
        {
            if (id < 0) return null;

            var rawKey = string.Concat("SysSMSTemplateByID-", id);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is SysSMSTemplateModel result) return result;
            // Item not found in cache - retrieve it and insert it into the cache
            result = Api.GetById(id);
            if (result != null) AddCacheItem(rawKey, result);

            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysSMSTemplateModel GetByCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return null;

            var rawKey = string.Concat("SysSMSTemplateByCode-", code);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is SysSMSTemplateModel result) return result;
            // Item not found in cache - retrieve it and insert it into the cache
            result = Api.GetByCode(code);
            if (result != null) AddCacheItem(rawKey, result);

            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(SysSMSTemplateModel model, string userName)
        {
            var result = Api.Save(model, userName);
            if (result > 0)
                // Invalidate the cache
                InvalidateCache();
            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysSMSTemplateModel model, string userName)
        {
            var isDeleted = Api.Delete(model, userName);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? InsertLog(SysSMSLogsModel model, string userName)
        {
            var result = Api.InsertLog(model, userName);
            if (result > 0)
                // Invalidate the cache
                InvalidateCache();
            return result;
        }
    }
}