using Core.Cate.Models;
using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_Report_BusinessOpportunityBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string RM_BusinessOpportunity_Report = "RM_Report_BusinessOpportunity";
     

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        public List<RM_Report_BusinessOpportunityModel> LoadList(out int total, RM_Report_BusinessOpportunitySearchModel model, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            // Thứ tự tham số phải khớp với thứ tự khai báo của RM_Report_BusinessOpportunity_v3
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_Report_BusinessOpportunityModel>(RM_BusinessOpportunity_Report,
                  DATA_PROVIDER_NAME,
                  model.TuKhoa,
                  model.Nam,
                  model.Loai,
                  model.CustomerTypeID,
                  model.ProductServiceID,
                  model.ProjectTypeID,
                  model.StatusID,
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
        /// <returns>Danh sách RM_Status</returns>
        public List<RM_Report_BusinessOpportunityModel> GetAll(RM_Report_BusinessOpportunitySearchModel model)
        {
            int total;
            var list = LoadList(out total, model, null);
            return list;
        }

    }
}
