using Core.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Processors;

namespace Core.API.Biz
{
    public class KWC_MobileOTPBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _MobileOTP_GetByID = "KWC_MobileOTP_GetByID";
        private readonly string _MobileOTP_Save = "KWC_MobileOTP_Save";
        private readonly string _poPOQrCodeLogCallAPISave = "PO_QrCode_LogCallAPI_Save";


        // Send/Resend OTP
        public int Save(KWC_MobileOTPModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_MobileOTP_Save, DATA_PROVIDER_NAME,
                model.ID
                , model.AccountUser
                , model.PhoneNumber
                , model.OTP
                , model.DeviceOS
                , model.DeviceUUID
                , model.InfoOTP);
            return result.GetValueOrDefault(0);
        }

        // Lấy tt tài khoản theo ID
        public KWC_MobileOTPModel GetByID(long ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<KWC_MobileOTPModel>(_MobileOTP_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        public int? AddLogCallAPI(QrCodeReqCallAPILogModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_poPOQrCodeLogCallAPISave, DATA_PROVIDER_NAME,
                model.APINameOrURL, model.Request, model.Response);
            return result;
        }
    }
}
