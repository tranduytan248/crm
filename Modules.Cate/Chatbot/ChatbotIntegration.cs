using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Modules.Cate.Chatbot
{
    public sealed class ChatbotToolRequest
    {
        public int Version { get; set; }
        public string BotId { get; set; }
        public string SubjectId { get; set; }
        public string ToolCallId { get; set; }
        public string ToolName { get; set; }
        public JObject Input { get; set; }
    }

    public sealed class ChatbotToolException : Exception
    {
        public HttpStatusCode Status { get; private set; }
        public string Code { get; private set; }
        public ChatbotToolException(HttpStatusCode status, string code, string message) : base(message)
        { Status = status; Code = code; }
    }

    public sealed class ChatbotCapability
    {
        public string SubjectId { get; set; }
        public string BotId { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }

    public static class ChatbotIntegration
    {
        // Capability is opaque to Chatbot BE. Both endpoints run in the same ASP.NET application.
        // A web farm must share an explicit machineKey; tokens are purpose-bound to this executor.
        private const string Purpose = "CRM.Chatbot.Tools.v1";
        public static string BotId => Required("Chatbot_BotId");
        public static string ApiKey => Required("Chatbot_ApiKey");
        public static string ViewerSessionUrl => Required("Chatbot_BaseUrl").TrimEnd('/')
            + "/public/bots/" + Uri.EscapeDataString(BotId) + "/viewer-session";

        private static string Required(string key)
        {
            var value = Environment.GetEnvironmentVariable(key) ?? ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value)) throw new ConfigurationErrorsException("Missing " + key);
            return value;
        }

        public static string IssueCapability(string subjectId, out DateTimeOffset expiresAt)
        {
            if (string.IsNullOrWhiteSpace(subjectId)) throw new ArgumentException("subjectId is required.");
            expiresAt = DateTimeOffset.UtcNow.AddMinutes(15);
            var data = new ChatbotCapability { SubjectId = subjectId, BotId = BotId, ExpiresAt = expiresAt };
            return HttpServerUtility.UrlTokenEncode(MachineKey.Protect(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)), Purpose));
        }

        public static bool TryReadCapability(string token, out ChatbotCapability capability)
        {
            capability = null;
            if (string.IsNullOrWhiteSpace(token) || token.Length > 8192) return false;
            try
            {
                var bytes = HttpServerUtility.UrlTokenDecode(token);
                if (bytes == null) return false;
                var clear = MachineKey.Unprotect(bytes, Purpose);
                if (clear == null) return false;
                var value = JsonConvert.DeserializeObject<ChatbotCapability>(Encoding.UTF8.GetString(clear));
                if (value == null || string.IsNullOrWhiteSpace(value.SubjectId)
                    || value.BotId != BotId || value.ExpiresAt <= DateTimeOffset.UtcNow) return false;
                capability = value;
                return true;
            }
            catch (System.Security.Cryptography.CryptographicException) { return false; }
            catch (ArgumentException) { return false; }
            catch (FormatException) { return false; }
            catch (JsonException) { return false; }
        }
    }
}
