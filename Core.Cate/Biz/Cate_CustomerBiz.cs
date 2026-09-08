using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web.Helpers;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class Cate_CustomerBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _Cate_Customer_Save = "Cate_Customer_Save";
        private readonly string _Cate_Customer_Get = "Cate_Customer_Get";
        private readonly string _Cate_Customer_Delete = "Cate_Customer_Delete";
        private readonly string _KWC_AccountMobileModel_GetByID = "KWC_AccountMobile_GetByID";
        private readonly string _Cate_Customer_GetByUsername = "Cate_Customer_GetByUsername";
        private readonly string _Cate_Customer_ResetPassword = "Cate_Customer_ResetPassword";


        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách Cate_Customer</returns>
        public List<Cate_CustomerModel> LoadList(out int total, Cate_CustomerSearchModel model, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<Cate_CustomerModel>(_Cate_Customer_Get,
                DATA_PROVIDER_NAME,
                model.TuKhoa,
                model.TuNgay,
                model.DenNgay,
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
        /// Lấy toàn bộ danh sách Cate_Customer
        /// </summary>
        /// <returns>Danh sách Cate_Customer</returns>
        public List<Cate_CustomerModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách Cate_Customer theo ID
        /// </summary>
        /// <returns>Danh sách Cate_Customer</returns>
        public KWC_AccountMobileModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<KWC_AccountMobileModel>(_KWC_AccountMobileModel_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Lấy danh sách Cate_Customer theo ID
        /// </summary>
        /// <returns>Danh sách Cate_Customer</returns>
        public Cate_CustomerModel LoadDetail(string username)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<Cate_CustomerModel>(_Cate_Customer_GetByUsername, DATA_PROVIDER_NAME, username);
            return data;
        }


        /// <summary>
        /// Xóa danh sách Cate_Customer theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(Cate_CustomerModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_Cate_Customer_Delete, DATA_PROVIDER_NAME, model.CustomerId, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách Cate_Customer theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(Cate_CustomerModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_Cate_Customer_Save, DATA_PROVIDER_NAME
                , model.CustomerId
                , model.FullName
                , model.Email
                , model.Phone
                , model.Address
                , model.DateOfBirth
                , model.Password
                , model.Salt
                , model.IsActive
               , username);
            return result.GetValueOrDefault(0);
        }
    }
}
