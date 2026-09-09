using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Chatbot
{
    [RoutePrefix("api/Chatbot")]
    public sealed class ChatbotToolsController : ApiController
    {
        // Keep legacy synchronous DB work bounded even when the HTTP request times out.
        private static readonly SemaphoreSlim Slots = new SemaphoreSlim(4);
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        { ContractResolver = new CamelCasePropertyNamesContractResolver(), DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind };

        [AllowAnonymous]
        [HttpPost, Route("Tools")]
        public async Task<HttpResponseMessage> Tools([FromBody] ChatbotToolRequest request, CancellationToken cancellationToken)
        {
            if (HttpContext.Current != null) HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;
            try
            {
                IEnumerable<string> keys;
                var idempotencyKeys = Request.Headers.TryGetValues("Idempotency-Key", out keys) ? keys.ToArray() : new string[0];
                if (!ModelState.IsValid || request == null || request.Version != 1 || string.IsNullOrWhiteSpace(request.ToolCallId)
                    || request.ToolCallId.Length > 200 || idempotencyKeys.Length != 1 || idempotencyKeys[0] != request.ToolCallId)
                    return Error(HttpStatusCode.BadRequest, "invalid_request", "Invalid request or Idempotency-Key.");
                if (request.BotId != ChatbotIntegration.BotId || !ChatbotToolService.AllowedTools.Contains(request.ToolName))
                    return Error(HttpStatusCode.Forbidden, "tool_not_allowed", "Bot or tool is not allowed.");
                var auth = Request.Headers.Authorization;
                ChatbotCapability capability;
                if (auth == null || !string.Equals(auth.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase)
                    || !ChatbotIntegration.TryReadCapability(auth.Parameter, out capability)
                    || capability.SubjectId != request.SubjectId || capability.BotId != request.BotId)
                    return Error(HttpStatusCode.Unauthorized, "invalid_capability", "Capability is invalid or expired. Create a new viewer session.");
                var input = ChatbotToolInput.Parse(request.ToolName, request.Input);
                if (!await Slots.WaitAsync(0, cancellationToken))
                    return Error(HttpStatusCode.ServiceUnavailable, "busy", "Tool executor is busy. Please retry later.");
                var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                deadline.CancelAfter(TimeSpan.FromSeconds(12));
                var worker = Task.Run(() =>
                {
                    try { return new ChatbotToolService().Execute(request.ToolName, capability.SubjectId, input, deadline.Token); }
                    finally { Slots.Release(); }
                });
                var cleanup = worker.ContinueWith(t => { var ignored = t.Exception; deadline.Dispose(); }, TaskScheduler.Default);
                if (await Task.WhenAny(worker, Task.Delay(TimeSpan.FromSeconds(12), cancellationToken)) != worker)
                    return Error(HttpStatusCode.GatewayTimeout, "tool_timeout", "Report exceeded the time budget. Narrow the filters and retry.");
                var result = await worker;
                var json = JsonConvert.SerializeObject(new { result }, JsonSettings);
                if (Encoding.UTF8.GetByteCount(json) > 256 * 1024)
                    return Error(HttpStatusCode.BadRequest, "result_too_large", "Result exceeds 256 KB. Reduce limit or narrow filters.");
                return JsonResponse(HttpStatusCode.OK, json);
            }
            catch (ChatbotToolException ex) { return Error(ex.Status, ex.Code, ex.Message); }
            catch (OperationCanceledException) { return Error(HttpStatusCode.GatewayTimeout, "tool_timeout", "Tool execution timed out or was cancelled."); }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
                return Error(HttpStatusCode.InternalServerError, "tool_failed", "Unable to query CRM data.");
            }
        }

        private static HttpResponseMessage Error(HttpStatusCode status, string code, string message) =>
            JsonResponse(status, JsonConvert.SerializeObject(new { error = new { code, message } }));
        private static HttpResponseMessage JsonResponse(HttpStatusCode status, string json)
        {
            var response = new HttpResponseMessage(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
            response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoStore = true };
            return response;
        }
    }
}
