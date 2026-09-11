using Core.Cate.Biz;
using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TSFramework.Libs.Models.Caching;
using TSFramework.Libs.Utils;

namespace Core.Cate.Caches
{
    public class RM_DigitalSalesCache : CacheLayer
    {
        private RM_DigitalSalesBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_DigitalSalesCache", "CENIT.APP.Cache" };
        private RM_DigitalSalesBiz Api => _api ?? (_api = new RM_DigitalSalesBiz());

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_DigitalSalesModel> LoadList(out int total, RM_DigitalSalesSearchModel model)
        {
            var rawKey = string.Concat("RM_DigitalSales_GetList_", UtilEncrypt.FromObject(model));
            var rawKeyTotal = string.Concat(rawKey, "_Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            var data = GetCacheItem(rawKey) as List<RM_DigitalSalesModel>;
            if (data != null) return data;

            data = Api.LoadList(out total, model);
            AddCacheItem(rawKey, data);
            AddCacheItem(rawKeyTotal, total);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public RM_DigitalSalesModel GetByID(int id)
        {
            if (id <= 0) return null;
            var rawKey = string.Concat("RM_DigitalSales_GetByID_", id);
            var data = GetCacheItem(rawKey) as RM_DigitalSalesModel;
            if (data != null) return data;

            data = Api.GetByID(id);
            AddCacheItem(rawKey, data);
            return data;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Save(RM_DigitalSalesModel model, string username)
        {
            var result = Api.Save(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public int Delete(int digitalSalesId, string username)
        {
            var result = Api.Delete(digitalSalesId, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public int ChangeStatus(int digitalSalesId, int newStatusId, string note, string attachmentPath, string username)
        {
            var result = Api.ChangeStatus(digitalSalesId, newStatusId, note, attachmentPath, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public List<RM_DigitalSalesProductModel> GetProductsBySalesID(int digitalSalesId)
        {
            if (digitalSalesId <= 0) return new List<RM_DigitalSalesProductModel>();
            var rawKey = string.Concat("RM_DigitalSalesProduct_GetBySalesID_", digitalSalesId);
            var data = GetCacheItem(rawKey) as List<RM_DigitalSalesProductModel>;
            if (data != null) return data;

            data = Api.GetProductsBySalesID(digitalSalesId);
            AddCacheItem(rawKey, data);
            return data;
        }

        public int SaveProduct(RM_DigitalSalesProductModel model, string username)
        {
            var result = Api.SaveProduct(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public int DeleteProduct(int salesProductId, string username)
        {
            var result = Api.DeleteProduct(salesProductId, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public List<RM_DigitalSalesTrackingModel> GetTrackingTasks(int digitalSalesId)
        {
            if (digitalSalesId <= 0) return new List<RM_DigitalSalesTrackingModel>();
            var rawKey = string.Concat("RM_DigitalSalesTracking_GetBySalesID_", digitalSalesId);
            var data = GetCacheItem(rawKey) as List<RM_DigitalSalesTrackingModel>;
            if (data != null) return data;

            data = Api.GetTrackingTasks(digitalSalesId);
            AddCacheItem(rawKey, data);
            return data;
        }

        public int UpdateTrackingStatus(int trackingId, byte status, string resultNote, string attachmentFile, int? assignedUserId, DateTime? deadline, string username)
        {
            var result = Api.UpdateTrackingStatus(trackingId, status, resultNote, attachmentFile, assignedUserId, deadline, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public int SaveTracking(RM_DigitalSalesTrackingModel model, string username)
        {
            var result = Api.SaveTracking(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public int DeleteTracking(int trackingId, string username)
        {
            var result = Api.DeleteTracking(trackingId, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public List<RM_DigitalSalesMemberModel> GetMembersBySalesID(int digitalSalesId)
        {
            if (digitalSalesId <= 0) return new List<RM_DigitalSalesMemberModel>();
            var rawKey = string.Concat("RM_DigitalSalesMember_GetBySalesID_", digitalSalesId);
            var data = GetCacheItem(rawKey) as List<RM_DigitalSalesMemberModel>;
            if (data != null) return data;

            data = Api.GetMembersBySalesID(digitalSalesId);
            AddCacheItem(rawKey, data);
            return data;
        }

        public int SaveMember(RM_DigitalSalesMemberModel model, string username)
        {
            var result = Api.SaveMember(model, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public int DeleteMember(int memberId, string username)
        {
            var result = Api.DeleteMember(memberId, username);
            if (result > 0) InvalidateCache();
            return result;
        }

        public List<RM_DigitalSalesTimelineModel> GetTimeline(int digitalSalesId)
        {
            if (digitalSalesId <= 0) return new List<RM_DigitalSalesTimelineModel>();
            var rawKey = string.Concat("RM_DigitalSales_GetTimeline_", digitalSalesId);
            var data = GetCacheItem(rawKey) as List<RM_DigitalSalesTimelineModel>;
            if (data != null) return data;

            data = Api.GetTimeline(digitalSalesId);
            AddCacheItem(rawKey, data);
            return data;
        }

        public List<RM_DigitalSalesStatusModel> GetStatusList(byte? businessType = null)
        {
            var rawKey = string.Concat("RM_DigitalSalesStatus_GetAll_", businessType);
            var data = GetCacheItem(rawKey) as List<RM_DigitalSalesStatusModel>;
            if (data != null) return data;

            data = Api.GetStatusList(businessType);
            AddCacheItem(rawKey, data);
            return data;
        }
    }
}
