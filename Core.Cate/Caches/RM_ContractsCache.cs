using Core.Cate.Biz;
using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_ContractsCache : CacheLayer
    {
        private RM_ContractsBiz _RM_ContractsApi;
        protected override string[] MasterCacheKeyArray => new[] { "RM_ContractsCache", "CENIT.APP.Cache" };

        private RM_ContractsBiz Api => _RM_ContractsApi ?? (_RM_ContractsApi = new RM_ContractsBiz());

        /// <summary>
        /// Lấy toàn bộ danh sách RM_Contracts
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ContractsModel> Get(out int total, RM_ContractsSearchModel model, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_RM_Contracts", UtilEncrypt.FromObject(model), UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_ContractsModel>;
            if (data != null) return data;
            data = Api.LoadList(out total, model, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }


        /// <summary>
        /// Lấy toàn bộ danh sách RM_Contracts
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ContractsModel> GetAll()
        {
            var rawKey = string.Concat("GetAllRM_Contracts");
            var data = GetCacheItem(rawKey) as List<RM_ContractsModel>;
            if (data != null) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tin RM_Contracts theo ID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_ContractsModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ContractsByID_", ID);
            var data = GetCacheItem(rawKey) as RM_ContractsModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tin RM_ContractsModel theo CustomerID(
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ContractsModel> GetByCustomerID(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ContractsByCustomerID_", ID);
            var data = GetCacheItem(rawKey) as List<RM_ContractsModel>;
            if (data != null) return data;
            data = Api.GetByCustomerID(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_ContractsModel model, string username)
        {
            var isDeleted = Api.Delete(model, username);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Thêm mới Hợp đồng
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_ContractsModel model,DataTable ngayNhacs, string username)
        {
            var isSaved = Api.Save(model, ngayNhacs, username);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        ///// <summary>
        ///// Lấy thông tin RM_Contracts theo ProjectID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ContractsModel> GetByProjectID(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ContractsByProjectID_", ID);
            var data = GetCacheItem(rawKey) as List<RM_ContractsModel>;
            if (data != null) return data;
            data = Api.GetByProjectID(ID);
            AddCacheItem(rawKey, data); return data;
        }

        ///// <summary>
        ///// Lấy thông tin RM_Contracts theo ProductProjectID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_ContractsModel> GetByProductProjectID(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetRM_ContractsByProjectID_", ID);
            var data = GetCacheItem(rawKey) as List<RM_ContractsModel>;
            if (data != null) return data;
            data = Api.GetByProductProjectID(ID);
            AddCacheItem(rawKey, data); return data;
        }
    }
}
