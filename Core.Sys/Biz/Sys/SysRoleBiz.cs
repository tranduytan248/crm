using System.Collections.Generic;
using System.Linq;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysRoleBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _sysRoleAddUser = "Sys_Role_AddUser";
        private readonly string _sysRoleAddUsers = "Sys_Role_AddUsers";
        private readonly string _sysRoleDelete = "Sys_Role_Delete";
        private readonly string _sysRoleGet = "Sys_Role_Get";
        private readonly string _sysRoleGetById = "Sys_Role_GetById";
        private readonly string _sysRoleRemoveUser = "Sys_Role_RemoveUser";
        private readonly string _sysRoleSave = "Sys_Role_Save";

        private List<SysRoleModel> Get(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listRoles = AppProcessor.ProcedureProvider.ExecuteTypedList<SysRoleModel>(_sysRoleGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listRoles != null && listRoles.Count > 0)
                total = int.Parse(listRoles.First()?.TotalRow.ToString() ?? "0");
            return listRoles;
        }

        private SysRoleModel LoadDetail(int roleId)
        {
            var dataRole =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysRoleModel>(_sysRoleGetById, DATA_PROVIDER_NAME,
                    roleId);
            return dataRole;
        }

        public bool Delete(SysRoleModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysRoleDelete, DATA_PROVIDER_NAME, model.RoleId);
            return result == model.RoleId;
        }

        public List<SysRoleModel> GetAll()
        {
            var listRoles = Get(out _, null);
            return listRoles;
        }

        public SysRoleModel GetById(int roleId)
        {
            var role = LoadDetail(roleId);
            return role;
        }

        public List<SysRoleModel> GetList(out int total, BaseSearchModel search = null)
        {
            var listRoles = Get(out total, search);
            return listRoles;
        }

        public int Save(SysRoleModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysRoleSave, DATA_PROVIDER_NAME,
                model.RoleId,
                model.Name);

            return result.GetValueOrDefault(0);
        }

        public int AddUsers(SysRoleModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysRoleAddUsers, DATA_PROVIDER_NAME,
                model.RoleId,
                model.Users);

            return result.GetValueOrDefault(0);
        }

        public int AddUser(SysRoleModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysRoleAddUser, DATA_PROVIDER_NAME,
                model.RoleId,
                model.UserId);

            return result.GetValueOrDefault(0);
        }

        public bool RemoveUser(int roleId, int userId)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysRoleRemoveUser, DATA_PROVIDER_NAME, roleId, userId);
            return result == 1;
        }
    }
}