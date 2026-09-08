using Core.Cate.Models;
using System.Collections.Generic;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    /// <summary>
    /// Xử lý truy vấn và cập nhật dữ liệu kế hoạch cơ hội kinh doanh.
    /// </summary>
    public class RM_OpportunityPlanBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";

        private readonly string _RM_OpportunityPlan_GetByOpportunityId = "RM_OpportunityPlan_GetByOpportunityId";
        private readonly string _RM_OpportunityPlan_GetById = "RM_OpportunityPlan_GetById";
        private readonly string _RM_OpportunityPlan_Save = "RM_OpportunityPlan_Save";
        private readonly string _RM_OpportunityPlan_Delete = "RM_OpportunityPlan_Delete";

        /// <summary>
        /// Lấy danh sách kế hoạch theo BusinessOpportunityID
        /// </summary>
        public List<RM_OpportunityPlanModel> GetByOpportunityId(int businessOpportunityId)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_OpportunityPlanModel>(
                _RM_OpportunityPlan_GetByOpportunityId,
                DATA_PROVIDER_NAME,
                businessOpportunityId);

            return data;
        }

        /// <summary>
        /// Lấy chi tiết kế hoạch theo Id
        /// </summary>
        public RM_OpportunityPlanModel GetById(int id)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_OpportunityPlanModel>(
                _RM_OpportunityPlan_GetById,
                DATA_PROVIDER_NAME,
                id);

            return data;
        }

        /// <summary>
        /// Thêm mới / Cập nhật kế hoạch
        /// </summary>
        public int Save(RM_OpportunityPlanModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_OpportunityPlan_Save,
                DATA_PROVIDER_NAME,
                model.Id,
                model.BusinessOpportunityID,
                model.Username,
                model.PlanName,
                model.Content,
                model.WorkingDate,
                model.AddressMeeting,
                username);

            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Xóa mềm kế hoạch theo Id
        /// </summary>
        public int Delete(int id, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_OpportunityPlan_Delete,
                DATA_PROVIDER_NAME,
                id,
                username);

            return result.GetValueOrDefault(0);
        }
    }
}
