using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_TaskTypeBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_TaskType_GetAll = "RM_TaskType_GetAll";

        /// <summary>
        /// Lấy toàn bộ danh sách loại công việc còn hiệu lực.
        /// </summary>
        public List<RM_TaskTypeModel> GetAll()
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_TaskTypeModel>(
                _RM_TaskType_GetAll,
                DATA_PROVIDER_NAME);
        }
    }
}
