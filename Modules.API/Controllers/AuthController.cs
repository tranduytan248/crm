using CenIT.Lib.SMSBrandName.Helpers;
using CenIT.Lib.SMSBrandName.Models;
using Core.API.Biz;
using Core.API.Caches;
using Core.API.Enums;
using Core.API.Models;
using Core.Cate.Caches;
using Core.Sys.Caches.Sys;
using Libs.VNPTMoney.Payment.Models;
using Modules.API.Helpers;
using Modules.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using TSFramework.Libs.BaseApps;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Modules.API.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly AccountCache _AccountCache;
        private readonly KWC_AccountMobileCache _KwcAccountMobileCache;
        private readonly KWC_MobileOTPCache _MobileOTPCache;
        private readonly Core.Cate.Caches.Cate_ServiceRequestsCache _ServiceRequestsCache;
        public AuthController()
        {
            _AccountCache = new AccountCache();
            _KwcAccountMobileCache = new KWC_AccountMobileCache();
            _MobileOTPCache = new KWC_MobileOTPCache();
            _ServiceRequestsCache = new Core.Cate.Caches.Cate_ServiceRequestsCache();
        }

        // Kiểm tra tai khoản
        [HttpPost]
        [Route("check-account")]
        public IHttpActionResult CheckAccount(CheckAccountModel model)
        {
            if (!string.IsNullOrEmpty(model.AccountUser))
            { 
                model.AccountUser = model.AccountUser.Trim();
                string _typeAccount = model.TypeAccount;
                var checkResult = _AccountCache.IsExist(new KWC_AccountMobileModel
                {
                    AccountUser = model.AccountUser,
                    TypeAccount = _typeAccount
                });

                var username_test = ConfigurationManager.AppSettings["CONFIG_USERNAME_TEST"] ?? "KHA0001";
                if (model.AccountUser == username_test) checkResult = 1;
                    // Đã có tk
                    if (checkResult == 1)
                {
                    int optId = 0;
                    if (!model.UsingFor.Equals("Sigin")) //Kiểm tra nếu không phải dùng để đăng nhập
                    {
                        if (_typeAccount == "phone") //Nếu thông tin đăng nhập là số điện thoại
                        {
                            string sdt = DinhDangSoDienThoai(model.AccountUser);

                            string _otpValue = UtilString.RandomNumber(6);

                            if (model.UsingFor.Equals("ForgetPass")){
                                SMSAuth(sdt, _otpValue);
                            }

                            optId = _MobileOTPCache.Save(new KWC_MobileOTPModel
                            {
                                AccountUser = model.AccountUser,
                                PhoneNumber = sdt,
                                OTP = _otpValue,
                                DeviceOS = model.DeviceOS,
                                DeviceUUID = model.DeviceUUID,
                                InfoOTP = model.DeviceInfo
                            });
                        }
                    }

                    var phoneMask = DinhDangSoDienThoai(model.AccountUser);
                    if (_typeAccount == "contract")
                    {
                        var serviceRq = _AccountCache.GetInfoContract(model.AccountUser, "");
                        if (serviceRq == null || serviceRq.ContractCode == null)
                        {
                            phoneMask = "";
                        }
                        else
                        {
                            phoneMask = serviceRq.Phone;
                        }
                        
                    }

                    return Json(new
                    {
                        status = EnumRequestStatus.OK,
                        data = new
                        {
                            existsAccountMobile = true,
                            typeAccount = _typeAccount,
                            idOTP = optId, //không đăng nhập qua OTP
                            phoneMask = MaskPhone(DinhDangSoDienThoai(phoneMask))
                        },
                        message = AppProcessor.Messagor.GetMessage("API_Get_Data_Success")
                    });
                }

                // có thông tin nhưng chưa có tài khoản

                if (checkResult == 2)
                {
                    if (_typeAccount == "phone")
                    {
                        string sdt = DinhDangSoDienThoai(model.AccountUser);

                        /* gửi OTP*/
                        string _otpValue = UtilString.RandomNumber(6);
                        SMSAuth(sdt, _otpValue);
                        int optId = _MobileOTPCache.Save(new KWC_MobileOTPModel
                        {
                            AccountUser = model.AccountUser,
                            PhoneNumber = sdt,
                            OTP = _otpValue,
                            DeviceOS = model.DeviceOS,
                            DeviceUUID = model.DeviceUUID,
                            InfoOTP = model.DeviceInfo
                        });
                        return Json(new
                        {
                            status = EnumRequestStatus.OK,
                            data = new
                            {
                                existsAccountMobile = false,
                                typeAccount = _typeAccount,
                                idOTP = optId,
                                phoneMask = MaskPhone(DinhDangSoDienThoai(model.AccountUser))
                            },
                            message = optId > 0 ? AppProcessor.Messagor.GetMessage("API_SendOTP_Success")
                                                : AppProcessor.Messagor.GetMessage("API_Get_Data_Success")
                        });
                    }
                    else
                    {
                           var phoneMask = "";
                            var serviceRq = _AccountCache.GetInfoContract(model.AccountUser, "");
                            if (serviceRq == null || serviceRq.ContractCode == null)
                            {
                            phoneMask = "";
                            }
                            else
                            {
                                phoneMask = serviceRq.Phone;
                            }

                        return Json(new
                        {
                            status = EnumRequestStatus.OK,
                            data = new
                            {
                                existsAccountMobile = false,
                                typeAccount = _typeAccount,
                                idOTP = 0,
                                phoneMask = MaskPhone(DinhDangSoDienThoai(phoneMask))
                            },
                            message = AppProcessor.Messagor.GetMessage("API_Get_Data_Success")
                        });
                    }
                }
            }

            // không có thông tin
            return Json(new
            {
                status = EnumRequestStatus.NOT_FOUND,
                data = new
                {
                    existsAccountMobile = false,
                    typeAccount = "",
                    idOTP = 0
                },
                message = string.Format(AppProcessor.Messagor.GetMessage("Common_DataNotExist"), model.AccountUser ?? "")
            });
        }

        // Xác minh hợp đồng
        [HttpPost]
        [Route("verify-contract")]
        public IHttpActionResult VerifyContract(VerifyContractModel model)
        {
            // kiểm tra mã HĐ
            //var serviceRq = _ServiceRequestsCache.GetByContractCode(model.ContractCode);//_AccountCache
            var serviceRq = _AccountCache.GetInfoContract(model.ContractCode, model.PhoneNumber);
            if (serviceRq == null || serviceRq.ContractCode == null)
            {
                return Json(new
                {
                    status = EnumRequestStatus.NOT_FOUND,
                    data = new { isValid = false, idOTP = 0 },
                    message = string.Format(AppProcessor.Messagor.GetMessage("Common_DataNotExist"), "Mã hợp đồng - " + model.ContractCode)
                });
            }

            //string sdt = DinhDangSoDienThoai(model.PhoneNumber);//Chuyển sang đầu số +84
            string sdt = DinhDangSoDienThoai(serviceRq.Phone);
            /* gửi OTP
            * 
            */
            string _otpValue = UtilString.RandomNumber(6);
            SMSAuth(sdt, _otpValue);

            int optId = _MobileOTPCache.Save(new KWC_MobileOTPModel
            {
                AccountUser = model.ContractCode,
                PhoneNumber = sdt,
                OTP = _otpValue,
                DeviceOS = model.DeviceOS,
                DeviceUUID = model.DeviceUUID,
                InfoOTP = model.DeviceInfo
            });

            return Json(new
            {
                status = EnumRequestStatus.OK,
                data = new { isValid = optId > 0, idOTP = optId },
                message = optId > 0 ? AppProcessor.Messagor.GetMessage("API_SendOTP_Success") : AppProcessor.Messagor.GetMessage("API_Get_Data_Fail")
            });
        }

        // Gửi lại OTP
        [HttpPost]
        [Route("send-otp")]
        public IHttpActionResult SendOtp(SendOTPModel model)
        {
            var otpModel = _MobileOTPCache.GetById(model.IdOTP);
            if (otpModel == null)
            {
                return Json(new
                {
                    status = EnumRequestStatus.BAD_REQUEST,
                    data = new { },
                    message = AppProcessor.Messagor.GetMessage("API_Get_Data_Fail")
                });
            }

            string sdt = DinhDangSoDienThoai(otpModel.PhoneNumber);
            var _otpValue = UtilString.RandomNumber(6);
            SMSAuth(sdt, _otpValue);

            /* gửi OTP
            *
            */

            // kiểm tra mã HĐ
            var optId = _MobileOTPCache.Save(new KWC_MobileOTPModel
            {
                ID = model.IdOTP,
               // OTP = UtilString.RandomNumber(6)
                OTP = _otpValue
            });
            return Json(new
            {
                status = optId > 0 ? EnumRequestStatus.OK : EnumRequestStatus.BAD_REQUEST,
                message = optId > 0 ? AppProcessor.Messagor.GetMessage("API_SendOTP_Success")
                : AppProcessor.Messagor.GetMessage("API_Get_Data_Fail")
            });
        }


        // Xác minh OTP
        [HttpPost]
        [Route("verify-otp")]
        public IHttpActionResult VerifyOtp(VerifyOTPModel model)
        {
            var otp = _MobileOTPCache.GetById(model.IdOTP);
            if (otp == null)
            {
                return Json(new
                {
                    status = EnumRequestStatus.BAD_REQUEST,
                    data = new { },
                    message = AppProcessor.Messagor.GetMessage("API_VerifyOTP_Incorrect")
                });
            }

            if (otp.OTP != model.OTP)
                return Json(new
                {
                    status = EnumRequestStatus.BAD_REQUEST,
                    message = AppProcessor.Messagor.GetMessage("API_VerifyOTP_Incorrect")
                });

            if (otp.ExpirationDate < DateTime.Now)
                return Json(new
                {
                    status = EnumRequestStatus.BAD_REQUEST,
                    message = AppProcessor.Messagor.GetMessage("API_VerifyOTP_Expired")
                });

            return Json(new
            {
                status = EnumRequestStatus.OK,
                message = AppProcessor.Messagor.GetMessage("API_VerifyOTP_Success")
            });
        }


        // Tạo tk
        [HttpPost]
        [Route("reset-password")]
        public IHttpActionResult CreateAccount(CreateAccountModel model)
        {
            var salt = UtilEncrypt.GenerateSalt();
            var passwordHash = UtilEncrypt.GenerateCryptoPassword(model.NewPassword, salt);
            var Account_ID = _AccountCache.Create(new KWC_AccountMobileModel
            {
                AccountUser = model.AccountUser,
                TypeAccount = model.TypeAccount,
                Salt = salt,
                Password = passwordHash,
            });

            var accountInfo = _KwcAccountMobileCache.GetById(Account_ID);

            return Json(new
            {
                status = Account_ID > 0 ? EnumRequestStatus.OK : EnumRequestStatus.BAD_REQUEST,
                data = Account_ID > 0 ? CustomDataApi.RemvovePropertiesAuto(accountInfo, new[] { "Password", "Salt", "LastTimeLogin" }) : new { },
                message = Account_ID > 0 ? AppProcessor.Messagor.GetMessage("API_Get_Data_Success")
                : AppProcessor.Messagor.GetMessage("API_Get_Data_Fail")
            });
        }

        #region function 
        string DinhDangSoDienThoai(string sdt)
        {
            if (string.IsNullOrEmpty(sdt)) return string.Empty;
            if (sdt.StartsWith("+84")) return sdt.Replace("+","");
            if (sdt.StartsWith("0")) return "84" + sdt.Substring(1, sdt.Length - 1);
            else return sdt;
        }

        /// <summary>
        /// Kiểm tra định dạng tài khoản số điện thoại hay là hợp đồng
        /// </summary>
        /// <param name="AccountUser"></param>
        /// <returns></returns>
        string DinhDangTypeAccount(string AccountUser)
        {
            if (string.IsNullOrEmpty(AccountUser)) return "";

            bool _isPhone = false;

            if (AccountUser.Length == 10)
            {
                //Kiểm tra đây là định dạng số điện thoại Việt Nam
                string VietnamPrefixes = new SysConfigCache().GetViaKey("PREFIX_PHONENUMBER_VN").ConfigValue;
                string[] prefixes = VietnamPrefixes.Split(',')
                                          .Select(x => x.Trim())
                                          .Where(x => x != "")
                                          .ToArray();
                _isPhone = UtilString.IsVietnamesePhoneNumber(AccountUser, prefixes);

                if (!_isPhone)
                {
                    return "contract";
                }


            }
            else
            {
                return "contract";
            }

            return _isPhone ? "phone" : "contract";
        }
        public static string MaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length <= 3)
                return phone;

            return new string('*', phone.Length - 3) + phone.Substring(phone.Length - 3);
        }



        #endregion

        public void SMSAuth(string sdt, string _otpValue)
        {
            #region Send SMS BrandName
            int isSendSMS = int.Parse(ConfigurationManager.AppSettings["SMS_BRANDNAME_ISSENDSMS"]);
            int minuteExpiredOTP = int.Parse(ConfigurationManager.AppSettings["SMS_BRANDNAME_MINUTEEXPIREDOTP"]);
            if (isSendSMS == 1)
            {

                RequestSendSMSListModel _sendSMSModel = new RequestSendSMSListModel();
                DataRequestSendSMSListModel _RQSTData = new DataRequestSendSMSListModel();

                #region Params
                List<DataRequestRaramsSendSMSListModel> _params = new List<DataRequestRaramsSendSMSListModel>();
                DataRequestRaramsSendSMSListModel _p = new DataRequestRaramsSendSMSListModel();
                _p.NUM = "1";
                _p.CONTENT = UtilString.RemoveSign4VietnameseString($"{_otpValue}");
                _params.Add(_p);
                DataRequestRaramsSendSMSListModel _p1 = new DataRequestRaramsSendSMSListModel();
                _p1.NUM = "2";
                _p1.CONTENT = UtilString.RemoveSign4VietnameseString($"{minuteExpiredOTP}");
                _params.Add(_p1);
                #endregion

                _RQSTData.AGENTID = ConfigurationManager.AppSettings["SMS_BRANDNAME_AGENTID"];
                _RQSTData.APIPASS = ConfigurationManager.AppSettings["SMS_BRANDNAME_APIPASS"];
                _RQSTData.APIUSER = ConfigurationManager.AppSettings["SMS_BRANDNAME_APIUSER"];
                _RQSTData.CONTRACTID = ConfigurationManager.AppSettings["SMS_BRANDNAME_CONTRACTID"];
                _RQSTData.CONTRACTTYPEID = ConfigurationManager.AppSettings["SMS_BRANDNAME_CONTRACTTYPEID"];
                _RQSTData.DATACODING = "0";
                _RQSTData.ISTELCOSUB = "0";
                _RQSTData.LABELID = ConfigurationManager.AppSettings["SMS_BRANDNAME_LABELID"];
                _RQSTData.MOBILELIST = sdt;
                _RQSTData.PACKAGEID = DateTime.Now.ToString("ddMMyyyyHHmmss");
                _RQSTData.PARAMS = _params;
                _RQSTData.REQID = DateTime.Now.ToString("ddMMyyyyHHmmss");
                _RQSTData.SALEORDERID = DateTime.Now.ToString("ddMMyyyyHHmmss");
                _RQSTData.SCHEDULETIME = "";
                _RQSTData.TEMPLATEID = ConfigurationManager.AppSettings["SMS_BRANDNAME_TEMPLATEID"];
                _RQSTData.USERNAME = ConfigurationManager.AppSettings["SMS_BRANDNAME_USERNAME"];

                _sendSMSModel.RQST = _RQSTData;

                APIHelper _apiHelper = new APIHelper();
                ResponseSendSMSList _response = _apiHelper.CallAPI<ResponseSendSMSList>(JsonConvert.SerializeObject(_sendSMSModel));
                // Lưu nội dung SMS vô DB
                new KWC_MobileOTPBiz().AddLogCallAPI(new QrCodeReqCallAPILogModel()
                {
                    APINameOrURL = "SMS BrandName",
                    Request = _RQSTData.ToJson(),
                    Response = _response.ToJson()

                });
            }
            #endregion
        }
    }
}