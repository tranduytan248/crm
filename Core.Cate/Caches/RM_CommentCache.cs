using Core.Cate.Biz;
using Core.Cate.Models;
using System.ComponentModel;
using TSFramework.Libs.Models.Caching;
using System.Collections.Generic;


namespace Core.Cate.Caches
{
    public class RM_CommentCache : CacheLayer
    {
        private RM_CommentBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_CommentCache", "CENIT.APP.Cache" };
        private RM_CommentBiz Api => _api ?? (_api = new RM_CommentBiz());

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_CommentModel> GetComentByTaskManagementID(int taskManagementID)
        {
            var key = $"GetRM_CommentByTaskManagementID_{taskManagementID}";
            var data = GetCacheItem(key) as List<RM_CommentModel>;
            if (data != null) return data;
            data = Api.LoadListByTaskManagementID(taskManagementID);
            AddCacheItem(key, data);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_CommentViewByProjectModel> LoadListByProjectID(int ProjectID)
        {
            var key = $"LoadListByProjectID_{ProjectID}";
            var data = GetCacheItem(key) as List<RM_CommentViewByProjectModel>;
            if (data != null) return data;
            data = Api.LoadListByProjectID(ProjectID);
            AddCacheItem(key, data);
            return data;
        }


        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_CommentModel GetByID(int commentID)
        {
            var key = $"GetRM_CommentByID_{commentID}";
            var data = GetCacheItem(key) as RM_CommentModel;
            if (data != null) return data;
            data = Api.GetByID(commentID);
            AddCacheItem(key, data);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_CommentModel model, string username)
        {
            var result = Api.Save(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(RM_CommentModel model, string username)
        {
            var result = Api.Delete(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }
    }
}
