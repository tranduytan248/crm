using Modules.API.Models;
using Modules.Dashboard.Provider;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http;
using System.Net.Http.Formatting;

namespace Modules.API.Attribute
{
    public class CustomAuthenticateAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// kiểm tra quyền truy cập từ value trên header
        /// </summary>
        /// <param name="actionContext"></param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (AllowAnonymous(actionContext)) return;

            // lấy value trong header
            var signatureHeader = actionContext.Request.Headers.FirstOrDefault(k => k.Key == "Signature").Value;
            var usernameHeader = actionContext.Request.Headers.FirstOrDefault(k => k.Key == "Username").Value;
            var tokenHeader = actionContext.Request.Headers.FirstOrDefault(k => k.Key == "Token").Value;

            var signature = signatureHeader != null ? signatureHeader.FirstOrDefault() : null;
            var username = usernameHeader != null ? usernameHeader.FirstOrDefault() : null;
            var token = tokenHeader != null ? tokenHeader.FirstOrDefault() : null;


            if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
            {
                var responseContentObj = new ResponseContentModel()
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Message = "Chưa đăng nhập" //AppProcessor.Messagor.GetMessage("API_Invalid_Authentication")
                };

                // Chuyển đổi đối tượng sang nội dung của phản hồi HTTP
                var responseContent = new ObjectContent<ResponseContentModel>(responseContentObj, new JsonMediaTypeFormatter());

                // Tạo phản hồi HTTP với nội dung đã được thiết lập
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = responseContent;
                actionContext.Response = response;
                return;
            }

            // kiểm tra private key
            var baseModel = new BaseApiModel
            {
                Signature = signature,
                UserName = username,
                Token = token
            };

            ResponseContentModel validToken = WebApiProvider.ValidateToken(baseModel);

            if (validToken.Status != 200)
            {
                // Chuyển đổi đối tượng sang nội dung của phản hồi HTTP
                var responseContent = new ObjectContent<ResponseContentModel>(validToken, new JsonMediaTypeFormatter());

                // Tạo phản hồi HTTP với nội dung đã được thiết lập
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = responseContent;
                actionContext.Response = response;
                return;
            }
        }

        /// <summary>
        /// kiểm tra api đang check quyền truy cập hay không
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private static bool AllowAnonymous(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ActionDescriptor.ControllerDescriptor
                       .GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}