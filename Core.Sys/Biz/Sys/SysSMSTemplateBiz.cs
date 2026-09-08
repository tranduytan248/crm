using System.Collections.Generic;
using System.Linq;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysSMSTemplateBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _sysSMSTemplateDelete = "Sys_SMSTemplate_Delete";
        private readonly string _sysSMSTemplateGet = "Sys_SMSTemplate_Get";
        private readonly string _sysSMSTemplateGetByID = "Sys_SMSTemplate_GetByID";
        private readonly string _sysSMSTemplateGetByCode = "Sys_SMSTemplate_GetByCode";
        private readonly string _sysSMSTemplateSave = "Sys_SMSTemplate_Save";
        private readonly string _sysSMSLogsInsert = "Sys_SMSLogs_Insert";
        

        private List<SysSMSTemplateModel> Get(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<SysSMSTemplateModel>(_sysSMSTemplateGet,
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

        private SysSMSTemplateModel LoadDetail(int id)
        {
            var data =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysSMSTemplateModel>(_sysSMSTemplateGetByID,
                    DATA_PROVIDER_NAME, id);
            return data;
        }

        public SysSMSTemplateModel GetByCode(string code)
        {
            var data =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysSMSTemplateModel>(_sysSMSTemplateGetByCode,
                    DATA_PROVIDER_NAME, code);
            return data;
        }

        public bool Delete(SysSMSTemplateModel model, string Username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysSMSTemplateDelete, DATA_PROVIDER_NAME, model.SMSTemplateID,
                Username);
            return result == model.SMSTemplateID;
        }

        public List<SysSMSTemplateModel> GetAll()
        {
            var data = Get(out _, null);
            return data;
        }

        public SysSMSTemplateModel GetById(int id)
        {
            var data = LoadDetail(id);
            return data;
        }

        public List<SysSMSTemplateModel> GetList(out int total, BaseSearchModel search = null)
        {
            var data = Get(out total, search);
            return data;
        }

        public int Save(SysSMSTemplateModel model, string userName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysSMSTemplateSave, DATA_PROVIDER_NAME,
                model.SMSTemplateID,
                model.TemplateCode,
                model.TemplateName,
                model.TemplateContent,
                model.IsActive,
                userName);

            return result.GetValueOrDefault(0);
        }

        public int InsertLog(SysSMSLogsModel model, string userName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysSMSLogsInsert, DATA_PROVIDER_NAME,
                model.PhoneNumber,
                model.TemplateCode,
                model.Content,
                model.IsSuccess,
                model.ErrorMessage,
                userName);

            return result.GetValueOrDefault(0);
        }
    }
}