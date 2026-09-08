using System.Collections.Generic;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    /// <summary>
    /// Xử lý truy vấn và cập nhật dữ liệu người liên quan của kế hoạch cơ hội kinh doanh.
    /// </summary>
    public class RM_OpportunityPlanRelatedPersonBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _saveMulti = "RM_OpportunityPlanRelatedPerson_SaveMulti";
        private readonly string _getByPlanID = "RM_OpportunityPlanRelatedPerson_GetByOpportunityPlanID";
        private readonly string _delete = "RM_OpportunityPlanRelatedPerson_Delete";

        /// <summary>
        /// Lấy danh sách người liên quan theo kế hoạch và điều kiện tìm kiếm.
        /// </summary>
        public List<RM_OpportunityPlanRelatedPersonModel> GetByOpportunityPlanID(int planID, out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "0", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_OpportunityPlanRelatedPersonModel>(
                _getByPlanID, DATA_PROVIDER_NAME,
                search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize, planID);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data[0]?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Xóa một người liên quan khỏi kế hoạch.
        /// </summary>
        public int Delete(int id, string userName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_delete, DATA_PROVIDER_NAME, id, userName);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Lưu nhiều người liên quan cho một kế hoạch.
        /// </summary>
        public int SaveMulti(RM_OpportunityPlanRelatedPersonFormModel model, string savedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_saveMulti, DATA_PROVIDER_NAME,
                model.Usernames, model.OpportunityPlanID, savedBy);
            return result.GetValueOrDefault(0);
        }
    }
}
