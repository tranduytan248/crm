using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_ContractRemindersBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_ContractReminders_Get = "RM_ContractReminders_Get";
        private readonly string _RM_ContractReminders_GetByID = "RM_ContractReminders_GetByID";
        private readonly string _RM_ContractReminders_Save = "RM_ContractReminders_Save";
        private readonly string _RM_ContractReminders_Delete = "RM_ContractReminders_Delete";
        private readonly string _Job_ContractPaymentReminder = "Job_ContractPaymentReminder";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_ContractReminders</returns>
        /// 
        public List<RM_ContractRemindersModel> LoadList(out int total, int ContractID, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_ContractRemindersModel>(_RM_ContractReminders_Get,
                DATA_PROVIDER_NAME,
                ContractID,
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
        /// Lấy toàn bộ danh sách RM_ContractReminders
        /// </summary>
        /// <returns>Danh sách RM_ContractReminders</returns>
        public List<RM_ContractRemindersModel> GetAll(int ContractID)
        {
            int total;
            var list = LoadList(out total, ContractID, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_ContractReminders theo ID
        /// </summary>
        /// <returns>Danh sách RM_ContractReminders</returns>
        public RM_ContractRemindersModel LoadDetail(string ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_ContractRemindersModel>(_RM_ContractReminders_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_ContractReminders theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_ContractRemindersModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ContractReminders_Delete, DATA_PROVIDER_NAME, model.ReminderId, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_ContractReminders theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_ContractRemindersModel model, DataTable dt, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_ContractReminders_Save, DATA_PROVIDER_NAME,
                model.ReminderId,
                model.ContractID,
                model.ReminderDate,
                model.Title
                );
            return result.GetValueOrDefault(0);
        }

        public List<Job_ContractPaymentReminderModel> ContractPaymentReminder()
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<Job_ContractPaymentReminderModel>(_Job_ContractPaymentReminder, DATA_PROVIDER_NAME);
            return data;
        }
    }
}
