using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Core.Sys.BaseApp;
using Modules.Cate.Chatbot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class ChatbotController : AppController
    {
        private static readonly HttpClient Client = new HttpClient { Timeout = TimeSpan.FromSeconds(12) };

        // Avoid MVC login redirects: authenticate explicitly and return a real 401.
        [AllowAnonymous]
        [HttpPost]
        public new async Task<ActionResult> Session()
        {
            Response.SuppressFormsAuthenticationRedirect = true;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            if (User?.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(User.UserName))
                return new HttpUnauthorizedResult();
            try
            {
                DateTimeOffset expiresAt;
                var capability = ChatbotIntegration.IssueCapability(User.UserName, out expiresAt);
                var body = new { subjectId = User.UserName, toolCapability = new { token = capability, expiresAt } };
                using (var request = new HttpRequestMessage(HttpMethod.Post, ChatbotIntegration.ViewerSessionUrl))
                {
                    request.Headers.Add("x-api-key", ChatbotIntegration.ApiKey);
                    request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                    using (var response = await Client.SendAsync(request))
                    {
                        if (!response.IsSuccessStatusCode)
                            return new HttpStatusCodeResult(HttpStatusCode.BadGateway, "Unable to create chatbot session.");
                        var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                        var session = payload["data"] as JObject;
                        DateTimeOffset viewerExpiresAt;
                        if (session == null
                            || string.IsNullOrWhiteSpace((string)session["viewerToken"])
                            || !DateTimeOffset.TryParse(
                                (string)session["expiresAt"],
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                                out viewerExpiresAt)
                            || viewerExpiresAt <= DateTimeOffset.UtcNow
                        )
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadGateway, "Chatbot returned an invalid or expired viewer session.");
                        }
                        return Json(new { viewerToken = (string)session["viewerToken"], expiresAt = (string)session["expiresAt"] });
                    }
                }
            }
            catch (TaskCanceledException)
            { return new HttpStatusCodeResult(HttpStatusCode.BadGateway, "Chatbot viewer-session request timed out."); }
            catch (JsonException)
            { return new HttpStatusCodeResult(HttpStatusCode.BadGateway, "Chatbot returned an invalid viewer session."); }
            catch (HttpRequestException)
            { return new HttpStatusCodeResult(HttpStatusCode.BadGateway, "Unable to reach chatbot service."); }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Unable to create chatbot session.");
            }
        }
    }
}
