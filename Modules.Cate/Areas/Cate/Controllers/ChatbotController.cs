using Core.Cate.Caches;
using Core.Sys.BaseApp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using System.Configuration;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class ChatbotController : AppController
    {
        private readonly ChatbotCache _chatbotCache;
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _baseUrl = ConfigurationManager.AppSettings["Chatbot_BaseUrl"];
        private static readonly string _apiKey = ConfigurationManager.AppSettings["Chatbot_ApiKey"];
        private static readonly string _botId = ConfigurationManager.AppSettings["Chatbot_BotId"];

        public ChatbotController()
        {
            _chatbotCache = new ChatbotCache();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Session()
        {
            try
            {
                var subjectId = User?.UserName;
                if (string.IsNullOrWhiteSpace(subjectId))
                    return new HttpUnauthorizedResult();

                var data = _chatbotCache.GetData(subjectId);

                var body = new
                {
                    subjectId,
                    data
                };

                var url = $"{_baseUrl}/public/bots/{_botId}/viewer-session";

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("x-api-key", _apiKey);
                request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadGateway, "Unable to create chatbot session.");
                }

                var payload = JObject.Parse(responseContent);
                var viewerSession = payload["data"] as JObject ?? payload;

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();

                return Json(new
                {
                    viewerToken = (string)viewerSession["viewerToken"],
                    expiresAt = (string)viewerSession["expiresAt"]
                });
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(new Exception($"[Chatbot Session] Create viewer session failed. {ex}", ex));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}