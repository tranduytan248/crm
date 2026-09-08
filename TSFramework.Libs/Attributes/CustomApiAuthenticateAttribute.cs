using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using TSFramework.Libs.Principals;
using TSFramework.Libs.Processors;

namespace TSFramework.Libs.Attributes
{
    public class CustomApiAuthorizeAttribute : AuthorizeAttribute
    {
        private AppPrincipal User => HttpContext.Current.User as AppPrincipal;

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var context = actionContext.RequestContext;

            if (AllowAnonymous(actionContext)) return;
            var ci = context.Principal.Identity as ClaimsIdentity;
            var authHeader = actionContext.Request.Headers.Authorization;
            if (authHeader == null || authHeader.Scheme.ToLower() != "bearer")
            {
                actionContext.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent(AppProcessor.Messagor.GetMessage("Authorize_AuthenticationRequired"))
                };
                return;
            }
            else if (ci != null && !ci.IsAuthenticated)
            {
                actionContext.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent(AppProcessor.Messagor.GetMessage("Authorize_AuthenticationTokenExpired"))
                };
                return;
            }
            else
            {
                var sToken = authHeader.Parameter;
                var jwtToken = new JwtSecurityToken(authHeader.Parameter);
                var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "customercode")?.Value;
                var fullName = jwtToken.Claims.FirstOrDefault(c => c.Type == "fullname")?.Value;
                var phone = jwtToken.Claims.FirstOrDefault(c => c.Type == "phone")?.Value;
                var userId = (jwtToken.Claims.FirstOrDefault(c => c.Type == "userid")?.Value) ?? "0";
                var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var password = jwtToken.Claims.FirstOrDefault(c => c.Type == "password")?.Value;

                var principal = new AppPrincipal(email)
                {
                    Email = email,
                    UserName = userName,
                    FullName = fullName,
                    UserId = int.Parse(userId),
                    Token = sToken,
                };

                Thread.CurrentPrincipal = principal;
                HttpContext.Current.User = principal;
            }
            base.HandleUnauthorizedRequest(actionContext);
        }

        private static bool AllowAnonymous(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ActionDescriptor.ControllerDescriptor
                       .GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}