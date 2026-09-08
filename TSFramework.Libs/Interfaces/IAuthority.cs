namespace TSFramework.Libs.Interfaces
{
    public interface IAuthority
    {
        IStoreProcedure ProcedureProvider { get; set; }
        bool IsAllow(string userName, string areaName, string controllerName, string actionName);
        bool IsValidUser(params object[] paramLogin);
        bool SaveLogin(params object[] paramLogin);
        AuthorizeData GetUserInfo(params object[] paramLogin);
        AuthorizeData Login(params object[] paramLogin);
        AuthorizeData Register(params object[] paramRegister);
        AuthorizeData Token(params object[] paramLogin);
    }

    public class AuthorizeData
    {
        public int UserId { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
    }
}