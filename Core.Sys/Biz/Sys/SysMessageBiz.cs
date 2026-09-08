using System.Collections.Generic;
using System.Linq;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysMessageBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _sysMessageDelete = "Sys_Message_Delete";
        private readonly string _sysMessageGet = "Sys_Message_Get";
        private readonly string _sysMessageGetById = "Sys_Message_GetById";
        private readonly string _sysMessageGetByKey = "Sys_Message_GetByKey";
        private readonly string _sysMessageGetByLangCode = "Sys_Message_GetByLangCode";
        private readonly string _sysMessageSave = "Sys_Message_Save";

        private List<SysMessageModel> Get(string langCode)
        {
            var listMessages =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysMessageModel>(_sysMessageGetByLangCode,
                    DATA_PROVIDER_NAME, langCode);
            return listMessages;
        }

        private List<SysMessageModel> Get(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listMessages = AppProcessor.ProcedureProvider.ExecuteTypedList<SysMessageModel>(_sysMessageGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listMessages != null && listMessages.Count > 0)
                total = int.Parse(listMessages.First()?.TotalRow.ToString() ?? "0");
            return listMessages;
        }

        private SysMessageModel LoadDetail(string langCode, string labelKey)
        {
            var dataMessage =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysMessageModel>(_sysMessageGetByKey,
                    DATA_PROVIDER_NAME, langCode, labelKey);
            return dataMessage;
        }

        private SysMessageModel LoadDetail(string messageId)
        {
            var dataMessage =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysMessageModel>(_sysMessageGetById,
                    DATA_PROVIDER_NAME, messageId);
            return dataMessage;
        }

        public bool Delete(SysMessageModel model)
        {
            var result =
                AppProcessor.ProcedureProvider.Execute(_sysMessageDelete, DATA_PROVIDER_NAME, model.LangCode,
                    model.LabelKey);
            return result == 1;
        }

        public List<SysMessageModel> GetAll()
        {
            var listMessages = Get(out _, null);
            return listMessages;
        }


        public List<SysMessageModel> GetByLangCode(string langCode)
        {
            var listMessages = Get(langCode);
            return listMessages;
        }

        public SysMessageModel GetByKey(string langCode, string labelKey)
        {
            var message = LoadDetail(langCode, labelKey);
            return message;
        }

        public SysMessageModel GetById(string messageId)
        {
            var message = LoadDetail(messageId);
            return message;
        }

        public List<SysMessageModel> GetList(out int total, BaseSearchModel search = null)
        {
            var listMessages = Get(out total, search);
            return listMessages;
        }

        public int Save(SysMessageModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysMessageSave, DATA_PROVIDER_NAME,
                model.LangCode,
                model.LabelKey,
                model.Message);

            return result.GetValueOrDefault(0);
        }
    }
}