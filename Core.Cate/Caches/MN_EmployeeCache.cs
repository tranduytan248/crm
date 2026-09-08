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
    public class MN_EmployeeCache : CacheLayer
    {
        private MN_EmployeeBiz _MN_EmployeeApi;
        protected override string[] MasterCacheKeyArray => new[] { "MN_EmployeeCache", "CENIT.APP.Cache" };

        private MN_EmployeeBiz Api => _MN_EmployeeApi ?? (_MN_EmployeeApi = new MN_EmployeeBiz());
        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(MN_EmployeeModel model, string deletedBy)
        {
            var isDeleted = Api.Delete(model.Employee_ID, deletedBy);
            if (isDeleted > 0) InvalidateCache();
            return isDeleted;
        }

        /// <summary>
        /// Lưu thông tin MN_Employee
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(MN_EmployeeModel model, string savedBy)
        {
            var isSaved = Api.Save(model, savedBy);
            if (isSaved > 0) InvalidateCache();
            return isSaved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách MN_Employee
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<MN_EmployeeModel> GetAll()
        {
            var rawKey = string.Concat("GetAllMN_Employee");
            var data = GetCacheItem(rawKey) as List<MN_EmployeeModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.GetAll();
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy thông tinMN_Employee theo ID
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public MN_EmployeeModel GetById(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("GetMN_EmployeeByID_", ID);
            var data = GetCacheItem(rawKey) as MN_EmployeeModel;
            if (data != null) return data;
            data = Api.LoadDetail(ID);
            AddCacheItem(rawKey, data); return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách MN_Employee
        /// </summary>
        /// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<MN_EmployeeModel> Get(out int total, BaseSearchModel search = null)
        {
            var rawKey = string.Concat("GetSearch_MN_Employee", UtilEncrypt.FromObject(search));
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal); total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<MN_EmployeeModel>;
            if (data != null && data.Count() > 0) return data;
            data = Api.LoadList(out total, search);
            AddCacheItem(rawKey, data); AddCacheItem(rawKeyTotal, total); return data;
        }

        ///// <summary>
        ///// Lấy thông tin MN_Employee theo BoPhanID
        ///// </summary>
        ///// <returns>Kết quả thực hiện</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<MN_EmployeeModel> GetByBoPhanID(int ID)
        {
            if (ID < 0) return null;
            var rawKey = string.Concat("Get_Employee_ByBoPhanID_", ID);
            var data = GetCacheItem(rawKey) as List<MN_EmployeeModel>;
            if (data != null) return data;
            data = Api.GetByBoPhanID(ID);
            AddCacheItem(rawKey, data); return data;
        }
    }
}
