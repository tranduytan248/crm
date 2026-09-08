using System.Collections.Generic;
using Core.Cate.Models;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_TaskStatusBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private const string RM_TaskStatus_GetAll = "RM_TaskStatus_GetAll";

        public List<RM_TaskStatusModel> GetAll()
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_TaskStatusModel>(
                RM_TaskStatus_GetAll, DATA_PROVIDER_NAME);
        }
    }
}