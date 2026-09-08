using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysPermissionBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _sysPermissionGet = "Sys_Permission_Get";
        private readonly string _sysPermissionGetByRoleId = "Sys_Permission_GetByRoleId";
        private readonly string _sysPermissionIsAllow = "Sys_Permission_IsAllow";
        private readonly string _sysRoleSavePermission = "Sys_Permission_Save";

        private List<SysPermissionModel> Get(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listPermissions = AppProcessor.ProcedureProvider.ExecuteTypedList<SysPermissionModel>(_sysPermissionGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listPermissions != null && listPermissions.Count > 0)
                total = int.Parse(listPermissions.First()?.TotalRow.ToString() ?? "0");
            return listPermissions;
        }

        public List<SysPermissionModel> GetByRoleId(int groupId)
        {
            var permissions =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysPermissionModel>(_sysPermissionGetByRoleId,
                    DATA_PROVIDER_NAME, groupId);
            return permissions;
        }

        public bool Save(int groupId, DataTable permission, string separated)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysRoleSavePermission, DATA_PROVIDER_NAME,
                groupId,
                permission,
                separated);

            return result > 0;
        }

        public bool IsAllow(string userName, string areaName, string controllerName, string actionName)
        {
            var dataPermissions = AppProcessor.ProcedureProvider.ExecuteTypedList<SysPermissionModel>(
                _sysPermissionIsAllow, DATA_PROVIDER_NAME, userName,
                areaName, controllerName, actionName);

            return dataPermissions != null && dataPermissions.Count > 0;
        }
    }
}