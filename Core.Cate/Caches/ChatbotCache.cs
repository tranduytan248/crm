using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using Core.Cate.Biz;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class ChatbotCache : CacheLayer
    {
        private ChatbotBiz _ChatbotApi;
        protected override string[] MasterCacheKeyArray => new[] { "ChatbotCache", "CENIT.APP.Cache" };

        private ChatbotBiz Api => _ChatbotApi ?? (_ChatbotApi = new ChatbotBiz());
       
        /// <summary>
        /// Lấy toàn bộ dữ liệu chatbot
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<ChatbotModel> GetData(string username)
        {
            var rawKey = string.Concat("GetSearch_Chatbot", username);
            var data = GetCacheItem(rawKey) as List<ChatbotModel>;
            if (data != null)
                return data;
            data = Api.GetData(username);
            AddCacheItem(rawKey, data);
            return data;
        }
    }
}
