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
    public class OpportunityProjectWithoutPGPBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _OpportunityProjectWithoutPGP_Get = "RM_OpportunityProjectWithoutPGP_Get";

        /// <summary>
        /// Lấy danh sách Cơ hội kinh doanh và dự án chưa có Phòng Giải Pháp tham gia
        /// </summary>
        /// <returns>Danh sách RM_Customer</returns>

        public List<OpportunityProjectWithoutPGPModel> LoadList(out int total, OpportunityProjectWithoutPGSearchPModel searchModel, BaseSearchModel search)
        {
            search = search ?? new OpportunityProjectWithoutPGSearchPModel
            {
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            searchModel = searchModel ?? new OpportunityProjectWithoutPGSearchPModel();

            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<OpportunityProjectWithoutPGPModel>(
                 _OpportunityProjectWithoutPGP_Get,
                DATA_PROVIDER_NAME,
                search.Search,      // @Search
                search.Order,       // @Order
                search.OrderDir,    // @OrderDir
                search.StartIndex,  // @PageIndex
                search.PageSize,    // @PageSize
                search.Search,      // @Keyword ✅ thêm vào
                searchModel.Year,   // @Year
                searchModel.ReportType  // @ReportType
                );
            total = 0;
            if (data != null && data.Count > 0)
            {
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            }
            return data;
        }
    }
}
