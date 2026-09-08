using System.Collections.Generic;
using System.Linq;
using Core.Cate.Models;
using Core.Sys.Models.Sys;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysUserBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";

        private readonly string _sysRoleGetByUserId = "Sys_Role_GetByUserId";
        private readonly string _sysUserActive = "Sys_User_Active";
        private readonly string _sysUserChangePassword = "Sys_User_ChangePassword";
        private readonly string _sysUserDeActive = "Sys_User_DeActive";
        private readonly string _sysUserDelete = "Sys_User_Delete";
        private readonly string _sysUserGet = "Sys_User_Get";
        private readonly string _sysUserGetByEmail = "Sys_User_GetByEmail";
        private readonly string _sysUserGetById = "Sys_User_GetById";
        private readonly string _sysUserGetByUserName = "Sys_User_GetByUserName";
        private readonly string _sysUserGetViaRole = "Sys_User_GetViaRole";
        private readonly string _sysUserLogin = "Sys_User_Login";
        private readonly string _sysUserLoginViaEmail = "Sys_User_LoginViaEmail";
        private readonly string _sysUserPermit = "Sys_User_Permit";
        private readonly string _sysUserResetPassword = "Sys_User_ResetPassword";
        private readonly string _sysUserSave = "Sys_User_Save";
        private readonly string _sysUserSaveLogin = "Sys_User_SaveLogin";
        private readonly string _sysUserUpdateInfo = "Sys_User_UpdateInfo";
        private readonly string _sysUserGetByLichSuPhieuID = "Sys_User_GetByLichSuPhieuID";
        private readonly string _sysUserGetByBoPhanAndChucVu = "Sys_User_GetByBoPhanAndChucVu";
        private readonly string _sysUserGetGetManager = "Sys_User_GetManager";
        private readonly string _sysUserMarkRegulation = "Sys_User_MarkRegulation";

        private List<SysUserModel> LoadList(out int total, SysUserSearchModel searchModel, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var dataUser = AppProcessor.ProcedureProvider.ExecuteTypedList<SysUserModel>(_sysUserGet,
                DATA_PROVIDER_NAME,
                //searchModel.TuKhoa,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (dataUser != null && dataUser.Count > 0)
                total = int.Parse(dataUser.First()?.TotalRow.ToString() ?? "0");
            return dataUser;
        }

        private SysUserModel LoadDetail(int userId)
        {
            var dataUser =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysUserModel>(_sysUserGetById, DATA_PROVIDER_NAME,
                    userId);
            return dataUser;
        }

        private SysUserModel LoadDetail(string userName)
        {
            var dataUser =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysUserModel>(_sysUserGetByUserName,
                    DATA_PROVIDER_NAME, userName);
            return dataUser;
        }

        public SysUserModel GetByEmail(string email)
        {
            var dataUser =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysUserModel>(_sysUserGetByEmail, DATA_PROVIDER_NAME,
                    email);
            return dataUser;
        }

        public SysUserModel Login(string userName, string password)
        {
            var dataLogin =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysUserModel>(_sysUserLogin, DATA_PROVIDER_NAME,
                    userName, password);
            return dataLogin;
        }

        public SysUserModel LoginViaEmail(string email, string password)
        {
            var dataLogin =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysUserModel>(_sysUserLoginViaEmail,
                    DATA_PROVIDER_NAME, email, password);
            return dataLogin;
        }

        public int Save(SysUserModel model, string savedBy)
        {
            var userId = AppProcessor.ProcedureProvider.Execute(_sysUserSave,
                DATA_PROVIDER_NAME,
                model.UserId,
                model.Email,
                model.FullName,
                model.UserName,
                model.Password,
                model.Salt,
                model.Avatar,
                model.Phone,
                model.RoleIDs,
                model.IsActive,
                model.Reason,
                savedBy
            );

            return userId.GetValueOrDefault(0);
        }

        public bool Delete(SysUserModel model, string deletedBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysUserDelete, DATA_PROVIDER_NAME, model.UserId,
                model.Reason, deletedBy);
            return result == model.UserId;
        }

        public bool DeActive(SysUserModel model, string deactiveBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysUserDeActive, DATA_PROVIDER_NAME, model.UserId,
                model.Reason, deactiveBy);
            return result == model.UserId;
        }

        public bool Active(SysUserModel model, string activeBy)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysUserActive, DATA_PROVIDER_NAME, model.UserId,
                model.Reason, activeBy);
            return result == model.UserId;
        }

        public List<SysUserModel> GetAll()
        {
            var searchModel = new SysUserSearchModel();
            var listUsers = LoadList(out _, searchModel, null);
            return listUsers;
        }

        public SysUserModel GetById(int userId)
        {
            var user = LoadDetail(userId);
            return user;
        }

        public SysUserModel GetByUserName(string userName)
        {
            var user = LoadDetail(userName);
            return user;
        }

        public List<SysUserModel> GetList(out int total, SysUserSearchModel searchModel, BaseSearchModel search = null)
        {
            var listUsers = LoadList(out total, searchModel, search);
            return listUsers;
        }

        public List<SysRoleModel> GetRoles(int? userId)
        {
            var dataRoles =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysRoleModel>(_sysRoleGetByUserId, DATA_PROVIDER_NAME,
                    userId);
            return dataRoles;
        }

        public int ChangePassword(string userName, string oldPass, string newPass, string salt, string reason,
            string changeBy)
        {
            var valReturn = AppProcessor.ProcedureProvider.Execute(_sysUserChangePassword,
                DATA_PROVIDER_NAME,
                userName,
                oldPass,
                newPass,
                salt,
                reason,
                changeBy);

            return valReturn.GetValueOrDefault(0);
        }

        public int ResetPassword(string userName, string newPass, string salt, string reason, string changeBy)
        {
            var valReturn = AppProcessor.ProcedureProvider.Execute(_sysUserResetPassword,
                DATA_PROVIDER_NAME,
                userName,
                newPass,
                salt,
                reason,
                changeBy
            );

            return valReturn.GetValueOrDefault(0);
        }

        public int SaveLogin(string userName, bool isValid, string senderIp, string senderHeader)
        {
            var iReturnInt = AppProcessor.ProcedureProvider.Execute(_sysUserSaveLogin, DATA_PROVIDER_NAME, userName,
                isValid, senderIp, senderHeader);
            return iReturnInt.GetValueOrDefault(0);
        }

        public List<SysUserModel> GetViaRole(int? roleId, out int total, BaseSearchModel search = null)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var dataUser = AppProcessor.ProcedureProvider.ExecuteTypedList<SysUserModel>(_sysUserGetViaRole,
                DATA_PROVIDER_NAME,
                roleId,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (dataUser != null && dataUser.Count > 0)
                total = int.Parse(dataUser.First()?.TotalRow.ToString() ?? "0");

            return dataUser;
        }

        public int Permit(int? userId, string roleIds, string moduleIds, string typeServiceIds)
        {
            var iReturnInt =
                AppProcessor.ProcedureProvider.Execute(_sysUserPermit, DATA_PROVIDER_NAME, userId, roleIds, moduleIds, typeServiceIds);
            return iReturnInt.GetValueOrDefault(0);
        }

        public int UpdateInfo(SysUserModel model, string savedBy)
        {
            var userId = AppProcessor.ProcedureProvider.Execute(_sysUserUpdateInfo,
                DATA_PROVIDER_NAME,
                model.UserId,
                model.Email,
                model.FullName,
                model.Avatar,
                model.Phone,
                savedBy
            );

            return userId.GetValueOrDefault(0);
        }

        /// <summary>
        /// Lấy danh sách user theo LoaiPhieuQuyTrinhID
        /// </summary>
        /// <returns>Danh sách MN_LoaiPhieuQuyTrinh</returns>
        public List<SysUserModel> GetByLichSuPhieu(int lichSuPhieuID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<SysUserModel>(_sysUserGetByLichSuPhieuID,
                DATA_PROVIDER_NAME,
                lichSuPhieuID);
            return data;
        }

        /// <summary>
        /// Lấy danh sách user theo bộ phận và chức vụ
        /// </summary>
        /// <returns>Danh sách MN_LoaiPhieuQuyTrinh</returns>
        public List<SysUserModel> GetByBoPhanAndChucVu(int? boPhanID, int? chucVuID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<SysUserModel>(_sysUserGetByBoPhanAndChucVu,
                DATA_PROVIDER_NAME,
                boPhanID,
                chucVuID);
            return data;
        }

        /// <summary>
        /// Lấy danh sách user quản lý từ list user id truyền vào
        /// </summary>
        /// <returns>Danh sách user</returns>
        public List<SysUserModel> GetManagement(string userIds)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<SysUserModel>(_sysUserGetGetManager, DATA_PROVIDER_NAME, userIds);
            return list;
        }

        public int MarkRegulation(string username)
        {
            var userId = AppProcessor.ProcedureProvider.Execute(_sysUserMarkRegulation, DATA_PROVIDER_NAME, username);

            return userId.GetValueOrDefault(0);
        }
    }
}