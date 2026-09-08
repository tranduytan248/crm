using Core.API.Biz;
using Core.API.Enums;
using Core.Cate.Caches;
using Modules.API.Models.KhachHang;
using System.Web.Http;
using TSFramework.Libs.BaseApps;
using TSFramework.Libs.Utils;
using Core.Cate.Models;
using System.Text.RegularExpressions;

namespace Modules.API.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : BaseApiController
    {
        private readonly APICustomerBiz aPICustomerBiz;
        private readonly KWC_AccountMobileCache _AccountMobileCache;

        public AccountController()
        {
            aPICustomerBiz = new APICustomerBiz();
            _AccountMobileCache = new KWC_AccountMobileCache(); 
        }

        [HttpPost]
        [Route("change-password")]
        public IHttpActionResult ChangePassword(ChangePasswordRequestModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.CustomerCode) ||
                string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                return Json(new
                {
                    status = 400,
                    data = new { },
                    message = "Thiếu thông tin"
                });
            }

            var salt = UtilEncrypt.GenerateSalt();
            var passwordHash = UtilEncrypt.GenerateCryptoPassword(model.NewPassword, salt);
            model.Salt = salt;
            model.NewPassword = passwordHash;

            var isChanged = aPICustomerBiz.ChangePassword(model);

            if (isChanged == -2)
            {
                return Json(new
                {
                    status = 400,
                    data = new { },
                    message = "Tài khoản không tồn tại hoặc đã bị khóa"
                });
            }
            if (isChanged == -3)
            {
                return Json(new
                {
                    status = 400,
                    data = new { },
                    message = "Mật khẩu hiện tại không đúng"
                });
            }

            return Json(new
            {
                status = 200,
                data = new { },
                message = "Đổi mật khẩu thành công"
            });
        }



    }
}
