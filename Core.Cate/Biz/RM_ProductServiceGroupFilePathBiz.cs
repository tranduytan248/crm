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
    public class RM_ProductServiceGroupFilePathBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_ProductServiceGroupFilePath_Save = "RM_ProductServiceGroupFilePath_Save";
        private readonly string _RM_ProductServiceGroupFilePath_Get = "RM_ProductServiceGroupFilePath_Get";
        private readonly string _RM_ProductServiceGroupFilePath_Delete = "RM_ProductServiceGroupFilePath_Delete";
        private readonly string _RM_ProductServiceGroupFilePath_GetByID = "RM_ProductServiceGroupFilePath_GetByID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_ProductServiceGroupFilePath</returns>
        public List<RM_ProductServiceGroupFilePathModel> LoadList(out int total, Cate_ProductServiceModel model, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProductServiceGroupFilePathModel>(_RM_ProductServiceGroupFilePath_Get,
                DATA_PROVIDER_NAME,
                model.ProductServiceID,
                model.Search,
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
        /// Lấy toàn bộ danh sách RM_ProductServiceGroupFilePath
        /// </summary>
        /// <returns>Danh sách RM_ProductServiceGroupFilePath</returns>
        public List<RM_ProductServiceGroupFilePathModel> GetAll(int ProductServiceID)
        {
            int total;
            var list = LoadList(out total, new Cate_ProductServiceModel { ProductServiceID = ProductServiceID }, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_ProductServiceGroupFilePath theo ID
        /// </summary>
        /// <returns>Danh sách RM_ProductServiceGroupFilePath</returns>
        public RM_ProductServiceGroupFilePathModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ProductServiceGroupFilePathModel>(_RM_ProductServiceGroupFilePath_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }


        /// <summary>
        /// Xóa danh sách RM_ProductServiceGroupFilePath theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_ProductServiceGroupFilePathModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ProductServiceGroupFilePath_Delete, DATA_PROVIDER_NAME, model.GroupFilePathID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_ProductServiceGroupFilePath theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_ProductServiceGroupFilePathModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ProductServiceGroupFilePath_Save, DATA_PROVIDER_NAME
                , model.GroupFilePathID
                , model.ProductServiceID
                , model.Title
                , model.Position
                , model.Note
               , username
               , model.ListFiles);
            return result.GetValueOrDefault(0);
        }
    }
}
