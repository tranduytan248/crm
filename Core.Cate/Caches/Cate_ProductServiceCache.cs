using Core.Cate.Biz;
using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class Cate_ProductServiceCache : CacheLayer
    {
        private Cate_ProductServiceBiz _Cate_ProductServiceApi;
        protected override string[] MasterCacheKeyArray => new[] { "Cate_ProductServiceCache", "CENIT.APP.Cache" };
        private Cate_ProductServiceBiz Api => _Cate_ProductServiceApi ?? (_Cate_ProductServiceApi = new Cate_ProductServiceBiz());

        /// <summary>
        /// Lấy toàn bộ danh sách
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<Cate_ProductServiceModel> GetAll()
        {
            var rawKey = string.Concat("GetAllCate_ProductService");
            var data = GetCacheItem(rawKey) as List<Cate_ProductServiceModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<Cate_ProductServiceModel> GetAllChild(string keyword = null, string SearchFile=null)
        {
            var rawKey = string.Concat("GetAllChildCate_ProductService", keyword, "_", SearchFile);
            var data = GetCacheItem(rawKey) as List<Cate_ProductServiceModel>;
            if (data != null) return data;
            data = Api.GetAllChild(keyword, SearchFile);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<Cate_ProductServiceModel> Get(out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_Cate_ProductService", UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<Cate_ProductServiceModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }

        /// <summary>
        /// Lấy thông tinCate_ProductService theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public Cate_ProductServiceModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetCate_ProductServiceByID_", ID);
            var data = GetCacheItem(rawKey) as Cate_ProductServiceModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lưu thông tin
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(Cate_ProductServiceModel model, string username)
        {
            var isSaved = Api.Save(model, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(Cate_ProductServiceModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }


    }
}
