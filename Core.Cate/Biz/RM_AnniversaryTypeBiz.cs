using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_AnniversaryTypeBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _rm_anniversarytype_Save = "RM_AnniversaryType_Save";
        private readonly string _rm_anniversarytype_Get = "RM_AnniversaryType_Get";
        private readonly string _rm_anniversarytype_Delete = "RM_AnniversaryType_Delete";
        private readonly string _rm_anniversarytype_GetByID = "RM_AnniversaryType_GetByID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_AnniversaryType</returns>
        public List<RM_AnniversaryTypeModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel { Search = null, Order = "1", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_AnniversaryTypeModel>(_rm_anniversarytype_Get,
                DATA_PROVIDER_NAME, search.Search, search.Order, search.OrderDir, search.StartIndex, search.PageSize);
            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_AnniversaryType
        /// </summary>
        /// <returns>Danh sách RM_AnniversaryType</returns>
        public List<RM_AnniversaryTypeModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_AnniversaryType theo ID
        /// </summary>
        /// <returns>Danh sách RM_AnniversaryType</returns>
        public RM_AnniversaryTypeModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_AnniversaryTypeModel>(_rm_anniversarytype_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_AnniversaryType theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(int ID, string UserName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_anniversarytype_Delete, DATA_PROVIDER_NAME, ID, UserName);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_AnniversaryType theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_AnniversaryTypeModel model, string savedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_rm_anniversarytype_Save, DATA_PROVIDER_NAME
               , model.AnniversaryType_ID
               , model.CodeAnniversaryType
               , model.NameAnniversaryType
               , model.IsReminder
               , model.IsDeleted
            , savedBy);
            return result.GetValueOrDefault(0);
        }

    }
}
