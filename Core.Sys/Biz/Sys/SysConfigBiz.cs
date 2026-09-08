using System.Collections.Generic;
using System.Linq;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysConfigBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _sysConfigsDelete = "Sys_Configs_Delete";
        private readonly string _sysConfigsGet = "Sys_Configs_Get";
        private readonly string _sysConfigsGetByID = "Sys_Configs_GetByID";
        private readonly string _sysConfigsGetViaKey = "Sys_Configs_GetViaKey";
        private readonly string _sysConfigsSave = "Sys_Configs_Save";
        private readonly string _sysConfigsSaveRefFiles = "Sys_Configs_SaveRefFiles";
        

        private List<SysConfigModel> Get(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listConfigs = AppProcessor.ProcedureProvider.ExecuteTypedList<SysConfigModel>(_sysConfigsGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listConfigs != null && listConfigs.Count > 0)
                total = int.Parse(listConfigs.First()?.TotalRow.ToString() ?? "0");
            return listConfigs;
        }

        private SysConfigModel LoadDetail(int configId)
        {
            var dataConfigs =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysConfigModel>(_sysConfigsGetByID,
                    DATA_PROVIDER_NAME, configId);
            return dataConfigs;
        }

        public SysConfigModel GetViaKey(string configKey)
        {
            var dataConfig =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysConfigModel>(_sysConfigsGetViaKey,
                    DATA_PROVIDER_NAME, configKey);
            return dataConfig;
        }

        public List<SysConfigModel> GetListViaKey(string configKey)
        {
            var listConfigs = AppProcessor.ProcedureProvider.ExecuteTypedList<SysConfigModel>(_sysConfigsGetViaKey,
                DATA_PROVIDER_NAME,
                configKey);
            return listConfigs;
        }

        public bool Delete(SysConfigModel model, string Username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysConfigsDelete, DATA_PROVIDER_NAME, model.ConfigId,
                Username);
            return result == model.ConfigId;
        }

        public List<SysConfigModel> GetAll()
        {
            var listConfigs = Get(out _, null);
            return listConfigs;
        }

        public SysConfigModel GetById(int configId)
        {
            var configs = LoadDetail(configId);
            return configs;
        }

        public List<SysConfigModel> GetList(out int total, BaseSearchModel search = null)
        {
            var listConfigs = Get(out total, search);
            return listConfigs;
        }

        public int Save(SysConfigModel model, string userName)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysConfigsSave, DATA_PROVIDER_NAME,
                model.ConfigId,
                model.ConfigKey,
                model.ConfigValue,
                model.ConfigDesc,
                model.IsFile,
                userName);

            return result.GetValueOrDefault(0);
        }

        public int SaveRefFiles(SysConfigModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysConfigsSaveRefFiles, DATA_PROVIDER_NAME,
                model.ConfigId,
                model.TableRefFile,
                model.UpdatedBy);

            return result.GetValueOrDefault(0);
        }
    }
}