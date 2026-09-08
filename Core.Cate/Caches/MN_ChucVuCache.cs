using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using Core.Cate.Models;
using Core.Cate.Biz;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class MN_ChucVuCache : CacheLayer
    {
        private MN_ChucVuBiz _MN_ChucVuApi;
        protected override string[] MasterCacheKeyArray => new[] { "MN_ChucVuCache", "CENIT.APP.Cache" };

        private MN_ChucVuBiz Api => _MN_ChucVuApi ?? (_MN_ChucVuApi = new MN_ChucVuBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(MN_ChucVuModel model, string deletedBy)
        {
            var isDeleted = Api.Delete(model.ChucVu_ID, deletedBy);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin MN_ChucVu
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(MN_ChucVuModel model, string savedBy)
        {
            var isSaved = Api.Save(model, savedBy);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách MN_ChucVu
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<MN_ChucVuModel> GetAll()
        {
            var rawKey = string.Concat("GetAllMN_ChucVu");
            var data = GetCacheItem(rawKey) as List<MN_ChucVuModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy thông tinMN_ChucVu theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public MN_ChucVuModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetMN_ChucVuByID_", ID);
            var data = GetCacheItem(rawKey) as MN_ChucVuModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách MN_ChucVu
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<MN_ChucVuModel> Get(out int total, MN_ChucVuSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_MN_ChucVu", UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<MN_ChucVuModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }

        public List<MN_ChucVuExportModel> Export(MN_ChucVuSearchModel search = null)
        {
            var rawKey = string.Concat("ExportSearch_MN_ChucVu", UtilEncrypt.FromObject(search));
            var data = GetCacheItem(rawKey) as List<MN_ChucVuExportModel>;
            if (data != null) return data;

            data = Api.Export(search);

            AddCacheItem(rawKey, data);

            return data;
        }

    }
}
