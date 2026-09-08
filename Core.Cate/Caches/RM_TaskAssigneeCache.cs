using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Cate.Biz;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Utils;
using System.ComponentModel;
using Core.Cate.Models;



namespace Core.Cate.Caches
{
    public class RM_TaskAssigneeCache : CacheLayer
    {
        private RM_TaskAssigneeBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_TaskAssigneeCache", "CENIT.APP.Cache" };
        private RM_TaskAssigneeBiz Api => _api ?? (_api = new RM_TaskAssigneeBiz());

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_TaskAssigneeModel> GetByTaskManagementID(int taskManagementID)
        {
            var key = $"GetRM_TaskAssigneeByTaskManagementID_{taskManagementID}";
            var data = GetCacheItem(key) as List<RM_TaskAssigneeModel>;
            if (data != null) return data;
            data = Api.GetByTaskManagementID(taskManagementID);
            AddCacheItem(key, data);
            return data;
        }

        /// <summary>
        /// Lưu hoặc cập nhật thông tin quản lý người đảm nhận và làm mới cache liên quan.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(int taskManagementID, string employeeIds, string username)
        {
            var result = Api.Save(taskManagementID, employeeIds, username);
            if (result > 0) InvalidateCache();
            return result;
        }
    }
}
