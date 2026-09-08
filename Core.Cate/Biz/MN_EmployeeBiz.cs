using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class MN_EmployeeBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _mn_employee_Save = "MN_Employee_Save";
        private readonly string _mn_employee_Get = "MN_Employee_Get";
        private readonly string _mn_employee_Delete = "MN_Employee_Delete";
        private readonly string _mn_employee_GetByID = "MN_Employee_GetByID";
        private readonly string _mn_employee_GetByBoPhanID = "MN_Employee_GetByBoPhanID";
        private readonly string _mn_employee_Sync = "MN_Employee_Sync";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách MN_Employee</returns>
        public List<MN_EmployeeModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "1", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<MN_EmployeeModel>(_mn_employee_Get,
                DATA_PROVIDER_NAME, search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách MN_Employee
        /// </summary>
        /// <returns>Danh sách MN_Employee</returns>
        public List<MN_EmployeeModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách MN_Employee theo ID
        /// </summary>
        /// <returns>Danh sách MN_Employee</returns>
        public MN_EmployeeModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<MN_EmployeeModel>(_mn_employee_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách MN_Employee theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(int ID, string UserName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_mn_employee_Delete, DATA_PROVIDER_NAME, ID, UserName);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách MN_Employee theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(MN_EmployeeModel model, string savedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_mn_employee_Save, DATA_PROVIDER_NAME
               , model.Employee_ID
               , model.Employee_Code
               , model.FullName
               , model.BoPhan_ID
               , model.ChucVu_ID
               , model.IsDeleted
            , savedBy);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Lấy danh sách MN_Employee theo BoPhanID
        /// </summary>
        /// <returns>Danh sách RM_Contracts</returns>
        public List<MN_EmployeeModel> GetByBoPhanID(int ID)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<MN_EmployeeModel>(_mn_employee_GetByBoPhanID, DATA_PROVIDER_NAME, ID);
            return list;
        }

        /// <summary>
        /// Đồng bộ thông tin user
        /// </summary>
        public int Sync(DataTable data, string savedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _mn_employee_Sync,
                DATA_PROVIDER_NAME,
                data,
                savedBy
            ).GetValueOrDefault(0);
            return result;
        }
    }
}
