using System.Collections.Generic;
using Core.Cate.Models;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_PriorityBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_Priority_GetAll = "RM_Priority_GetAll";

        /// <summary>
        /// Lấy toàn bộ danh sách độ ưu tiên còn hiệu lực.
        /// </summary>
        public List<RM_PriorityModel> GetAll()
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_PriorityModel>(
                _RM_Priority_GetAll,
                DATA_PROVIDER_NAME);
        }
    }
}
