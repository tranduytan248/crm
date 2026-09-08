using Core.Cate.Models;
using System.Collections.Generic;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_RevenueAllocationBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_RevenueAllocation_GetByProjectID = "RM_RevenueAllocation_GetByProjectID";
        private readonly string _RM_RevenueAllocation_Save = "RM_RevenueAllocation_Save";

        /// <summary>
        /// Lấy danh sách thành viên + tỷ lệ phân bổ theo ProductProjectID
        /// </summary>
        public List<RM_RevenueAllocationModel> GetByProductProjectID(int productProjectId)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_RevenueAllocationModel>(
                _RM_RevenueAllocation_GetByProjectID,
                DATA_PROVIDER_NAME,
                productProjectId);
            return data;
        }

        /// <summary>
        /// Lưu tỷ lệ phân bổ (Insert nếu ID = 0, Update nếu ID > 0)
        /// Trả về: > 0 thành công | 0 thất bại
        /// </summary>
        public int Save(RM_RevenueAllocationModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_RevenueAllocation_Save,
                DATA_PROVIDER_NAME,
                model.RevenueAllocationID,
                model.RevenueReceivedID,
                model.ProjectMemberID,
                model.AllocationRate,
                username);
            return result.GetValueOrDefault(0);
        }
    }
}