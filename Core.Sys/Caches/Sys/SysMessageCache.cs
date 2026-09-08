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
    public class SysMessageCache : CacheLayer
    {
        private SysMessageBiz _messageApi;

        private SysMessageBiz Api => _messageApi ?? (_messageApi = new SysMessageBiz());

        protected override string[] MasterCacheKeyArray => new[] { "SysMessagesCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysMessageModel> GetByLangCode(string langCode)
        {
            var rawKey = string.Concat("MessagesByLangCode-", langCode);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysMessageModel> messages) return messages;
            // Item not found in cache - retrieve it and insert it into the cache
            messages = Api.GetByLangCode(langCode);
            AddCacheItem(rawKey, messages);

            return messages;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysMessageModel> GetAll()
        {
            var rawKey = "AllMessages";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<SysMessageModel> messages) return messages;
            // Item not found in cache - retrieve it and insert it into the cache
            messages = Api.GetAll();
            AddCacheItem(rawKey, messages);

            return messages;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysMessageModel> Get(out int total, BaseSearchModel search = null)
        {
            var objectKey = UtilEncrypt.FromObject(search);
            var rawKey = string.Concat("ListMessages-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            // See if the item is in the cache
            var messages = GetCacheItem(rawKey) as List<SysMessageModel>;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            if (messages != null) return messages;
            // Item not found in cache - retrieve it and insert it into the cache
            messages = Api.GetList(out total, search);
            AddCacheItem(rawKey, messages);
            AddCacheItem(rawKeyTotal, total);

            return messages;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysMessageModel GetByKey(string langCode, string labelKey)
        {
            if (string.IsNullOrEmpty(langCode) && string.IsNullOrEmpty(labelKey)) return null;

            var rawKey = string.Concat("MessageByLangKey-", langCode, "-", labelKey);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is SysMessageModel message) return message;
            // Item not found in cache - retrieve it and insert it into the cache
            message = Api.GetByKey(langCode, labelKey);
            if (message != null) AddCacheItem(rawKey, message);

            return message;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysMessageModel GetById(string messageId)
        {
            if (string.IsNullOrEmpty(messageId) && string.IsNullOrEmpty(messageId)) return null;

            var rawKey = string.Concat("MessageByID-", messageId);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is SysMessageModel message) return message;
            // Item not found in cache - retrieve it and insert it into the cache
            message = Api.GetById(messageId);
            if (message != null) AddCacheItem(rawKey, message);

            return message;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(SysMessageModel model)
        {
            var messageId = Api.Save(model);
            if (messageId > 0)
                // Invalidate the cache
                InvalidateCache();
            return messageId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysMessageModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}