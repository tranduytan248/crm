using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Web.Helpers;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Providers;

namespace Core.Cate.Biz
{
    public class RM_Employee_BusinessPlanBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_Employee_BusinessPlan_Save = "RM_Employee_BusinessPlan_Save";
        private readonly string _RM_Employee_BusinessPlan_Get = "RM_Employee_BusinessPlan_Get";
        private readonly string _RM_Employee_BusinessPlan_Getv2 = "RM_Employee_BusinessPlan_Getv2";
        private readonly string _RM_Employee_BusinessPlan_Delete = "RM_Employee_BusinessPlan_Delete";
        private readonly string _RM_Employee_BusinessPlan_GetByID = "RM_Employee_BusinessPlan_GetByID";
        private readonly string _Import_RM_Employee_BusinessPlan_ClearData = "Import_RM_Employee_BusinessPlan_ClearData";
        private readonly string _Import_RM_Employee_BusinessPlan_SaveData = "Import_RM_Employee_BusinessPlan_SaveData";
        private readonly string _Import_RM_Employee_BusinessPlan_GetData = "Import_RM_Employee_BusinessPlan_GetData";
        private readonly string _Import_RM_Employee_BusinessPlan_ValidData = "Import_RM_Employee_BusinessPlan_ValidData";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_Employee_BusinessPlan</returns>
        public List<RM_Employee_BusinessPlanModel> LoadList(int businessPlanID, out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_Employee_BusinessPlanModel>(_RM_Employee_BusinessPlan_Get,
                DATA_PROVIDER_NAME,
                businessPlanID,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_Employee_BusinessPlan
        /// </summary>
        /// <returns>Danh sách RM_Employee_BusinessPlan</returns>
        public List<RM_Employee_BusinessPlanModel> GetAll(int businessPlanID)
        {
            int total;
            var list = LoadList(businessPlanID, out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_Employee_BusinessPlan theo ID
        /// </summary>
        /// <returns>Danh sách RM_Employee_BusinessPlan</returns>
        public RM_Employee_BusinessPlanModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_Employee_BusinessPlanModel>(_RM_Employee_BusinessPlan_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_Employee_BusinessPlan theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_Employee_BusinessPlanModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Employee_BusinessPlan_Delete, DATA_PROVIDER_NAME, model.EmployeeBusinessPlanID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_Employee_BusinessPlan theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_Employee_BusinessPlanModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Employee_BusinessPlan_Save, DATA_PROVIDER_NAME
                , model.EmployeeBusinessPlanID
                , model.BoPhan_ID
                , model.EmployeeID
                , model.BusinessPlanID
                , model.RevenueTarget
                , model.ResponsibilityRevenue
                , model.Note
               , username);
            return result.GetValueOrDefault(0);
        }

        public List<RM_Employee_BusinessPlanModel> LoadData(int year, int? boPhanId, out int total)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_Employee_BusinessPlanModel>(_RM_Employee_BusinessPlan_Getv2, DATA_PROVIDER_NAME, year, boPhanId);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Validate thông tin import
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public List<Import_Employee_BusinessPlanViewModel> ValidData(DataTable db, int businessPlanId, string key, string username)
        {
            var list = new List<Import_Employee_BusinessPlanViewModel>();
            var data = AppProcessor.ProcedureProvider.ExecuteProcedure(_Import_RM_Employee_BusinessPlan_ValidData, false, DATA_PROVIDER_NAME,
                db, businessPlanId, key, username);
            if (data != null) list = ModelProvider.CreateListFromTable<Import_Employee_BusinessPlanViewModel>(data);
            return list;
        }

        /// <summary>
        /// Lưu dữ liệu import
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int SaveData(string key, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_Import_RM_Employee_BusinessPlan_SaveData, DATA_PROVIDER_NAME, key, username);
            return result.GetValueOrDefault(0);
        }

        public List<Import_Employee_BusinessPlanViewModel> GetData(string key)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<Import_Employee_BusinessPlanViewModel>(_Import_RM_Employee_BusinessPlan_GetData,
                DATA_PROVIDER_NAME, key);
            return data;
        }

        /// <summary>
        /// Xóa dữ liệu import từ bảng tạm
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Clear(string key)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_Import_RM_Employee_BusinessPlan_ClearData, DATA_PROVIDER_NAME, key);
            return result.GetValueOrDefault(0);
        }
    }
}
