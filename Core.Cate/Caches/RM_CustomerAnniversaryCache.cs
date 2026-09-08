using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_CustomerAnniversaryCache : CacheLayer
    {
        private RM_CustomerAnniversaryBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_CustomerAnniversaryCache", "CENIT.APP.Cache" };

        private RM_CustomerAnniversaryBiz Api =>
            _api ?? (_api = new RM_CustomerAnniversaryBiz());

        /// <summary>
        /// Lưu (thêm mới / cập nhật) ngày kỷ niệm
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_CustomerAnniversaryModel model, string username)
        {
            var result = Api.Save(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        /// <summary>
        /// Xoá ngày kỷ niệm
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_CustomerAnniversaryModel model, string username)
        {
            var result = Api.Delete(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        /// <summary>
        /// Lấy chi tiết ngày kỷ niệm theo ID
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_CustomerAnniversaryModel GetById(int id)
        {
            if (id <= 0) return null;
            var rawKey = $"GetRM_CustomerAnniversaryByID_{id}";
            var data = GetCacheItem(rawKey) as RM_CustomerAnniversaryModel;
            if (data != null) return data;
            data = Api.LoadDetail(id);
            AddCacheItem(rawKey, data);
            return data;
        }

        /// <summary>
        /// Lấy danh sách ngày kỷ niệm theo điều kiện tìm kiếm (DataTables phân trang)
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_CustomerAnniversaryModel> Get(out int total,
            RM_CustomerAnniversarySearchModel searchModel = null,
            BaseSearchModel search = null)
        {
            var rawKey = $"GetSearch_RM_CustomerAnniversary_{UtilEncrypt.FromObject(searchModel)}_{UtilEncrypt.FromObject(search)}";
            var rawKeyTotal = rawKey + "-Total";

            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            var data = GetCacheItem(rawKey) as List<RM_CustomerAnniversaryModel>;
            if (data != null) return data;

            data = Api.LoadList(out total, searchModel, search);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }
    }
}
