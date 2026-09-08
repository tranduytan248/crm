using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.API.Biz;
using Core.Cate.Caches;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using TSFramework.Libs.Processors;

namespace CenIT.Solution.TOC.WebApp.Providers
{
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        ///     Public client ID property.
        /// </summary>
        private readonly string _publicClientId;
        private readonly APICustomerBiz _CustomerBiz;
        private readonly Cate_CustomerCache _CustomerCache;

        public OAuthProvider(string publicClientId)
        {
            // Settings.  
            _publicClientId = publicClientId ?? throw new ArgumentNullException(nameof(publicClientId));
            _CustomerBiz = new APICustomerBiz();
            _CustomerCache = new Cate_CustomerCache();
        }

        #region[GrantResourceOwnerCredentials]

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                var allowedOrigin = "*";
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

                // lấy deviceToken 
                string deviceToken = context.Request.ReadFormAsync().Result.Get("deviceToken");
                string deviceInfo = context.Request.ReadFormAsync().Result.Get("deviceInfo");
                string deviceOS = context.Request.ReadFormAsync().Result.Get("deviceOS");
                string deviceUUID = context.Request.ReadFormAsync().Result.Get("deviceUUID");
                if (string.IsNullOrEmpty(deviceToken))
                {
                    context.SetError("deviceToken", "Không nhận được token");
                    return;
                }
                // ================= MOCK LOGIN =================
                var _username_test = ConfigurationManager.AppSettings["CONFIG_USERNAME_TEST"] ?? "KHA0001";
                var _password_test = ConfigurationManager.AppSettings["CONFIG_PASSWORD_TEST"] ?? "Kha@0001";
                if (context.UserName == _username_test && context.Password == _password_test)
                {
                    var mockUser = new
                    {
                        Account_ID = -1,
                        AccountUser = _username_test,
                        TypeAccount = "contract"
                    };

                    var claims1 = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, mockUser.Account_ID.ToString()),
                new Claim(ClaimTypes.Name, mockUser.AccountUser),
                new Claim("IsMock", "true")
            };

                    var oAuthIdentity = new ClaimsIdentity(
                        claims1, OAuthDefaults.AuthenticationType);

                    var cookieIdentity = new ClaimsIdentity(
                        claims1, CookieAuthenticationDefaults.AuthenticationType);

                    var properties1 = CreateProperties(new Dictionary<string, string>
            {
                { "id", mockUser.Account_ID.ToString() },
                { "userName", mockUser.AccountUser },
                { "TypeAccount", mockUser.TypeAccount },
                { "AccountUser", mockUser.AccountUser }
            });

                    var ticket1 = new AuthenticationTicket(oAuthIdentity, properties1);
                    context.Validated(ticket1);
                    context.Request.Context.Authentication.SignIn(cookieIdentity);
                    return;
                }
                var userId = _CustomerBiz.Login(new Modules.API.Models.KhachHang.KWC_User_LoginModel
                {
                    Username = context.UserName,
                    Password = context.Password,
                    DeviceToken = deviceToken,
                    DeviceInfo = deviceInfo,
                    DeviceOS = deviceOS,
                    DeviceUUID = deviceUUID
                });

                if (userId <= 0)
                {
                    // Settings.  
                    context.SetError("invalid_grant", AppProcessor.Messagor.GetMessage("Auth_Response_Incorrect"));
                    return;
                }

                //var userModel = AppProcessor.Author.GetUserInfo(context.UserName);
                var userModel = _CustomerCache.GetById(userId);
                if (userModel == null) return;
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, Convert.ToString(userModel.Account_ID)),
                    new Claim(ClaimTypes.Name, userModel.AccountUser)
                };

                // Setting Claim Identities for OAUTH 2 protocol.  

                var oAuthClaimIdentity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                var cookiesClaimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);

                var properties = CreateProperties(new Dictionary<string, string>
                {
                    { "id", userModel.Account_ID.ToString() },
                    { "userName", userModel.AccountUser },
                    { "TypeAccount", userModel.TypeAccount },
                    { "AccountUser", userModel.AccountUser }
                });
                var ticket = new AuthenticationTicket(oAuthClaimIdentity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(cookiesClaimIdentity);
            });
        }

        #endregion

        #region[ValidateClientAuthentication]

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
                context.Validated();

            return Task.FromResult<object>(null);
        }

        #endregion

        #region Validate client redirect URI override method

        /// <summary>
        ///     Validate client redirect URI override method
        /// </summary>
        /// <param name="context">Context parmeter</param>
        /// <returns>Returns validation of client redirect URI</returns>
        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            // Verification.  
            if (context.ClientId != _publicClientId) return Task.FromResult<object>(null);
            // Initialization.  
            var expectedRootUri = new Uri(context.Request.Uri, "/");

            // Verification.  
            if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                // Validating.  
                context.Validated();

            // Return info.  
            return Task.FromResult<object>(null);
        }

        #endregion

        #region[TokenEndpoint]

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var property in context.Properties.Dictionary)
                context.AdditionalResponseParameters.Add(property.Key, property.Value);

            return Task.FromResult<object>(null);
        }

        #endregion

        #region[CreateProperties]

        public static AuthenticationProperties CreateProperties(Dictionary<string, string> dataPropAuthen)
        {
            return new AuthenticationProperties(dataPropAuthen);
        }

        //public static AuthenticationProperties CreateProperties(string userName, string fullName, string email)
        //{
        //    IDictionary<string, string> data = new Dictionary<string, string>
        //    {
        //        {"username", userName},
        //        {"fullname", fullName},
        //        {"email", email}
        //    };
        //    return new AuthenticationProperties(data);
        //}

        #endregion
    }
}