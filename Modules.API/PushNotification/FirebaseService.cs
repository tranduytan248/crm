using Google.Apis.Auth.OAuth2;
using Modules.API.PushNotification.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Modules.API.Services
{
    public class FirebaseService
    {
        private readonly string _projectId;
        private readonly string _serviceAccountPath;

        public FirebaseService(string projectId, string fileConfigJSON)
        {
            // 🔸 Đọc cấu hình từ file (bạn có thể thay bằng appsettings hoặc web.config)
            _projectId = projectId == null?"cskh-khawassco-app": projectId; // 👉 thay bằng Project ID của bạn
            _serviceAccountPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", fileConfigJSON == null?"cskh-khawassco-app-firebase-adminsdk-fbsvc-2335b9910d.json": fileConfigJSON);
        }

        public async Task<FirebaseResult> SendNotificationWithResultAsync(string deviceToken, string title, string body, string deviceUUID = "", object dataSend = null)
        {
            string json = "";
            FirebaseResult resultObj = new FirebaseResult();
            try
            {
                // 🔸 Lấy access token từ file JSON của Firebase
                GoogleCredential credential = GoogleCredential.FromFile(_serviceAccountPath)
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

                string accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

                // 🔸 Tạo nội dung JSON gửi lên Firebase
                var message = new
                {
                    message = new
                    {
                        token = deviceToken,
                        notification = new
                        {
                            title = title,
                            body = body
                        },
                        // Thêm trường DATA TÙY CHỈNH tại đây
                        data = dataSend
                    }
                    
                };

                //json = JsonSerializer.Serialize(message);
                json = message.ToJson();
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string url = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";

                // 🔸 Gửi HTTP POST
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await client.PostAsync(url, content);
                    var result = await response.Content.ReadAsStringAsync();

                    StringBuilder logBuilder = new StringBuilder();
                    logBuilder.AppendLine("=============================================");
                    logBuilder.AppendLine($" * Result Push Notification Firebase ");
                    logBuilder.AppendLine($" => Device:{deviceUUID} ");
                    logBuilder.AppendLine($" => Input:{json} ");
                    logBuilder.AppendLine($" => Output:{result} ");
                    logBuilder.AppendLine("=============================================");
                    AppProcessor.Logger.Error(new Exception(logBuilder.ToString()));

                    //=========================================
                    
                    if (response.IsSuccessStatusCode)
                    {
                        // Firebase trả về JSON dạng: { "name": "projects/.../messages/123" }
                        if (result.Contains("\"name\""))
                        {
                            resultObj.IsSuccess = true;
                            resultObj.MessageId = UtilString.RandomNumber(30);
                            resultObj.Token = deviceToken;
                            resultObj.RawResponse = result.ToString();
                            resultObj.DeviceUUID = deviceUUID;
                        }
                    }
                    else
                    {
                        // Parse lỗi nếu có
                        if (result.Contains("error"))
                        {
                            var start = result.IndexOf("\"status\":") + 10;
                            var end = result.IndexOf("\"", start);
                            if (end > start)
                            {
                                resultObj.ErrorCode = result.Substring(start, end - start);
                            }
                            
                            resultObj.MessageId = UtilString.RandomNumber(30);
                            resultObj.RawResponse = result.ToString();
                            resultObj.Token = deviceToken;
                            resultObj.DeviceUUID = deviceUUID;
                        }
                    }

                    return resultObj;
                }
            }
            catch (Exception ex)
            {
                StringBuilder logBuilder = new StringBuilder();
                logBuilder.AppendLine("=============================================");
                logBuilder.AppendLine($" * Error Push Notification Firebase ");
                logBuilder.AppendLine($" => Device:{deviceUUID} ");
                logBuilder.AppendLine($" => Input:{json} ");
                logBuilder.AppendLine($" => Output:{ex.Message} ");
                logBuilder.AppendLine("=============================================");
                AppProcessor.Logger.Error(new Exception(logBuilder.ToString()));

                resultObj.IsSuccess = false;
                resultObj.Token = deviceToken;
                resultObj.ErrorCode = ex.Message;
                resultObj.DeviceUUID = deviceUUID;
                return resultObj;
            }
        }

        public async Task<bool> SendNotificationAsync(string deviceToken, string title, string body, string deviceUUID = "", object dataSend = null)
        {
            string json = "";
            try
            {
                // 🔸 Lấy access token từ file JSON của Firebase
                GoogleCredential credential = GoogleCredential.FromFile(_serviceAccountPath)
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

                string accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

                // 🔸 Tạo nội dung JSON gửi lên Firebase
                var message = new
                {
                    message = new
                    {
                        token = deviceToken,
                        notification = new
                        {
                            title = title,
                            body = body
                        },
                         // Thêm trường DATA TÙY CHỈNH tại đây
                    data = dataSend
                    }
                };

                //json = JsonSerializer.Serialize(message);
                json = message.ToJson();
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string url = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";

                // 🔸 Gửi HTTP POST
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await client.PostAsync(url, content);
                    var result = await response.Content.ReadAsStringAsync();

                    StringBuilder logBuilder = new StringBuilder();
                    logBuilder.AppendLine("=============================================");
                    logBuilder.AppendLine($" * Result Push Notification Firebase ");
                    logBuilder.AppendLine($" => Device:{deviceUUID} ");
                    logBuilder.AppendLine($" => Input:{json} ");
                    logBuilder.AppendLine($" => Output:{result} ");
                    logBuilder.AppendLine("=============================================");
                    AppProcessor.Logger.Error(new Exception(logBuilder.ToString()));
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                StringBuilder logBuilder = new StringBuilder();
                logBuilder.AppendLine("=============================================");
                logBuilder.AppendLine($" * Error Push Notification Firebase ");
                logBuilder.AppendLine($" => Device:{deviceUUID} ");
                logBuilder.AppendLine($" => Input:{json} ");
                logBuilder.AppendLine($" => Output:{ex.Message} ");
                logBuilder.AppendLine("=============================================");
                AppProcessor.Logger.Error(new Exception(logBuilder.ToString()));
                return false;
            }
        }
    }
}
