using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Principals;
using TSFramework.Libs.Processors;

namespace Core.Sys.Permissions
{
    public class AppPermissionContext
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private const string SP_GET_IDS_BY_MANAGER = "Sys_User_ByManager";

        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string FullName { get; private set; }

        public int EmployeeId => UserId;
        public List<int> AllowedEmployeeIds { get; private set; }

        public static AppPermissionContext FromPrincipal(AppPrincipal user)
        {
            var ctx = new AppPermissionContext
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName
            };
            ctx.Resolve();
            return ctx;
        }

        private void Resolve()
        {
            var result = AppProcessor.ProcedureProvider
                .ExecuteTypedList<UserIdModel>(SP_GET_IDS_BY_MANAGER, DATA_PROVIDER_NAME, UserName)
                ?? new List<UserIdModel>();

            var ids = result.Select(r => r.Value).Distinct().ToList();

            AllowedEmployeeIds = ids.Any() ? ids : new List<int> { UserId };
        }

        public string GetAllowedEmployeeIdsString()
            => AllowedEmployeeIds == null ? null : string.Join(",", AllowedEmployeeIds);

        private class UserIdModel
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }
    }
}