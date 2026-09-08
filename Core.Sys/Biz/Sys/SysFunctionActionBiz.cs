using System.Collections.Generic;
using System.Linq;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysFunctionActionBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _sysFunctionActionGet = "Sys_FunctionAction_GetAll";

        private List<SysFunctionActionModel> Get(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listFunctionActions = AppProcessor.ProcedureProvider.ExecuteTypedList<SysFunctionActionModel>(
                _sysFunctionActionGet, DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listFunctionActions != null && listFunctionActions.Count > 0)
                total = int.Parse(listFunctionActions.First()?.TotalRow.ToString() ?? "0");
            return listFunctionActions;
        }

        public List<SysFunctionActionModel> GetAll()
        {
            var listFunctionActions = Get(out _, null);
            return listFunctionActions;
        }
    }
}