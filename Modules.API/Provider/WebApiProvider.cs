using Core.API.Caches;
using Modules.API.Models;
using System;
using System.Configuration;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Modules.Dashboard.Provider
{
    public class WebApiProvider
    {
        private static KeyPrivateCache _keyPrivaceCache = new KeyPrivateCache();

        public static ResponseContentModel ValidateToken(BaseApiModel model)
        {
            var response = new ResponseContentModel();

            try
            {
                var infoFromCache = _keyPrivaceCache.GetTokenByUser(model.UserName); //Thông tin token được lưu trên cache

                //1 - Trường hợp tokenFromCache null => return về 401 => Trả về lỗi Unauthorized => message thông báo lại Token đã hết hạn
                if (infoFromCache == null)
                {
                    response.Status = 401;
                    response.Message = AppProcessor.Messagor.GetMessage("API_Token_OutOfDate");
                    return response;
                }

                var tokenFromCache = infoFromCache.TokenAuth; //Thông tin token được lưu trên cache

                //2 - Trường hợp token truyền lên và token được lưu trên cache khác nhau => Trả về lỗi Unauthorized => message thông báo lại Token gửi lên không đúng
                if (!tokenFromCache.Trim().Equals(model.Token.Trim()))
                {
                    response.Status = 401;
                    response.Message = AppProcessor.Messagor.GetMessage("API_Invalid_Token");
                    return response;
                }
                else
                {
                    //Xử lý ktra thông signature
                    //Nếu thông tin không khớp  => Trả về lỗi Unauthorized => message thông báo lại Thông tin truy cập gửi lên không đúng
                    var tokenHash = UtilString.FStrMd5((ConfigurationManager.AppSettings["Key_Private_API"]) + infoFromCache.TokenAuth);
                    if (!string.Equals(model.Signature.Trim(), tokenHash.Trim()))
                    {
                        response.Status = 401;
                        response.Message = AppProcessor.Messagor.GetMessage("API_Invalid_Info");
                        return response;
                    }

                    //Nếu thông tin khớp => Trả về success => message thông báo lại Thông tin truy cập thành công

                }

                response.Status = 200;
                response.Message = string.Empty;

                //Save to cache - Using when api not call
                //Mỗi lần gọi API sẽ lưu thông tin cache mới
                _keyPrivaceCache.SaveToken(infoFromCache, model.Token);

                return response;

            }
            catch (Exception)
            {
                //Trường hợp lỗi trả về 500 => Message: Hệ thống lỗi 
                response.Status = 500;
                response.Message = AppProcessor.Messagor.GetMessage("API_Server_Error");
                return response;
            }
        }
    }
}
