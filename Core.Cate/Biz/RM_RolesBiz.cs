using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_RolesBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _rm_roles_Save = "RM_Roles_Save";
        private readonly string _rm_roles_Get = "RM_Roles_Get";
        private readonly string _rm_roles_Delete = "RM_Roles_Delete";
        private readonly string _rm_roles_GetByID = "RM_Roles_GetByID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_Roles</returns>
        public List<RM_RolesModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "1", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_RolesModel>(_rm_roles_Get,
                DATA_PROVIDER_NAME, search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_Roles
        /// </summary>
        /// <returns>Danh sách RM_Roles</returns>
        public List<RM_RolesModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_Roles theo ID
        /// </summary>
        /// <returns>Danh sách RM_Roles</returns>
        public RM_RolesModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_RolesModel>(_rm_roles_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_Roles theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(int ID, string UserName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_roles_Delete, DATA_PROVIDER_NAME, ID, UserName);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_Roles theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_RolesModel model, string savedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_roles_Save, DATA_PROVIDER_NAME
               , model.RoleID
               , model.RoleName
               , model.CreatedBy
               , model.CreatedDate
               , model.UpdatedBy
               , model.UpdatedDate
               , model.IsDeleted
            , savedBy);
            return result.GetValueOrDefault(0);
        }

    }
}
