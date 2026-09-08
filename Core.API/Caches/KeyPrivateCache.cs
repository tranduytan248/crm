using Core.API.Biz;
using Core.API.Models;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.ComponentModel;

using TSFramework.Libs.Models.Caching;

namespace Core.API.Caches
{
    public class KeyPrivateCache : CacheLayer
    {
        private KeyPrivateBiz _dnKeyPrivaceBiz;
        protected override string[] MasterCacheKeyArray => new[]
        {
            "KeyPrivacyCache"
        };

        private KeyPrivateBiz Api => _dnKeyPrivaceBiz ?? (_dnKeyPrivaceBiz = new KeyPrivateBiz());


        /// <summary>
        /// Check tài khoản người dùng (username, password)
        /// </summary>
        /// <returns>Thông tin Key so với username và applyFor</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public KeyPrivateModel ValidAccount(string username, string password, string applyFor)
        {
            var keyPrivacies = Api.ValidAccount(username, password, applyFor);
            if (keyPrivacies == null) return null;

            return keyPrivacies;
        }
        
        /// <summary>
        /// Đăng nhập cho doanh nghiệp
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public KeyPrivateModel ValidateBusinessAccount(string username, string password)
        {
            var keyPrivacies = Api.ValidateBusinessAccount(username, password);
            if (keyPrivacies == null) return null;
            return keyPrivacies;
        }

        /// <summary>
        /// Lưu thông tin token sau khi Authen thành công
        /// </summary>
        /// <param name="token">Token được khởi tạo ngẫu nhiên</param>
        /// <param name="username">Tài khoản truy cập</param>
        public void SaveTokenByUser(KeyPrivateModel token, string username)
        {
            var rawKey = string.Concat("InfoToken-", username);
            AddCacheItem(rawKey, token);
        }
     
        /// <summary>
        /// Lấy thông tin token từ thông tin username
        /// </summary>
        /// <param name="username">Tài khoản truy cập</param>
        /// <returns></returns>
        public KeyPrivateModel GetTokenByUser(string username)
        {
            var rawKey = string.Concat("InfoToken-", username);
            var token = GetCacheItem(rawKey) as KeyPrivateModel;
            return token;
        }

        /// <summary>
        /// Kiểm tra tài khoản có phải là tài khoản doanh nghiêp
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public bool IsBusinessAccount(string username)
        {
            var result = Api.IsBusinessAccount(username);
            return result;
        }

        /// <summary>
        /// Kiểm tra token còn hạn
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool GetToken(string token)
        {
            var data = GetCacheItem(token) as KeyPrivateModel;
            if (data == null)
            {
                data = Api.GetToken(token);
                if (data == null) return false;
                SaveToken(data, token);
            }
            return true;
        }

        /// <summary>
        /// Lấy thông tin token từ thông tin username
        /// </summary>
        /// <param name="username">Tài khoản truy cập</param>
        /// <returns></returns>
        public KeyPrivateModel GetDataByToken(string token)
        {
            var data = GetCacheItem(token) as KeyPrivateModel;
            if (data == null)
            {
                data = Api.GetToken(token);
                if(data == null) return null;   
                SaveToken(data,token);
            }
            return data;
        }

        /// <summary>
        /// Lưu thông tin token
        /// </summary>
        /// <param name="data">Gồm token và username</param>
        /// <param name="token">key</param>
        public void SaveToken(KeyPrivateModel data, string token)
        {
            InvalidateCache(token);
            AddCacheItem(token, data);
        }
    }
}
