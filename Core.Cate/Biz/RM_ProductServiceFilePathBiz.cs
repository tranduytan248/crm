using System.Collections.Generic;
using System.Linq;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_ProductServiceFilePathBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        // private readonly string _rm_ProductServicefilepath_Save = "RM_ProductServiceFilePath_Save";
        private readonly string _rm_ProductServicefilepath_Get = "RM_ProductServiceFilePath_Get";
        private readonly string _rm_ProductServicefilepath_Delete = "RM_ProductServiceFilePath_Delete";
        private readonly string _rm_ProductServicefilepath_GetByID = "RM_ProductServiceFilePath_GetByID";


        //private readonly string _rm_ProductServicefilepath_GetByProductService = "RM_ProductServiceFilePath_GetByProductService";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_ProductServiceFilePath</returns>
        public List<RM_ProductServiceFilePathModel> LoadList(out int total, RM_ProductServiceGroupFilePathModel model, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "1", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ProductServiceFilePathModel>(_rm_ProductServicefilepath_Get,
                DATA_PROVIDER_NAME, model.GroupFilePathID, search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_ProductServiceFilePath
        /// </summary>
        /// <returns>Danh sách RM_ProductServiceFilePath</returns>
        public List<RM_ProductServiceFilePathModel> GetAll(int GroupFilePathID)
        {
            int total;
            var list = LoadList(out total, new RM_ProductServiceGroupFilePathModel { GroupFilePathID = GroupFilePathID }, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_ProductServiceFilePath theo ID
        /// </summary>
        /// <returns>Danh sách RM_ProductServiceFilePath</returns>
        public RM_ProductServiceFilePathModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ProductServiceFilePathModel>(_rm_ProductServicefilepath_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_ProductServiceFilePath theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(int ID, string UserName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_ProductServicefilepath_Delete, DATA_PROVIDER_NAME, ID, UserName);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_ProductServiceFilePath theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        //public int Save(RM_ProductServiceFilePathModel model, string savedBy)
        //{
        //    var result = AppProcessor.ProcedureProvider.Execute(_rm_ProductServicefilepath_Save, DATA_PROVIDER_NAME
        //       , model.FilePathID
        //       , model.ProductServiceID
        //       , model.FilePath
        //    , savedBy);
        //    return result.GetValueOrDefault(0);
        //}
    }
}
