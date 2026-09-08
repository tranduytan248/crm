using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_BusinessPlanFilePathBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _rm_BusinessPlanfilepath_Save = "RM_BusinessPlanFilePath_Save";
        private readonly string _rm_BusinessPlanfilepath_Get = "RM_BusinessPlanFilePath_Get";
        private readonly string _rm_BusinessPlanfilepath_Delete = "RM_BusinessPlanFilePath_Delete";
        private readonly string _rm_BusinessPlanfilepath_GetByID = "RM_BusinessPlanFilePath_GetByID";
        private readonly string _rm_BusinessPlanfilepath_GetByBusinessPlanID = "RM_BusinessPlanFilePath_GetByBusinessPlanID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_BusinessPlanFilePath</returns>
        public List<RM_BusinessPlanFilePathModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "1", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_BusinessPlanFilePathModel>(_rm_BusinessPlanfilepath_Get,
                DATA_PROVIDER_NAME, search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_BusinessPlanFilePath
        /// </summary>
        /// <returns>Danh sách RM_BusinessPlanFilePath</returns>
        public List<RM_BusinessPlanFilePathModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_BusinessPlanFilePath theo ID
        /// </summary>
        /// <returns>Danh sách RM_BusinessPlanFilePath</returns>
        public RM_BusinessPlanFilePathModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_BusinessPlanFilePathModel>(_rm_BusinessPlanfilepath_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_BusinessPlanFilePath theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(int ID, string UserName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_BusinessPlanfilepath_Delete, DATA_PROVIDER_NAME, ID, UserName);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_BusinessPlanFilePath theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_BusinessPlanFilePathModel model, string savedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_BusinessPlanfilepath_Save, DATA_PROVIDER_NAME
               , model.FilePathID
               , model.BusinessPlanID
               , model.FilePath
            , savedBy);
            return result.GetValueOrDefault(0);
        }
        public List<RM_BusinessPlanFilePathModel> GetByBusinessPlanID(int ID)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_BusinessPlanFilePathModel>(_rm_BusinessPlanfilepath_GetByBusinessPlanID, DATA_PROVIDER_NAME, ID);
            return list;
        }
    }
}
