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
    public class ReviewReportBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _ReviewReport_Get = "RM_Review_Report_Get";

        /// <summary>
        /// Lấy danh sách Cơ hội kinh doanh và dự án chưa có Phòng Giải Pháp tham gia
        /// </summary>
        /// <returns>Danh sách RM_Customer</returns>

        public List<ReviewReportModel> LoadList(out int total, ReviewReportSearchModel searchModel, BaseSearchModel search)
        {
            search = search ?? new ReviewReportSearchModel
            {
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            searchModel = searchModel ?? new ReviewReportSearchModel();

            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<ReviewReportModel>(
                 _ReviewReport_Get,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize,
                searchModel.ReviewBatchID
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
