using Core.API.Models;
using DocumentFormat.OpenXml.VariantTypes;
using Modules.API.Models.NewsKhachHang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Processors;
using KWC_AccountMobileModel = Core.API.Models.KWC_AccountMobileModel;

namespace Core.API.Biz
{
    public class AccountBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _AccountMobile_CheckExist = "KWC_AccountMobile_CheckExist";
        private readonly string _AccountMobile_Login = "KWC_AccountMobile_Login";
        private readonly string _AccountMobile_Save = "KWC_AccountMobile_Save";
        private readonly string _ContractGetInfo = "KWC_Contract_GetInfo";

        // Ktra tài khoản tồn tại
        public int IsExist(KWC_AccountMobileModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_AccountMobile_CheckExist, DATA_PROVIDER_NAME, model.AccountUser, model.TypeAccount);
            return result.GetValueOrDefault(0);
        }

        // Login
        public int Login(KWC_AccountMobile_LoginModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_AccountMobile_Login, DATA_PROVIDER_NAME,
                model.AccountUser,
                model.Password,
                model.DeviceOS,
                model.DeviceInfo,
                model.DeviceUUID,
                model.DeviceToken);
            return result.GetValueOrDefault(0);
        }

        // tạo tài khoản
        public int Create(KWC_AccountMobileModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_AccountMobile_Save, DATA_PROVIDER_NAME,
                model.AccountUser,
                model.TypeAccount,
                model.Password,
                model.Salt);
            return result.GetValueOrDefault(0);
        }

        // tạo tài khoản
        //public CSKH_KH_CONTRACT_Model GetInfoContract(string contractCode, string phoneNumber)
        //{
        //    var result = 
        //    AppProcessor.ProcedureProvider.ExecuteTypedList<CSKH_KH_CONTRACT_Model>(
        //       _ContractGetInfo, DATA_PROVIDER_NAME, contractCode, phoneNumber);
        //    return result?.FirstOrDefault(); ;
        //}
    }
}
