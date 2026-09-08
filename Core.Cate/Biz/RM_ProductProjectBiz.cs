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
    public class RM_ProductProjectBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_ProductProject_Save = "RM_ProductProject_Save";
        private readonly string _RM_ProductProject_Convert = "RM_ProductProject_Convert";
        private readonly string _RM_ProductProject_Get = "RM_ProductProject_Get";
        private readonly string _RM_ProductProject_Delete = "RM_ProductProject_Delete";
        private readonly string _RM_ProductProject_GetByID = "RM_ProductProject_GetByID";
        private readonly string _RM_ProductProject_GetByProjectID = "RM_ProductProject_GetByProjectID";


        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_ProductProject</returns>
        public List<RM_ProductProjectModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProductProjectModel>(_RM_ProductProject_Get,
                DATA_PROVIDER_NAME,
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
        /// Lấy toàn bộ danh sách RM_ProductProject
        /// </summary>
        /// <returns>Danh sách RM_ProductProject</returns>
        public List<RM_ProductProjectModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_ProductProject theo ID
        /// </summary>
        /// <returns>Danh sách RM_ProductProject</returns>
        public RM_ProductProjectModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ProductProjectModel>(_RM_ProductProject_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Lấy danh sách RM_ProductProject theo ProjectID
        /// </summary>
        /// <returns>Danh sách RM_ProductProject</returns>
        public List<RM_ProductProjectModel> GetByProjectID(int ID)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProductProjectModel>(_RM_ProductProject_GetByProjectID, DATA_PROVIDER_NAME, ID);
            return list;
        }

        /// <summary>
        /// Xóa danh sách RM_ProductProject theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_ProductProjectModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ProductProject_Delete, DATA_PROVIDER_NAME, model.ProductProjectID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_ProductProject theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_ProductProjectModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ProductProject_Save, DATA_PROVIDER_NAME
                , model.ProductProjectID
                , model.ProjectID
                , model.ProductServiceID
                , model.ExpectedRevenue
                , model.StartDate
                , model.EndDate
               , username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_ProductProject theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Convert(RM_ProductProjectModel model, int businessOpportunityID, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ProductProject_Convert, DATA_PROVIDER_NAME
                , model.ProductProjectID
                , model.ProjectID
                , businessOpportunityID
                , model.ProductServiceID
                , model.ExpectedRevenue
                , model.StartDate
                , model.EndDate
               , username);
            return result.GetValueOrDefault(0);
        }
    }
}
