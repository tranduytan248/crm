using System;
using System.Linq;
using System.Security.Principal;
using TSFramework.Libs.Interfaces;

namespace TSFramework.Libs.Principals
{
    public class AppPrincipal : IBasePrincipal
    {
        public AppPrincipal(string username)
        {
            Identity = new GenericIdentity(username);
        }

        public string[] Permits { get; set; }

        public string Token { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Avatar { get; set; }

        public IIdentity Identity { get; }

        public int UserId { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsInRole(string permit)
        {
            return Permits.Any(permit.Equals);
        }
    }
}