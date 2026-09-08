using System.Collections.Generic;
using System.ComponentModel;
using Core.Cate.Models;
using Core.Sys.Biz.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Sys.Caches.Sys
{
    /// <summary>
    /// Cache dữ liệu mẫu email.
    /// </summary>
    [DataObject]
    public class SysMailTemplateCache : CacheLayer
    {
        #region Declaration

        private SysMailTemplateBiz _api;

        private SysMailTemplateBiz Api => _api ?? (_api = new SysMailTemplateBiz());

        protected override string[] MasterCacheKeyArray => new[]
        {
            "SysMailTemplateCache",
            "CENIT.APP.Cache"
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Lấy danh sách mẫu email theo điều kiện tìm kiếm.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysMailTemplateModel> Get(out int total, BaseSearchModel search = null)
        {
            string objectKey = UtilEncrypt.FromObject(search);
            string rawKey = string.Concat("SysMailTemplateList-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;

            List<SysMailTemplateModel> cachedTemplates = GetCacheItem(rawKey) as List<SysMailTemplateModel>;
            int? cachedTotal = GetCacheItem(rawKeyTotal) as int?;
            if (cachedTemplates != null)
            {
                total = cachedTotal ?? cachedTemplates.Count;
                return cachedTemplates;
            }

            List<SysMailTemplateModel> templates = Api.Get(out total, search);
            AddCacheItem(rawKey, templates);
            AddCacheItem(rawKeyTotal, total);

            return templates;
        }

        /// <summary>
        /// Lấy chi tiết mẫu email theo mã dữ liệu.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysMailTemplateModel GetById(int mailTemplateId)
        {
            string rawKey = string.Concat("SysMailTemplateById-", mailTemplateId);
            if (GetCacheItem(rawKey) is SysMailTemplateModel template)
            {
                return template;
            }

            template = Api.GetById(mailTemplateId);
            AddCacheItem(rawKey, template);

            return template;
        }

        /// <summary>
        /// Lấy mẫu email đang hoạt động theo mã template.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysMailTemplateModel GetActiveByCode(string templateCode)
        {
            string rawKey = string.Concat("SysMailTemplateActiveByCode-", templateCode);
            if (GetCacheItem(rawKey) is SysMailTemplateModel template)
            {
                return template;
            }

            template = Api.GetActiveByCode(templateCode);
            AddCacheItem(rawKey, template);

            return template;
        }

        /// <summary>
        /// Lấy danh sách tham số theo mã mẫu email.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysMailTemplateParamModel> GetParamsByTemplateId(int mailTemplateId)
        {
            string rawKey = string.Concat("SysMailTemplateParamById-", mailTemplateId);
            if (GetCacheItem(rawKey) is List<SysMailTemplateParamModel> parameters)
            {
                return parameters;
            }

            parameters = Api.GetParamsByTemplateId(mailTemplateId);
            AddCacheItem(rawKey, parameters);

            return parameters;
        }

        /// <summary>
        /// Lấy danh sách tham số theo mã template.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysMailTemplateParamModel> GetParamsByTemplateCode(string templateCode)
        {
            string rawKey = string.Concat("SysMailTemplateParamByCode-", templateCode);
            if (GetCacheItem(rawKey) is List<SysMailTemplateParamModel> parameters)
            {
                return parameters;
            }

            parameters = Api.GetParamsByTemplateCode(templateCode);
            AddCacheItem(rawKey, parameters);

            return parameters;
        }

        /// <summary>
        /// Lưu thông tin mẫu email.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int Save(SysMailTemplateModel model, string updatedBy)
        {
            int result = Api.Save(model, updatedBy);
            if (result != 0)
            {
                InvalidateCache();
            }

            return result;
        }

        /// <summary>
        /// Xóa mẫu email theo mã dữ liệu.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int Delete(int mailTemplateId, string updatedBy)
        {
            int result = Api.Delete(mailTemplateId, updatedBy);
            if (result > 0)
            {
                InvalidateCache();
            }

            return result;
        }

        /// <summary>
        /// Lưu thông tin tham số của mẫu email.
        /// </summary>
        public int SaveParam(SysMailTemplateParamModel model, string updatedBy)
        {
            int result = Api.SaveParam(model, updatedBy);
            if (result != 0)
            {
                InvalidateCache();
            }

            return result;
        }

        /// <summary>
        /// Xóa toàn bộ tham số của một mẫu email.
        /// </summary>
        public int DeleteParamsByTemplateId(int mailTemplateId, string updatedBy)
        {
            int result = Api.DeleteParamsByTemplateId(mailTemplateId, updatedBy);
            if (result >= 0)
            {
                InvalidateCache();
            }

            return result;
        }

        /// <summary>
        /// Xóa toàn bộ cache liên quan đến mẫu email.
        /// </summary>
        public void ClearCache()
        {
            InvalidateCache();
        }

        #endregion
    }
}
