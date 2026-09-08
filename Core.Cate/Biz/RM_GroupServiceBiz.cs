using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_GroupServiceBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_GroupService_Save = "RM_GroupService_Save";
        private readonly string _RM_GroupService_Get = "RM_GroupService_Get";
        private readonly string _RM_GroupService_Delete = "RM_GroupService_Delete";
        private readonly string _RM_GroupService_GetByID = "RM_GroupService_GetByID";

        /// <summary>
        /// Lấy danh sách GroupService theo Search (Tree)
        /// </summary>
        public List<RM_GroupServiceModel> LoadList(string search = "")
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_GroupServiceModel>(
                _RM_GroupService_Get,
                DATA_PROVIDER_NAME,
                search
            );

            return data ?? new List<RM_GroupServiceModel>();
        }

        /// <summary>
        /// Lấy toàn bộ danh sách GroupService
        /// </summary>
        public List<RM_GroupServiceModel> GetAll()
        {
            return LoadList("");
        }

        /// <summary>
        /// Lấy chi tiết GroupService theo ID
        /// </summary>
        public RM_GroupServiceModel LoadDetail(int id)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_GroupServiceModel>(
                _RM_GroupService_GetByID,
                DATA_PROVIDER_NAME,
                id
            );

            return data;
        }

        /// <summary>
        /// Xóa GroupService
        /// </summary>
        public int Delete(RM_GroupServiceModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_GroupService_Delete,
                DATA_PROVIDER_NAME,
                model.GroupServiceID,
                username
            );

            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Lưu GroupService
        /// </summary>
        public int Save(RM_GroupServiceModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_GroupService_Save,
                DATA_PROVIDER_NAME,
                model.GroupServiceID,
                model.NameGroup,
                model.ParentGroupServiceID,
                model.IsActived,
                username
            );

            return result.GetValueOrDefault(0);
        }
    }
}
