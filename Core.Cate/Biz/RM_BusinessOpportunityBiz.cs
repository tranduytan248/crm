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
    public class RM_BusinessOpportunityBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_BusinessOpportunity_Get = "RM_BusinessOpportunity_Get";
        private readonly string _RM_BusinessOpportunity_GetById = "RM_BusinessOpportunity_GetById";
        private readonly string _RM_BusinessOpportunity_Delete = "RM_BusinessOpportunity_Delete";
        private readonly string _RM_BusinessOpportunity_Save = "RM_BusinessOpportunity_Save";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_BusinessOpportunity</returns>
        public List<RM_BusinessOpportunityModel> LoadList(out int total, RM_BusinessOpportunitySearchModel model, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "DESC",
                StartIndex = 0,
                PageSize = -1
            };

            // Parse FromDate / ToDate từ string "dd/MM/yyyy" sang DateTime?
            DateTime? fromDate = null;
            DateTime? toDate = null;

            if (!string.IsNullOrEmpty(model.FromDate))
            {
                if (DateTime.TryParseExact(model.FromDate, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime fd))
                    fromDate = fd;
            }

            if (!string.IsNullOrEmpty(model.ToDate))
            {
                if (DateTime.TryParseExact(model.ToDate, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime td))
                    toDate = td;
            }

            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_BusinessOpportunityModel>(_RM_BusinessOpportunity_Get,
                DATA_PROVIDER_NAME,
                model.Keyword,
                model.CustomerID,
                model.StatusID,
                model.EmployeeID,
                model.ProductServiceID,
                model.BoPhanID,
                fromDate,    
                toDate,       
                model.SuccessRateFrom,
                model.SuccessRateTo,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize,
                model.UserName);

            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_BusinessOpportunity
        /// </summary>
        /// <returns>Danh sách RM_BusinessOpportunity</returns>
        public List<RM_BusinessOpportunityModel> GetAll()
        {
            int total;
            var model = new RM_BusinessOpportunitySearchModel();
            var list = LoadList(out total, model, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_BusinessOpportunity theo ID
        /// </summary>
        /// <returns>Danh sách RM_BusinessOpportunity</returns>
        public RM_BusinessOpportunityModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_BusinessOpportunityModel>(_RM_BusinessOpportunity_GetById, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_BusinessOpportunity theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_BusinessOpportunityModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_BusinessOpportunity_Delete, DATA_PROVIDER_NAME, model.BusinessOpportunityID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Thêm mới Cơ hội kinh doanh
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_BusinessOpportunityModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_BusinessOpportunity_Save, DATA_PROVIDER_NAME,
                model.BusinessOpportunityID,
                model.OpportunityName,
                model.CustomerID,
                model.ContactPerson_ID,
                model.ExchangeDate,
                model.SalesStageID,
                model.ClosingProbability,
                model.ExpectedValue,
                model.StatusID,
                model.Description,
                model.FileAttach,
                model.ProductServiceIDs,
                username);
            return result.GetValueOrDefault(0);
        }
    }
}
