using System;
using System.Collections.Generic;
using Core.Cate.Models;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    /// <summary>
    /// Xử lý truy vấn và cập nhật dữ liệu nhắc lịch của kế hoạch cơ hội kinh doanh.
    /// </summary>
    public class RM_OpportunityPlanReminderBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";

        private readonly string _RM_OpportunityPlanReminder_GetPending = "RM_OpportunityPlanReminder_GetPending";
        private readonly string _RM_OpportunityPlanReminder_MarkSent = "RM_OpportunityPlanReminder_MarkSent";
        private readonly string _RM_OpportunityPlanReminder_Seed = "RM_OpportunityPlanReminder_Seed";
        private readonly string _RM_OpportunityPlanReminder_Update = "RM_OpportunityPlanReminder_Update";
        private readonly string _RM_OpportunityPlanReminder_DeleteForPlan = "RM_OpportunityPlanReminder_DeleteForPlan";

        /// <summary>
        /// Lấy danh sách reminder tới hạn để background job xử lý gửi mail.
        /// Điều kiện:
        ///   - SendStatus = Pending
        ///   - ReminderTime &lt;= thời điểm hiện tại
        ///   - IsDeleted = 0
        /// </summary>
        public List<RM_OpportunityPlanReminderModel> GetPending()
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_OpportunityPlanReminderModel>(
                _RM_OpportunityPlanReminder_GetPending,
                DATA_PROVIDER_NAME
            );
            return data;
        }

        /// <summary>
        /// Cập nhật reminder sang trạng thái Sent sau khi gửi mail thành công.
        /// </summary>
        /// <param name="id">Id reminder</param>
        /// <param name="updatedBy">Username cập nhật, thường là system</param>
        public int MarkSent(int id, string updatedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_OpportunityPlanReminder_MarkSent,
                DATA_PROVIDER_NAME,
                id,
                updatedBy
            );
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Tạo 2 reminder cho kế hoạch mới:
        ///   - OneDayBefore    : ReminderTime = WorkingDate - 1 ngày
        ///   - ThirtyMinBefore : ReminderTime = WorkingDate - 30 phút
        /// Chỉ tạo nếu thời gian kế hoạch còn đủ xa so với hiện tại.
        /// </summary>
        /// <param name="opportunityPlanId">Id kế hoạch</param>
        /// <param name="workingDate">Thời gian diễn ra kế hoạch</param>
        /// <param name="createdBy">Username người tạo</param>
        public int SeedForPlan(int opportunityPlanId, DateTime workingDate, string createdBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_OpportunityPlanReminder_Seed,
                DATA_PROVIDER_NAME,
                opportunityPlanId,
                workingDate,
                createdBy
            );
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật lại ReminderTime và reset SendStatus = 'Pending' khi WorkingDate thay đổi.
        /// Nếu ReminderTime mới đã qua thì giữ nguyên SendStatus cũ, không gửi lại.
        /// </summary>
        /// <param name="opportunityPlanId">Id kế hoạch vừa cập nhật</param>
        /// <param name="workingDate">WorkingDate mới của kế hoạch</param>
        /// <param name="updatedBy">Username người cập nhật</param>
        public int UpdateForPlan(int opportunityPlanId, DateTime workingDate, string updatedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_OpportunityPlanReminder_Update,
                DATA_PROVIDER_NAME,
                opportunityPlanId,
                workingDate,
                updatedBy
            );
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Xóa mềm toàn bộ reminder của kế hoạch khi kế hoạch bị xóa (IsDeleted = true).
        /// </summary>
        /// <param name="opportunityPlanId">Id kế hoạch bị xóa</param>
        /// <param name="updatedBy">Username người thực hiện xóa</param>
        public int DeleteForPlan(int opportunityPlanId, string updatedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_OpportunityPlanReminder_DeleteForPlan,
                DATA_PROVIDER_NAME,
                opportunityPlanId,
                updatedBy
            );
            return result.GetValueOrDefault(0);
        }
    }
}
