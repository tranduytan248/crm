using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TSFramework.Libs
{
    public class DialogflowProvider
    {
        private readonly string projectId; // ID của dự án Google Cloud
        private readonly string credentialsPath; // Đường dẫn tới tệp JSON chứa thông tin xác thực
        private readonly HttpClient httpClient; // Đối tượng HttpClient để thực hiện các yêu cầu HTTP

        public DialogflowProvider(string projectId, string credentialsPath)
        {
            this.projectId = projectId; // Khởi tạo giá trị projectId
            this.credentialsPath = credentialsPath; // Khởi tạo giá trị credentialsPath
            this.httpClient = new HttpClient(); // Tạo một đối tượng HttpClient mới
        }

        // Phương thức lấy access token cần thiết để xác thực với các API của Google
        private async Task<string> GetAccessTokenAsync()
        {
            // Tải thông tin xác thực của Google từ tệp và tạo một credential có phạm vi
            var googleCredential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(credentialsPath)
                .CreateScoped("https://www.googleapis.com/auth/cloud-platform");

            // Lấy access token
            var token = await googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }

        public async Task<JObject> DetectIntentAsync(string text, string languageCode)
        {
            // Kiểm tra nếu văn bản đầu vào là rỗng hoặc null
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text is required", nameof(text)); // Ném ngoại lệ nếu văn bản bị thiếu
            }

            // Tạo một session ID mới cho yêu cầu
            var sessionId = Guid.NewGuid().ToString();
            // Xây dựng URL của API sử dụng project ID và session ID
            var url = $"https://dialogflow.googleapis.com/v2/projects/{projectId}/agent/sessions/{sessionId}:detectIntent";

            // Tạo đối tượng JSON cho đầu vào truy vấn
            var queryInput = new JObject
            {
                ["text"] = new JObject
                {
                    ["text"] = text,
                    ["languageCode"] = languageCode
                }
            };

            // Tạo đối tượng JSON cho thân yêu cầu
            var requestBody = new JObject
            {
                ["queryInput"] = queryInput
            };

            // Lấy access token
            var accessToken = await GetAccessTokenAsync();
            // Đặt tiêu đề xác thực với access token
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Tạo nội dung HTTP với đối tượng JSON của thân yêu cầu
            var content = new StringContent(requestBody.ToString(), Encoding.UTF8, "application/json");
            // Gửi yêu cầu POST đến API Dialogflow
            var response = await httpClient.PostAsync(url, content);

            // Kiểm tra nếu phản hồi không thành công
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error calling Dialogflow API: {response.ReasonPhrase}"); // Ném ngoại lệ nếu có lỗi xảy ra
            }

            // Đọc nội dung phản hồi
            var responseContent = await response.Content.ReadAsStringAsync();
            // Chuyển đổi nội dung phản hồi thành đối tượng JSON
            var jsonResponse = JObject.Parse(responseContent);

            // Lấy kết quả từ phản hồi JSON
            var result = jsonResponse["queryResult"];

            // Trích xuất fulfillmentText
            var fulfillmentText = result["fulfillmentText"]?.ToString() ?? string.Empty;

            // Trích xuất các tùy chọn lựa chọn từ fulfillmentMessages
            var quickReplies = new List<string>();
            var fulfillmentMessages = result["fulfillmentMessages"] as JArray;

            if (fulfillmentMessages != null)
            {
                foreach (var message in fulfillmentMessages)
                {
                    if (message != null)
                    {
                        var payload = message["payload"];
                        if (payload != null)
                        {
                            var richContent = payload["richContent"];
                            if (richContent != null && richContent is JArray)
                            {
                                foreach (var richContentItem in (JArray)richContent)
                                {
                                    foreach (var item in richContentItem)
                                    {
                                        if (item["type"]?.ToString() == "chips")
                                        {
                                            var options = item["options"] as JArray;
                                            if (options != null)
                                            {
                                                foreach (var option in options)
                                                {
                                                    var optionText = option["text"]?.ToString();
                                                    if (!string.IsNullOrEmpty(optionText))
                                                    {
                                                        quickReplies.Add(optionText);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Tạo và trả về đối tượng JSON kết quả
            var resultObject = new JObject
            {
                { "responseId", jsonResponse["responseId"] },
                { "queryText", result["queryText"] },
                { "fulfillmentText", fulfillmentText },
                { "fulfillmentMessages", fulfillmentMessages },
                { "quickReplies", JArray.FromObject(quickReplies) }
            };

            return resultObject;
        }
    }
}
