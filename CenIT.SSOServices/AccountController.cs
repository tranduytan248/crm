using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Core.API.Models;
using Core.API.Providers;
using Core.API.Results;
using System.Linq;
using Core.API.Constant.Messages;
using Core.BusinessLayer;
using Newtonsoft.Json;
using Core.DomainModel;
using CoreFramework.Constant;
using System.Web.Security;
using Core.API.FillterAttribute;
using CenIT.SSOServices.ServiceSSO;

namespace Core.API.Controllers
{
    [RoutePrefix("api/Account")]
    [AllowAnonymous]

    public class AccountController : BaseAPIController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        private readonly INguoiDungBL _nguoiDungBLL = BusinessFactory.Create<INguoiDungBL>();
        private readonly ITramBL _tramBLL = BusinessFactory.Create<ITramBL>();
        private readonly IMenuBL _menuBLL = BusinessFactory.Create<IMenuBL>();
        private readonly IStoreProcedureBL _storeProcedureBLL = BusinessFactory.Create<IStoreProcedureBL>();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [Route("LoginSSO")]
        [HttpPost]
        [AllowAnonymous]
        [AllowAnonymousFilter]
        public IHttpActionResult LoginSSO(UserAuthenCationHeaderModels _UserAuthenCationHeader)
        {
            try
            {
                CService sv = new CService();
                CheckUserInfo user = sv.CheckUser(_UserAuthenCationHeader.SessionID, _UserAuthenCationHeader.Userkey, _UserAuthenCationHeader.Ticket, _UserAuthenCationHeader.AppCode, "");
                //fixloi dang nhap 10042021
                NguoiDung Nguoidung = _nguoiDungBLL.GetByEmail(user.globalusername + "@vnpt.vn");
                if(Nguoidung != null)
                {
                    user.Email_VNPTKhanhHoa = Nguoidung.Email_VTKH;
                }
                //End fixloi dang nhap 10042021
                return Json(new { status = EnumStartus.Success, data = user, message = string.Empty });
            }
            catch (Exception exx)
            {
                return Json(new { status = EnumStartus.Error, message = "Khong ket noi duoc toi services SSO:" + exx.ToString() });
            }

        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="_userName"></param>
        /// <returns></returns>
        [Route("Login")]
        [HttpPost]
        [AllowAnonymousFilter]
        public IHttpActionResult LogIn(RegisterExternalBindingModel _userName)
        {

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            var _nguoiDung = _nguoiDungBLL.GetByEmail(_userName.Email);
            if (_nguoiDung != null && User.Identity.GetUserName() == _userName.Email)
            {
                _storeProcedureBLL.LogData_Insert("Login", "", User.Identity.Name, 3);
                #region MENUS
                List<Menu> _lstMenu = new List<Menu>();
                var _menus = _menuBLL.GetAllMenuByNguoiDung(_nguoiDung.ID);

                foreach (Menu mn in _menus)
                {
                    var _childs = _menuBLL.GetAllChildByParent(mn.ID, _nguoiDung.ID);
                    if (_childs.Count > 0 || !string.IsNullOrEmpty(mn.URL))
                    {
                        foreach (Menu mc in _childs)
                        {
                            if (mc.Da_Xoa)
                            {
                                continue;
                            }
                            mc.ChucNang = null;
                        }

                        _lstMenu.Add(new Menu()
                        {
                            ID = mn.ID,
                            MenuChilds = _childs,
                            Ten = mn.Ten,
                            URL = mn.URL,
                            ViTri = mn.ViTri,
                            Icon = mn.Icon,
                        });
                    }
                }

                #endregion

                #region TRAM
                var _trams = _tramBLL.DanhSachTramTheoNhanVien(_nguoiDung.ID);
                #endregion

                #region NGUOIDUNG
                var _mNguoiDung = new NguoiDungModels()
                {
                    ID = _nguoiDung.ID,
                    HoTen = _nguoiDung.HoTen,
                    Gioi_Tinh = _nguoiDung.Gioi_Tinh,
                    Email_VTKH = _nguoiDung.Email_VTKH,
                    Email_VNPT = _nguoiDung.Email_VNPT,
                    Ten_DonVi = _nguoiDung.DonVi != null ? _nguoiDung.DonVi.Ten_DonVi : string.Empty,
                    ID_Nhom = _nguoiDung.ID_Nhom,
                    ID_DonVi = _nguoiDung.ID_DonVi
                };
                #endregion

                #region QUYỀN NGƯỜI DÙNG
                var _quyenNguoiDungs = _storeProcedureBLL.LayDSQuyenNguoiDung(_nguoiDung.ID);
                List<string> _dsQuyen = new List<string>();

                foreach (var quyen in _quyenNguoiDungs)
                {
                    _dsQuyen.Add(string.Format("{0}/{1}", quyen.URL, quyen.Action));
                }

                #endregion

                return Json(new
                {
                    status = EnumStartus.Success,
                    data = new
                    {
                        user = _mNguoiDung,
                        menu = _lstMenu,
                        trams = _trams,
                        quyens = _dsQuyen
                    },
                    message = Constant.Messages.ConstMessages.LOGIN_SUCCESS
                },
                    new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            return Json(new { status = EnumStartus.LogInFail, data = string.Empty, message = Constant.Messages.ConstMessages.LOGIN_FAIL },
                    new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        /// <summary>
        /// Lấy thông tin người dùng
        /// </summary>
        /// <returns></returns>
        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        /// <summary>
        /// Đăng xuất tài khoản
        /// </summary>
        /// <returns></returns>
        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            _storeProcedureBLL.LogData_Insert("Logout", "0", User.Identity.Name, 3);
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            CService sv = new CService();
            return Ok();
        }

        [Route("LogoutSSO")]
        [HttpPost]
        [AllowAnonymous]
        [AllowAnonymousFilter]
        public IHttpActionResult LogoutSSO(UserAuthenCationHeaderModels _UserAuthenCationHeader)
        {
            _storeProcedureBLL.LogData_Insert("Logout", "0", _UserAuthenCationHeader.Email, 3);
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            CService sv = new CService();
            sv.LogOutByTicket(_UserAuthenCationHeader.Ticket);
            return Ok();
        }
        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        //[Route("ChangePassword")]
        //public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
        //        model.NewPassword);

        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }

        //    return Ok();
        //}

        //// POST api/Account/SetPassword
        //[Route("SetPassword")]
        //public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }

        //    return Ok();
        //}

        // POST api/Account/AddExternalLogin

        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        //// GET api/Account/ExternalLogin
        //[OverrideAuthentication]
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        //[AllowAnonymous]
        //[Route("ExternalLogin", Name = "ExternalLogin")]
        //public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        //{
        //    if (error != null)
        //    {
        //        return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
        //    }

        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return new ChallengeResult(provider, this);
        //    }

        //    ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

        //    if (externalLogin == null)
        //    {
        //        return InternalServerError();
        //    }

        //    if (externalLogin.LoginProvider != provider)
        //    {
        //        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //        return new ChallengeResult(provider, this);
        //    }

        //    ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
        //        externalLogin.ProviderKey));

        //    bool hasRegistered = user != null;

        //    if (hasRegistered)
        //    {
        //        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

        //        ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
        //           OAuthDefaults.AuthenticationType);
        //        ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
        //            CookieAuthenticationDefaults.AuthenticationType);

        //        AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
        //        Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
        //    }
        //    else
        //    {
        //        IEnumerable<Claim> claims = externalLogin.GetClaims();
        //        ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
        //        Authentication.SignIn(identity);
        //    }

        //    return Ok();
        //}

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true

        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        /// <summary>
        /// Đăng ký tài khoản người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = EnumStartus.Error,
                    data = ModelState.Select(s => s.Value.Errors[0]),
                    message = ModelState.First().Value.Errors[0].ErrorMessage
                });
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, ConstantSecurity.Core_Identity_Password);

            if (!result.Succeeded)
            {
                //return GetErrorResult(result);
                return Json(new
                {
                    status = EnumStartus.Error,
                    data = new[] { model },
                    message = string.Format(ConstMessages.ADD_FAIL, model.Email)
                });
            }

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        /// <summary>
        /// Kiểm tra quyền của người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("Permission")]
        [HttpPost]
        public IHttpActionResult CheckPermission(CheckPermissionBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = EnumStartus.Error,
                    data = ModelState.Select(s => s.Value.Errors[0]),
                    message = ModelState.First().Value.Errors[0].ErrorMessage
                });
            }

            var _nguoiDung = _nguoiDungBLL.GetByEmail(model.Email);
            if (_nguoiDung != null)
            {
                return Json(new
                {
                    status = EnumStartus.Success,
                    data = new[] { _nguoiDung },
                    message = string.Empty
                });
            }

            return Json(new { status = EnumStartus.Error, data = string.Empty, message = ConstMessages.DATA_NOT_FOUND });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

    }
}
