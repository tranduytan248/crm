using Core.API.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.API.Biz
{
    public class KeyPrivateBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";

        private readonly string _dl_API_Authentication_GetKeyPrivace = "API_Authentication_GetKeyPrivace";
        private readonly string _dl_API_Authentication_ValidateBusinessAccount = "API_Authentication_ValidateBusinessAccount";
        private readonly string _dl_API_Authentication_CheckKeyPrivaceAndApplyFor = "API_Authentication_CheckKeyPrivaceAndApplyFor";
        private readonly string _dl_API_Authentication_CheckBusinessAccount = "API_Authentication_CheckBusinessAccount";
        private readonly string _apiGetToken = "API_Token_Get";
        private readonly string _apiSaveToken = "API_Token_Save";

        /// <summary>
        /// Check tài khoản người dùng (username, password)
        /// </summary>
        /// <returns>Thông tin Key so với username và applyFor</returns>
        public KeyPrivateModel ValidAccount(string username, string password, string applyFor)
        { 
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<KeyPrivateModel>(_dl_API_Authentication_GetKeyPrivace,
                DATA_PROVIDER_NAME,
                username,
                password,
                applyFor);

            return data;
        }

        /// <summary>
        /// Lấy thông tin từ chuỗi Token
        /// </summary>
        /// <returns>Thông tin Key so với username và applyFor</returns>
        public KeyPrivateModel GetToken(string token)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<KeyPrivateModel>(_apiGetToken,
                DATA_PROVIDER_NAME,
                token);

            return data;
        }

        /// <summary>
        /// Lưu thông tin xuống database
        /// </summary>
        /// <param name="token"></param>
        /// <param name="username"></param>
        public void SaveToken(string token, string username)
        {
            AppProcessor.ProcedureProvider.Execute(_apiSaveToken, DATA_PROVIDER_NAME,
                token, username
            );
        }

        /// <summary>
        /// Đăng nhập cho doanh nghiệp
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public KeyPrivateModel ValidateBusinessAccount(string username, string password)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<KeyPrivateModel>(_dl_API_Authentication_ValidateBusinessAccount,
                DATA_PROVIDER_NAME,
                username,
                password);
            return data;
        }

        /// <summary>
        /// Check token 
        /// </summary>
        /// <returns></returns>
        public KeyPrivateModel CheckToken(string signature, string applyFor, string token, string username)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<KeyPrivateModel>(_dl_API_Authentication_CheckKeyPrivaceAndApplyFor,
                DATA_PROVIDER_NAME,
                signature,
                applyFor,
                token,
                username);
            return data;
        }


        /// <summary>
        /// Kiêm tra username có phải là tài khoản doanh nghiệp
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public bool IsBusinessAccount(string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_dl_API_Authentication_CheckBusinessAccount, DATA_PROVIDER_NAME, username);
            return result > 0;
        }
    }
}
