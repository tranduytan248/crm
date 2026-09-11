using System.Collections.Generic;
using System.Linq;
using Core.Cate.Models;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_DigitalSalesStatusBiz
    {
        private const string Provider = "CenIT.Provider.Major";

        public List<RM_DigitalSalesStatusModel> Get(out int total, RM_DigitalSalesStatusSearchModel search)
        {
            search = search ?? new RM_DigitalSalesStatusSearchModel { Order = "0", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesStatusModel>(
                "RM_DigitalSalesStatus_Get", Provider, search.Search, search.BusinessType, search.Order,
                search.OrderDir, search.StartIndex, search.PageSize) ?? new List<RM_DigitalSalesStatusModel>();
            total = data.Count == 0 ? 0 : (int)data.First().TotalRow;
            return data;
        }

        public List<RM_DigitalSalesStatusModel> GetAll(int? businessType = null)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesStatusModel>(
                "RM_DigitalSalesStatus_GetAll", Provider, businessType) ?? new List<RM_DigitalSalesStatusModel>();
        }

        public RM_DigitalSalesStatusModel GetById(int id) =>
            AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_DigitalSalesStatusModel>("RM_DigitalSalesStatus_GetByID", Provider, id);

        public int Save(RM_DigitalSalesStatusModel model, string username) =>
            AppProcessor.ProcedureProvider.Execute("RM_DigitalSalesStatus_Save", Provider, model.StatusID,
                model.BusinessType, model.StatusCode, model.StatusName, model.Description, model.SortOrder,
                model.IsDefault, model.IsActive, username).GetValueOrDefault(0);

        public int Delete(int id, string username) =>
            AppProcessor.ProcedureProvider.Execute("RM_DigitalSalesStatus_Delete", Provider, id, username).GetValueOrDefault(0);
    }

    public class RM_DigitalSalesProcessBiz
    {
        private const string Provider = "CenIT.Provider.Major";

        public List<RM_DigitalSalesProcessModel> Get(out int total, RM_DigitalSalesProcessSearchModel search)
        {
            search = search ?? new RM_DigitalSalesProcessSearchModel { Order = "0", OrderDir = "ASC", StartIndex = 0, PageSize = -1 };
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesProcessModel>(
                "RM_DigitalSalesProcess_Get", Provider, search.Search, search.BusinessType, search.StatusID,
                search.Order, search.OrderDir, search.StartIndex, search.PageSize) ?? new List<RM_DigitalSalesProcessModel>();
            total = data.Count == 0 ? 0 : (int)data.First().TotalRow;
            return data;
        }

        public RM_DigitalSalesProcessModel GetById(int id) =>
            AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_DigitalSalesProcessModel>("RM_DigitalSalesProcess_GetByID", Provider, id);

        public int Save(RM_DigitalSalesProcessModel model, string username) =>
            AppProcessor.ProcedureProvider.Execute("RM_DigitalSalesProcess_Save", Provider, model.ProcessID,
                model.StatusID, model.ProcessCode, model.ProcessName, model.Description, model.SortOrder,
                model.IsActive, username).GetValueOrDefault(0);

        public int Delete(int id, string username) =>
            AppProcessor.ProcedureProvider.Execute("RM_DigitalSalesProcess_Delete", Provider, id, username).GetValueOrDefault(0);
    }

    public class RM_DigitalSalesProgressBiz
    {
        private const string Provider = "CenIT.Provider.Major";

        public List<RM_DigitalSalesProgressModel> GetByProcess(int processId) =>
            AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesProgressModel>(
                "RM_DigitalSalesProgress_GetByProcess", Provider, processId) ?? new List<RM_DigitalSalesProgressModel>();

        public RM_DigitalSalesProgressModel GetById(int id) =>
            AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_DigitalSalesProgressModel>("RM_DigitalSalesProgress_GetByID", Provider, id);

        public int Save(RM_DigitalSalesProgressModel model, string username) =>
            AppProcessor.ProcedureProvider.Execute("RM_DigitalSalesProgress_Save", Provider, model.ProgressID,
                model.ProcessID, model.ProgressCode, model.ProgressName, model.Description, model.SortOrder,
                model.IsActive, username).GetValueOrDefault(0);

        public int Delete(int id, string username) =>
            AppProcessor.ProcedureProvider.Execute("RM_DigitalSalesProgress_Delete", Provider, id, username).GetValueOrDefault(0);
    }
}
