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
    public class RM_BusinessPlanBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_BusinessPlan_Save = "RM_BusinessPlan_Save";
        private readonly string _RM_BusinessPlan_Get = "RM_BusinessPlan_Get";
        private readonly string _RM_BusinessPlan_Delete = "RM_BusinessPlan_Delete";
        private readonly string _RM_BusinessPlan_GetByID = "RM_BusinessPlan_GetByID";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_BusinessPlan</returns>
        public List<RM_BusinessPlanModel> LoadList(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_BusinessPlanModel>(_RM_BusinessPlan_Get,
                DATA_PROVIDER_NAME,
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
        /// Lấy toàn bộ danh sách RM_BusinessPlan
        /// </summary>
        /// <returns>Danh sách RM_BusinessPlan</returns>
        public List<RM_BusinessPlanModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_BusinessPlan theo ID
        /// </summary>
        /// <returns>Danh sách RM_BusinessPlan</returns>
        public RM_BusinessPlanModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_BusinessPlanModel>(_RM_BusinessPlan_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_BusinessPlan theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_BusinessPlanModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_BusinessPlan_Delete, DATA_PROVIDER_NAME, model.BusinessPlanID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_BusinessPlan theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_BusinessPlanModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_BusinessPlan_Save, DATA_PROVIDER_NAME,
                model.BusinessPlanID,
                model.BusinessPlanName,
                model.PlanYear,
                model.DecisionNo,
                model.DecisionDate,
                model.Note,
                model.FileAttach,
                username);
            return result.GetValueOrDefault(0);
        }
    }
}
