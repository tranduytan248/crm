using System.Collections.Generic;
using System.Linq;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_ProjectMemberBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _rm_ProjectMember_Save = "RM_ProjectMember_Save"; 
        private readonly string _rm_ProjectMember_SaveMulti = "RM_ProjectMember_SaveMulti";
        private readonly string _rm_ProjectMember_Get = "RM_ProjectMember_Get";
        private readonly string _rm_ProjectMember_GetByProductProjectID = "RM_ProjectMember_GetByProductProjectID";
        private readonly string _rm_ProjectMember_Delete = "RM_ProjectMember_Delete";
        private readonly string _rm_ProjectMember_GetByID = "RM_ProjectMember_GetByID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_ProjectMember</returns>
        public List<RM_ProjectMemberModel> LoadList(out int total,int ProjectID, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "1", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProjectMemberModel>(_rm_ProjectMember_Get,
                DATA_PROVIDER_NAME, ProjectID, search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ProjectMember
        /// </summary>
        /// <returns>Danh sách RM_ProjectMember</returns>
        public List<RM_ProjectMemberModel> GetAll(int ProjectID)
        {
            int total;
            var list = LoadList(out total, ProjectID, null);
            return list;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ProjectMember
        /// </summary>
        /// <returns>Danh sách RM_ProjectMember</returns>
        public List<RM_ProjectMemberModel> GetByProductProjectID(int ID)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProjectMemberModel>(_rm_ProjectMember_GetByProductProjectID, DATA_PROVIDER_NAME, ID);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_ProjectMember theo ID
        /// </summary>
        /// <returns>Danh sách RM_ProjectMember</returns>
        public RM_ProjectMemberModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ProjectMemberModel>(_rm_ProjectMember_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_ProjectMember theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(int ID, string UserName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_ProjectMember_Delete, DATA_PROVIDER_NAME, ID, UserName);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_ProjectMember theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_ProjectMemberModel model, string savedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_ProjectMember_Save, DATA_PROVIDER_NAME
               , model.ProjectMemberID
               , model.ProductProjectID
               , model.Employee_ID
               , model.RoleID
            , savedBy);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_ProjectMember theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int SaveMulti(RM_ProjectMemberFormModel model, string savedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_ProjectMember_SaveMulti, DATA_PROVIDER_NAME
               , model.EmployeeIDs
               , model.RoleIDs
               , model.ProductProjectID
            , savedBy);
            return result.GetValueOrDefault(0);
        }
    }
}
