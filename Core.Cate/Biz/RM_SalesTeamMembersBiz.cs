using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_SalesTeamMembersBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _rm_salesteammembers_Save = "RM_SalesTeamMembers_Save"; 
        private readonly string _rm_salesteammembers_SaveMulti = "RM_SalesTeamMembers_SaveMulti";
        private readonly string _rm_salesteammembers_Get = "RM_SalesTeamMembers_Get";
        private readonly string _rm_salesteammembers_GetByBusinessOpportunityID = "RM_SalesTeamMembers_GetByBusinessOpportunityID";
        private readonly string _rm_salesteammembers_Delete = "RM_SalesTeamMembers_Delete";
        private readonly string _rm_salesteammembers_GetByID = "RM_SalesTeamMembers_GetByID";
        private readonly string _rm_salesteammembers_GetGetManagementByBOID = "RM_SalesTeamMembers_GetGetManagementByBOID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_SalesTeamMembers</returns>
        public List<RM_SalesTeamMembersModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "1", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_SalesTeamMembersModel>(_rm_salesteammembers_Get,
                DATA_PROVIDER_NAME, search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_SalesTeamMembers
        /// </summary>
        /// <returns>Danh sách RM_SalesTeamMembers</returns>
        public List<RM_SalesTeamMembersModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_SalesTeamMembers
        /// </summary>
        /// <returns>Danh sách RM_SalesTeamMembers</returns>
        public List<RM_SalesTeamMembersModel> GetByBusinessOpportunityID(int businessOpportunityID, out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "1", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_SalesTeamMembersModel>(_rm_salesteammembers_GetByBusinessOpportunityID,
                DATA_PROVIDER_NAME, search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize, businessOpportunityID);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy danh sách RM_SalesTeamMembers theo ID
        /// </summary>
        /// <returns>Danh sách RM_SalesTeamMembers</returns>
        public RM_SalesTeamMembersModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_SalesTeamMembersModel>(_rm_salesteammembers_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_SalesTeamMembers theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(int ID, string UserName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_salesteammembers_Delete, DATA_PROVIDER_NAME, ID, UserName);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_SalesTeamMembers theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_SalesTeamMembersModel model, string savedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_salesteammembers_Save, DATA_PROVIDER_NAME
               , model.MemberID
               , model.EmployeeID
               , model.RoleID
               , model.BusinessOpportunityID
            , savedBy);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_SalesTeamMembers theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int SaveMulti(RM_SalesTeamMembersFormModel model, string savedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_salesteammembers_SaveMulti, DATA_PROVIDER_NAME
               , model.EmployeeIDs
               , model.RoleIDs
               , model.BusinessOpportunityID
            , savedBy);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Lấy danh sách user quản lý SalesTeamMembers theo businessOpportunityID
        /// </summary>
        /// <returns>Danh sách RM_SalesTeamMembers</returns>
        public List<RM_SalesTeamMembersModel> GetManagementByBOID(int businessOpportunityID)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_SalesTeamMembersModel>(_rm_salesteammembers_GetGetManagementByBOID, DATA_PROVIDER_NAME, businessOpportunityID);
            return list;
        }
    }
}
