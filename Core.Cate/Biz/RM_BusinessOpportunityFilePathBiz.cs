using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_BusinessOpportunityFilePathBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _rm_BusinessOpportunityfilepath_Save = "RM_BusinessOpportunityFilePath_Save";
        private readonly string _rm_BusinessOpportunityfilepath_Get = "RM_BusinessOpportunityFilePath_Get";
        private readonly string _rm_BusinessOpportunityfilepath_Delete = "RM_BusinessOpportunityFilePath_Delete";
        private readonly string _rm_BusinessOpportunityfilepath_GetByID = "RM_BusinessOpportunityFilePath_GetByID";
        private readonly string _rm_BusinessOpportunityfilepath_GetByBOID = "RM_BusinessOpportunityFilePath_GetByBOID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_BusinessOpportunityFilePath</returns>
        public List<RM_BusinessOpportunityFilePathModel> LoadList(RM_BusinessOpportunityFilePathSearchModel model, out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "1", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_BusinessOpportunityFilePathModel>(_rm_BusinessOpportunityfilepath_Get,
                DATA_PROVIDER_NAME, model.BusinessOpportunityID, model.BusinessOpportunityID, search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_BusinessOpportunityFilePath
        /// </summary>
        /// <returns>Danh sách RM_BusinessOpportunityFilePath</returns>
        public List<RM_BusinessOpportunityFilePathModel> GetAll()
        {
            int total;
            var model = new RM_BusinessOpportunityFilePathSearchModel();
            var list = LoadList(model, out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_BusinessOpportunityFilePath theo ID
        /// </summary>
        /// <returns>Danh sách RM_BusinessOpportunityFilePath</returns>
        public RM_BusinessOpportunityFilePathModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_BusinessOpportunityFilePathModel>(_rm_BusinessOpportunityfilepath_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_BusinessOpportunityFilePath theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(int ID, string UserName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_BusinessOpportunityfilepath_Delete, DATA_PROVIDER_NAME, ID, UserName);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_BusinessOpportunityFilePath theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_BusinessOpportunityFilePathModel model, string savedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_BusinessOpportunityfilepath_Save, DATA_PROVIDER_NAME
               , model.FilePathID
               , model.BusinessOpportunityID
               , model.FilePath
               , model.IsDeleted
            , savedBy);
            return result.GetValueOrDefault(0);
        }
        public List<RM_BusinessOpportunityFilePathModel> GetByBOID(int ID)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_BusinessOpportunityFilePathModel>(_rm_BusinessOpportunityfilepath_GetByBOID, DATA_PROVIDER_NAME, ID);
            return list;
        }
    }
}
