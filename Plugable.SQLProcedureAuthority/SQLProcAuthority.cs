using System;
using System.Linq;
using TSFramework.Libs.Interfaces;

namespace Plugable.SQLProcedureAuthority
{
    public class SQLProcAuthority : IAuthority
    {
        public IStoreProcedure ProcedureProvider { get; set; }

        public bool IsAllow(string userName, string areaName, string controllerName, string actionName)
        {
            var spCheckPermission = "Sys_Permission_IsAllow";

            var permissionObj = ProcedureProvider?.ExecuteProcedureTypedList<object>(spCheckPermission, userName,
                areaName, controllerName, actionName);

            return permissionObj != null && permissionObj.Count > 0;
        }

        public bool IsValidUser(params object[] paramLogin)
        {
            var spCheckLogin = "Sys_User_Login";
            var permissionObj = ProcedureProvider.ExecuteProcedureTypedList<object>(spCheckLogin, paramLogin.ToArray());

            return permissionObj != null && permissionObj.Count > 0;
        }

        public AuthorizeData GetUserInfo(params object[] paramLogin)
        {
            var spGetInfoViaUserName = "Sys_User_GetByUserName";
            var authDatas =
                ProcedureProvider.ExecuteProcedureTypedList<AuthorizeData>(spGetInfoViaUserName, paramLogin.ToArray());
            return authDatas?[0];
        }

        public AuthorizeData Login(params object[] paramLogin)
        {
            var spCheckLogin = "Sys_User_Login";
            var authDatas =
                ProcedureProvider.ExecuteProcedureTypedList<AuthorizeData>(spCheckLogin, paramLogin.ToArray());
            return authDatas?[0];
        }

        public AuthorizeData Register(params object[] paramRegister)
        {
            throw new NotImplementedException();
        }

        public AuthorizeData Token(params object[] paramLogin)
        {
            var spCheckToken = "Sys_User_Token";

            var authDatas =
                ProcedureProvider.ExecuteProcedureTypedList<AuthorizeData>(spCheckToken, paramLogin.ToArray());
            return authDatas?[0];
        }

        public bool SaveLogin(params object[] paramLogin)
        {
            var spCheckLogin = "Sys_User_SaveLogin";
            var retInt = ProcedureProvider.ExecuteProcedure(spCheckLogin, paramLogin.ToArray());

            return retInt != null && retInt > 0;
        }
    }
}