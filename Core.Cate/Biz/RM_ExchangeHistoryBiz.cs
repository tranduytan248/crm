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
    public class RM_ExchangeHistoryBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_ExchangeHistory_Save = "RM_ExchangeHistory_Save";
        private readonly string _RM_ExchangeHistory_Get = "RM_ExchangeHistory_Get";
        private readonly string _RM_ExchangeHistory_Delete = "RM_ExchangeHistory_Delete";
        private readonly string _RM_ExchangeHistory_GetByID = "RM_ExchangeHistory_GetByID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_ExchangeHistory</returns>
        public List<RM_ExchangeHistoryModel> LoadList(RM_ExchangeHistorySearchModel model, out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ExchangeHistoryModel>(_RM_ExchangeHistory_Get,
                DATA_PROVIDER_NAME,
                model.BusinessOpportunityID,
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
        /// Lấy toàn bộ danh sách RM_ExchangeHistory
        /// </summary>
        /// <returns>Danh sách RM_ExchangeHistory</returns>
        public List<RM_ExchangeHistoryModel> GetAll()
        {
            int total;
            var model = new RM_ExchangeHistorySearchModel();
            var list = LoadList(model, out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_ExchangeHistory theo ID
        /// </summary>
        /// <returns>Danh sách RM_ExchangeHistory</returns>
        public RM_ExchangeHistoryModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ExchangeHistoryModel>(_RM_ExchangeHistory_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_ExchangeHistory theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_ExchangeHistoryModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ExchangeHistory_Delete, DATA_PROVIDER_NAME, model.ExchangeHistoryID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_ExchangeHistory theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_ExchangeHistoryModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ExchangeHistory_Save, DATA_PROVIDER_NAME,
                model.ExchangeHistoryID,
                model.ParticipatingMembers,
                model.ExchangeDate,
                model.StatusID,
                model.BusinessOpportunityID,
                model.ExchangeContent,
                model.FileAttach,
                model.ContactPerson_ID,
                username);
            return result.GetValueOrDefault(0);
        }
    }
}
