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
    public class Cate_ProductServiceBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _Cate_ProductService_Get = "Cate_ProductService_Get";
        private readonly string _RM_ProductService_GetAll = "RM_ProductService_GetAll";
        private readonly string _Cate_ProductService_GetByID = "Cate_ProductService_GetByID";
        private readonly string _Cate_ProductService_Save = "Cate_ProductService_Save";
        private readonly string _Cate_ProductService_Delete = "Cate_ProductService_Delete";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách Cate_ProductService</returns>
        public List<Cate_ProductServiceModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<Cate_ProductServiceModel>(_Cate_ProductService_Get,
                DATA_PROVIDER_NAME, search.Search);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách Cate_ProductService
        /// </summary>
        /// <returns>Danh sách Cate_ProductService</returns>
        public List<Cate_ProductServiceModel> GetAllChild(string Search ,string SearchFile)
        {
            //var search = new BaseSearchModel
            //{
            //    Search = model.Search,
            //    Order = "1",
            //    OrderDir = "ASC",
            //    StartIndex = 0,
            //    PageSize = -1
            //};
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<Cate_ProductServiceModel>(_RM_ProductService_GetAll,
                DATA_PROVIDER_NAME, Search, SearchFile);
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách Cate_ProductService
        /// </summary>
        /// <returns>Danh sách Cate_ProductService</returns>
        public List<Cate_ProductServiceModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách Cate_ProductService theo ID
        /// </summary>
        /// <returns>Danh sách Cate_ProductService</returns>
        public Cate_ProductServiceModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<Cate_ProductServiceModel>(_Cate_ProductService_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách Cate_ProductService theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(Cate_ProductServiceModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_Cate_ProductService_Delete, DATA_PROVIDER_NAME, model.ProductServiceID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách Cate_ProductService theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(Cate_ProductServiceModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_Cate_ProductService_Save, DATA_PROVIDER_NAME
               , model.ProductServiceID
               , model.GroupServiceID
               , model.ParentProductID
               , model.NameProduct
               , model.CodeProduct
               , model.ShortNameProduct
               , model.FileAttach
               , model.Note
               , username
               , model.IsActived);
            return result.GetValueOrDefault(0);
        }
    }
}
