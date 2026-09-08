using Core.Cate.Biz;
using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Cate.PushNotification.Models;
using Core.Cate.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Providers;
using TSFramework.Libs.Utils;

namespace Core.Cate.Process
{
    public class NotificationProcess
    {
        private readonly string _projectId = ConfigurationManager.AppSettings["projectIdFirebase"];
        private readonly string _fileConfigJSON = ConfigurationManager.AppSettings["fileConfigJSONFirebase"];

        public void Send(string title, string message, object jsonData)
        {
            var lst_token = new KWC_NotificationBiz().GetLastDeviceLogin();
            var firebase = new FirebaseService(_projectId, _fileConfigJSON);

            // Tạo danh sách Task gửi thông báo
            var tasks = lst_token.Select(item =>
                firebase.SendNotificationWithResultAsync(item.DeviceToken, title, message, item.DeviceUUID, jsonData)
            );

            // Chạy tất cả async song song
            Task.Run(async () =>
            {
                await Task.WhenAll(tasks);
            });
        }

        public async Task sendNotifyForAccountUser(string title, string message, object jsonData, string AccountUser, string Category = "Thanh toán", string data_id = null)
        {
            var lst_token = new KWC_NotificationBiz().GetLastDeviceLoginByAccountUser(AccountUser);
            if (lst_token == null) {
                AppProcessor.Logger.Message($"{AccountUser} Không có thông tin để gửi thông báo");
                return;
            }
            var firebase = new FirebaseService(_projectId, _fileConfigJSON);

            // Gửi thông báo và đợi kết quả
            var result = await firebase.SendNotificationWithResultAsync(
                lst_token.DeviceToken,
                title,
                message,
                lst_token.DeviceUUID,
                jsonData
            );

            if (result.IsSuccess)
            {
                new KWC_NotificationBiz().Save(new KWC_NotificationModel()
                {
                    Title = title,
                    Content = message,
                    Summary = message,
                    Category = Category,
                    URL = Category == "Thanh toán" ? null : Category == "Tiến độ yêu cầu" ? $"khawassco://screen/progressTracking?id={data_id}" : $"khawassco://screen/customerSupportScreen?id={data_id}",
                    UserAccount = lst_token.AccountUser,
                    DeviceUUID = lst_token.DeviceUUID,
                });
            }
        }



        #region Phục vụ cho thông báo cước
        public async Task<FirebaseResult> SendNotifyADevice(string title, string message, object jsonData, string deviceUUID, string deviceToken)
        {
            var firebase = new FirebaseService(_projectId, _fileConfigJSON);

            var res = await firebase.SendNotificationWithResultAsync(deviceToken, title, message, deviceUUID, jsonData);

            return res;
        }
        public async Task SendAllDevice(DateTime period, string username)
        {
            var lstToken = new KWC_NotificationBiz().GetAll(period);
            var firebase = new FirebaseService(_projectId, _fileConfigJSON);

            var tasks = lstToken.Select(async item =>
            {
                var info = new KWC_NotificationBiz().GetClockRecordPeriod(item.ContractCode, item.Period);
                if (info == null) return;
                string amountVND = string.Format("{0:N0} VNĐ", item.Amount);
                string body =
                    $"CTNKH TB: K.H. {info.CustomerName} có hợp đồng {item.ContractCode}, " +
                    $"tháng {item.Period:MM/yyyy} sử dụng {info.Consumption} m3, " +
                    $"số tiền thanh toán {amountVND}, Trân trọng!";

                var jsonData = new
                {
                    data_id = item.ContractCode.ToString() + period.ToString("MM/yyyy"),
                    screen_name = "NotificationScreen",
                    summary = body
                };
                // Gửi thông báo
                var result = await firebase.SendNotificationWithResultAsync(
                    item.DeviceToken,
                    "Thông báo hóa đơn nước đến kỳ thanh toán",
                    body,
                    item.DeviceUUID,
                    jsonData
                );

                // Lưu log
                new KWC_NotificationBiz().Save_NotificationLog(new KWC_Notification_FareNotice_LogModel()
                {
                    ContractCode = item.ContractCode,
                    Period = item.Period,
                    Request = $"Thông báo hóa đơn: {body} , {jsonData}, {item.DeviceUUID}, {item.DeviceToken}",
                    Response = result.ToJson(),
                    IsSuccess = result.IsSuccess
                }, username);

                // Lưu thông báo
                if (result.IsSuccess)
                {
                    new KWC_NotificationBiz().Save(new KWC_NotificationModel()
                    {
                        Title = "Thông báo hóa đơn nước đến kỳ thanh toán",
                        Content = body,
                        Summary = body,
                        Category = "Thanh toán nước",
                        UserAccount = item.AccountUser,
                        DeviceUUID = item.DeviceUUID,
                    });
                }
            });

            // Chạy song song và đợi tất cả
            await Task.WhenAll(tasks);
        }

        public (int ios, int android) countDeviceIOSAndroidUnPaid(DateTime Period)
        {
            var lstDevive = new KWC_NotificationBiz().GetAll(Period);

            int ios = lstDevive.Count(x => x.DeviceOS == "iOS");
            int android = lstDevive.Count(x => x.DeviceOS == "Android");

            return (ios, android);
        }
        #endregion

        public (int ios, int android) countDeviceIOSAndroid()
        {
            var lstDevive = new KWC_NotificationBiz().GetLastDeviceLogin();

            int ios = lstDevive.Count(x => x.DeviceOS == "iOS");
            int android = lstDevive.Count(x => x.DeviceOS == "Android");

            return (ios, android);
        }

        

    }

}
