using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Cate.Models;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_TaskActivityBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_TaskActivity_GetByTaskManagementID = "RM_TaskActivity_GetByTaskManagementID";
        private readonly string _RM_TaskActivity_Save = "RM_TaskActivity_Save";

        public List<RM_TaskActivityModel> GetByTaskManagementID(int taskManagementID)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_TaskActivityModel>(
                _RM_TaskActivity_GetByTaskManagementID, DATA_PROVIDER_NAME, taskManagementID);
        }

        public int Save(RM_TaskActivityModel model, string username) 
        {
            return AppProcessor.ProcedureProvider.Execute(
                _RM_TaskActivity_Save,
                DATA_PROVIDER_NAME,
                model.ActivityID,
                model.TaskManagementID,
                model.Employee_ID,
                model.ActivityType,
                model.OldValue,
                model.NewValue,
                model.Description,
                username
            ).GetValueOrDefault(0);
        }
    }
}
