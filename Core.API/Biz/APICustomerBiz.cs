using Modules.API.Models.KhachHang;
using TSFramework.Libs.Processors;

namespace Core.API.Biz
{
    public class APICustomerBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string Cate_Customer_ChangePassword = "Cate_Customer_ChangePassword";
        private readonly string Cate_Customer_Login = "Cate_Customer_Login";

        /// <summary>
        /// Cập nhật danh sách MN_BoPhan theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Login(KWC_User_LoginModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(Cate_Customer_Login, DATA_PROVIDER_NAME
               , model.Username
               , model.Password
               , model.DeviceOS, model.DeviceInfo, model.DeviceUUID, model.DeviceToken);
            return result.GetValueOrDefault(0);
        }
        public int ChangePassword(ChangePasswordRequestModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(Cate_Customer_ChangePassword, DATA_PROVIDER_NAME,
                model.CustomerCode,
                model.CurrentPassword,
                model.NewPassword,
                model.Salt
            );
            return result.GetValueOrDefault(0);
        }
    }
}

