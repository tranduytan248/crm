using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Cate.Models;
using TSFramework.Libs.Processors;



namespace Core.Cate.Biz
{
    public class RM_TaskAssigneeBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_TaskAssignee_Save = "RM_TaskAssignee_Save";
        private readonly string _RM_TaskAssignee_GetByTaskManagementID = "RM_TaskAssignee_GetByTaskManagementID";

        public List<RM_TaskAssigneeModel> GetByTaskManagementID(int taskManagementID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_TaskAssigneeModel>(
                _RM_TaskAssignee_GetByTaskManagementID,
                DATA_PROVIDER_NAME,
                taskManagementID);

            return data;
        }

        /// <summary>
        /// Lưu hoặc cập nhật thông tin quản lý người đảm nhận theo công việc.
        /// </summary>
        public int Save(int taskManagementID, string employeeIds, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
               _RM_TaskAssignee_Save,
               DATA_PROVIDER_NAME,
               taskManagementID,
               employeeIds,
               username);

            return result.GetValueOrDefault(0);
        }
    }
}
