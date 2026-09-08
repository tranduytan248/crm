using Core.Cate.Biz;
using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TSFramework.Libs.Models.Caching;

namespace Core.Cate.Caches
{
    /// <summary>
    /// Cache dữ liệu nhắc lịch của kế hoạch cơ hội kinh doanh.
    /// </summary>
    public class RM_OpportunityPlanReminderCache : CacheLayer
    {
        private RM_OpportunityPlanReminderBiz _api;
        protected override string[] MasterCacheKeyArray => new[] { "RM_OpportunityPlanReminderCache", "CENIT.APP.Cache" };

        private RM_OpportunityPlanReminderBiz Api => _api ?? (_api = new RM_OpportunityPlanReminderBiz());

        /// <summary>
        /// Lấy danh sách reminder có SendStatus = 'Pending' và ReminderTime <= Now.
        /// Dùng cho background job gửi mail nhắc lịch.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RM_OpportunityPlanReminderModel> GetPending()
        {
            return Api.GetPending();
        }

        /// <summary>
        /// Cập nhật reminder sang trạng thái Sent sau khi gửi mail thành công.
        /// </summary>
        /// <param name="id">Id của reminder cần cập nhật</param>
        /// <param name="updatedBy">Username người cập nhật (thường là "system")</param>
        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int MarkSent(int id, string updatedBy)
        {
            var result = Api.MarkSent(id, updatedBy);
            if (result > 0) InvalidateCache();
            return result;
        }

        /// <summary>
        /// Tạo 2 bản ghi reminder khi kế hoạch mới được tạo:
        ///   - OneDayBefore    : ReminderTime = WorkingDate - 1 ngày
        ///   - ThirtyMinBefore : ReminderTime = WorkingDate - 30 phút
        /// Chỉ tạo nếu WorkingDate còn đủ xa so với hiện tại.
        /// </summary>
        /// <param name="opportunityPlanId">Id kế hoạch vừa tạo</param>
        /// <param name="workingDate">Thời gian diễn ra kế hoạch</param>
        /// <param name="createdBy">Username người tạo kế hoạch</param>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int SeedForPlan(int opportunityPlanId, DateTime workingDate, string createdBy)
        {
            var result = Api.SeedForPlan(opportunityPlanId, workingDate, createdBy);
            if (result > 0) InvalidateCache();
            return result;
        }

        /// <summary>
        /// Cập nhật lại ReminderTime và reset SendStatus = 'Pending' khi WorkingDate thay đổi.
        /// Nếu ReminderTime mới đã qua thì giữ nguyên SendStatus cũ, không gửi lại.
        /// </summary>
        /// <param name="opportunityPlanId">Id kế hoạch vừa cập nhật</param>
        /// <param name="workingDate">WorkingDate mới của kế hoạch</param>
        /// <param name="updatedBy">Username người cập nhật</param>
        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int UpdateForPlan(int opportunityPlanId, DateTime workingDate, string updatedBy)
        {
            var result = Api.UpdateForPlan(opportunityPlanId, workingDate, updatedBy);
            if (result > 0) InvalidateCache();
            return result;
        }

        /// <summary>
        /// Xóa mềm toàn bộ reminder của kế hoạch khi kế hoạch bị xóa (IsDeleted = true).
        /// </summary>
        /// <param name="opportunityPlanId">Id kế hoạch bị xóa</param>
        /// <param name="updatedBy">Username người thực hiện xóa</param>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public int DeleteForPlan(int opportunityPlanId, string updatedBy)
        {
            var result = Api.DeleteForPlan(opportunityPlanId, updatedBy);
            if (result > 0) InvalidateCache();
            return result;
        }
    }
}
