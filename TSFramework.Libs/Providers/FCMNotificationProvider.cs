using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace TSFramework.Libs.Providers
{
    //public class FCMNotificationProvider
    //{
    //    private string FCM_SERVER_API_URL = "https://fcm.googleapis.com/fcm/send";
    //    private string FCM_SERVER_API_KEY = "AAAAEDyLiFk:APA91bGe5-NsCfegNnDOWo3xcqK6KmMuxemOK-Ma-ExlOZaViwcJyk_dlPl64S1RH1Un6_X-LoO67zKrySz9XgA53uS9cuhzNsM7fh7xFMIvW9ghFnpLyvZ7b1h95CO8N7OktRqXrihH";
    //    private string FCM_SENDER_ID = "69735254105";
    //    public FCMNotificationProvider(string fmAPIURL, string fcmAPIKey, string fcmSender) {
    //        this.FCM_SERVER_API_URL = fmAPIURL;
    //        this.FCM_SERVER_API_KEY = fcmAPIKey;
    //        this.FCM_SENDER_ID = fcmSender;
    //    }

    //    /// <summary>
    //    ///     Call FCM API to push notify to one or multiple devices
    //    /// </summary>
    //    /// <param name="deviceTokens">List devices token</param>
    //    /// <param name="title">Title notify</param>
    //    /// <param name="body">Body notify</param>
    //    /// <param name="data">Data attach: {key : value, key2 : value2}</param>
    //    /// <returns></returns>
    //    public async Task<bool> PushToDevices(string[] deviceTokens, string title, string body, object data)
    //    {
    //        if (!deviceTokens.Any()) return false;
    //        //Object creation

    //        var messageInformation = new FCMMessageModel
    //        {
    //            notification = new FCMNotificationModel
    //            {
    //                title = title,
    //                text = body
    //            },
    //            data = data,
    //            registration_ids = deviceTokens
    //        };

    //        //Object to JSON STRUCTURE => using Newtonsoft.Json;
    //        var jsonMessage = JsonConvert.SerializeObject(messageInformation);

    //        /*
    //             ------ JSON STRUCTURE ------
    //             {
    //                notification: {
    //                                title: "",
    //                                text: ""
    //                                },
    //                data: {
    //                        action: "Play",
    //                        playerId: 5
    //                        },
    //                registration_ids = ["id1", "id2"]
    //             }
    //             ------ JSON STRUCTURE ------
    //             */

    //        //Create request to Firebase API
    //        var request = new HttpRequestMessage(HttpMethod.Post, FCM_SERVER_API_URL);

    //        request.Headers.TryAddWithoutValidation("Authorization", "key=" + FCM_SERVER_API_KEY);
    //        request.Headers.TryAddWithoutValidation("Sender", "id=" + FCM_SENDER_ID);

    //        request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

    //        using (var client = new HttpClient())
    //        {
    //            await client.SendAsync(request);
    //        }

    //        return false;
    //    }
    //}

    public class FirebaseAdminProgram
    {
        readonly string _filePath;
        public FirebaseAdminProgram(string filePath) {
            _filePath = filePath;
        }
        public async Task PushToDevices(List<string> deviceTokens, string title, string content)
        {
            // Khởi tạo FirebaseApp bằng tệp JSON chứa thông tin xác thực
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(_filePath),
                });
            }

            // Gọi hàm để gửi thông báo
            foreach (var token in deviceTokens)
            {
                //await SendMessageAsync(t, title, content);
                //await Task.Delay(3000);

                try
                {
                    if (token != null && (string.IsNullOrEmpty(token)))
                        continue;

                    var message = new Message()
                    {
                        Notification = new Notification
                        {
                            Title = title,
                            Body = content,
                        },
                        Token = token,
                    };

                    var messaging = FirebaseMessaging.DefaultInstance;
                    var response = await messaging.SendAsync(message).ConfigureAwait(true);
                }
                catch (FirebaseMessagingException ex)
                {
                    // Xử lý ngoại lệ từ dịch vụ Firebase Messaging nếu cần
                    Console.WriteLine($"Firebase Messaging Exception: {ex.Message}");
                    // Bỏ qua lỗi và tiếp tục với phần tử tiếp theo trong danh sách token
                    continue;
                }
                catch (Exception ex)
                {
                    // Xử lý các ngoại lệ khác nếu có
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                Thread.Sleep(1000);
            }
        }

        static async Task SendMessageAsync(string token, string title, string content)
        {
            // Tạo thông báo
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = title,
                    Body = content,
                },
                Token = token,
            };

            // Gửi thông báo
            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine("Successfully sent message: " + response);
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine("Error sending message: " + ex.Message);
            }
        }

        public class FirebaseNotificationUtils
        {
            public async Task SendNotificationToSingleDeviceAsync(string notificationTitle, string notificationBody, List<string> tokens)
            {
                foreach (var token in tokens)
                {
                    try
                    {
                        if (token != null && (string.IsNullOrEmpty(token)))
                            continue;

                        var message = new Message()
                        {
                            Notification = new Notification
                            {
                                Title = notificationTitle,
                                Body = notificationBody,
                            },
                            Token = token,
                        };

                        var messaging = FirebaseMessaging.DefaultInstance;
                        var response = await messaging.SendAsync(message);
                    }
                    catch (FirebaseMessagingException ex)
                    {
                        // Xử lý ngoại lệ từ dịch vụ Firebase Messaging nếu cần
                        Console.WriteLine($"Firebase Messaging Exception: {ex.Message}");
                        // Bỏ qua lỗi và tiếp tục với phần tử tiếp theo trong danh sách token
                        continue;
                    }
                    catch (Exception ex)
                    {
                        // Xử lý các ngoại lệ khác nếu có
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }
        }
    }
}
