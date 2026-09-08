using Core.Cate.Models;
using Core.Sys.Models.Sys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Core.Sys.Biz.Sys
{
    public class SysInstructBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _sysInstructsGet = "Sys_Instructs_Get";
        private readonly string _sysInstructsGetByID = "Sys_Instructs_GetByID";
        private readonly string _sysInstructsSave = "Sys_Instructs_Save";
        private readonly string _sysInstructsDelete = "Sys_Instructs_Delete";

        public List<SysInstructModel> LoadList(string search = "")
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<SysInstructModel>(
                _sysInstructsGet,
                DATA_PROVIDER_NAME,
                search
            );
            return data ?? new List<SysInstructModel>();
        }
        public List<SysInstructModel> Get(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null
            };

            var listInstructs = AppProcessor.ProcedureProvider.ExecuteTypedList<SysInstructModel>(
                _sysInstructsGet,
                DATA_PROVIDER_NAME,
                search.Search
            );

            total = listInstructs?.Count ?? 0;

            return listInstructs ?? new List<SysInstructModel>();
        }

        public SysInstructModel LoadDetail(int instructId)
        {
            var dataInstruct =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysInstructModel>(_sysInstructsGetByID,
                    DATA_PROVIDER_NAME, instructId);
            return dataInstruct;
        }

        public int Save(SysInstructModel model, string username)
        {
            var instructId = AppProcessor.ProcedureProvider.Execute(_sysInstructsSave,
                DATA_PROVIDER_NAME,
                model.InstructID,
                model.InstructParentID,
                model.InstructName,
                model.Content,
                model.PositionShow,
                model.IsActive,
                username);
            return instructId.GetValueOrDefault(0);
        }

        public int Delete(SysInstructModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysInstructsDelete,
                DATA_PROVIDER_NAME, model.InstructID, username);
            return result.GetValueOrDefault(0);
        }
    }
}
