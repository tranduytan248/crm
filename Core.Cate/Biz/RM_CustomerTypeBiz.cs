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
    public class RM_CustomerTypeBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_CustomerType_Save = "RM_CustomerType_Save";
        private readonly string _RM_CustomerType_Get = "RM_CustomerType_Get";
        private readonly string _RM_CustomerType_Delete = "RM_CustomerType_Delete";
        private readonly string _RM_CustomerType_GetByID = "RM_CustomerType_GetByID";


        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_CustomerType</returns>
        public List<RM_CustomerTypeModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_CustomerTypeModel>(_RM_CustomerType_Get,
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
        /// Lấy toàn bộ danh sách RM_CustomerType
        /// </summary>
        /// <returns>Danh sách RM_CustomerType</returns>
        public List<RM_CustomerTypeModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_CustomerType theo ID
        /// </summary>
        /// <returns>Danh sách RM_CustomerType</returns>
        public RM_CustomerTypeModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_CustomerTypeModel>(_RM_CustomerType_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_CustomerType theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_CustomerTypeModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_CustomerType_Delete, DATA_PROVIDER_NAME, model.CustomerTypeID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_CustomerType theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_CustomerTypeModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_CustomerType_Save, DATA_PROVIDER_NAME
                , model.CustomerTypeID
                , model.CustomerTypeCode
                , model.CustomerTypeName
               , username);
            return result.GetValueOrDefault(0);
        }
    }
}
