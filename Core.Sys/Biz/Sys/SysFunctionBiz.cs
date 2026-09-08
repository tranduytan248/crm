using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Core.Sys.Biz.Sys
{
    public class SysFunctionBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _sysFunctionDelete = "Sys_Function_Delete";
        private readonly string _sysFunctionGet = "Sys_Function_Get";
        private readonly string _sysFunctionGetAction = "Sys_Function_GetAction";
        private readonly string _sysFunctionGetById = "Sys_Function_GetById";
        private readonly string _sysFunctionRegister = "Sys_Function_Register";
        private readonly string _sysFunctionSave = "Sys_Function_Save";

        private List<SysFunctionModel> Get(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listFunctions = AppProcessor.ProcedureProvider.ExecuteTypedList<SysFunctionModel>(_sysFunctionGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listFunctions != null && listFunctions.Count > 0)
                total = int.Parse(listFunctions.First()?.TotalRow.ToString() ?? "0");
            return listFunctions;
        }

        private SysFunctionModel LoadDetail(int functionId)
        {
            var dataFunction =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysFunctionModel>(_sysFunctionGetById,
                    DATA_PROVIDER_NAME, functionId);
            return dataFunction;
        }

        public bool Delete(SysFunctionModel model)
        {
            var result =
                AppProcessor.ProcedureProvider.Execute(_sysFunctionDelete, DATA_PROVIDER_NAME, model.FunctionId);
            return result == model.FunctionId;
        }

        public List<SysFunctionModel> GetAll()
        {
            var listFunctions = Get(out _, null);
            return listFunctions;
        }

        public SysFunctionModel GetById(int functionId)
        {
            var function = LoadDetail(functionId);
            return function;
        }

        public List<SysFunctionModel> GetList(out int total, BaseSearchModel search = null)
        {
            var listFunctions = Get(out total, search);
            return listFunctions;
        }

        public int Save(SysFunctionModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysFunctionSave, DATA_PROVIDER_NAME,
                model.FunctionId,
                model.ModuleId,
                model.Area,
                model.Name,
                model.Description,
                UtilString.SplitToTable(model.SelectedActions, new[] { ',' })
            );

            return result.GetValueOrDefault(0);
        }

        public int Register(DataTable data)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysFunctionRegister, DATA_PROVIDER_NAME, data);

            return result.GetValueOrDefault(0);
        }

        public string GetActions(int functionId)
        {
            var dataActions =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysFunctionActionModel>(_sysFunctionGetAction,
                    DATA_PROVIDER_NAME,
                    functionId);
            if (dataActions == null || dataActions.Count <= 0) return string.Empty;
            var sActions = dataActions.Select(a => a.Action).ToList();

            return string.Join(",", sActions.ToArray());
        }
    }
}