using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Core.Cate.Models;
using Modules.Cate.Chatbot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TSFramework.Libs.Models.Base;

public static class ChatbotTests
{
    private static int passed;
    public static int Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
        {
            var path = Path.Combine(args[0], new AssemblyName(e.Name).Name + ".dll");
            return File.Exists(path) ? Assembly.LoadFrom(path) : null;
        };
        try { Run(); Console.WriteLine("PASS: " + passed + " checks (no database/network calls)."); return 0; }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }

    private static void Check(bool value, string message)
    { if (!value) throw new Exception(message); passed++; }
    private static ChatbotToolInput Input(string name, string json) => ChatbotToolInput.Parse(name, JObject.Parse(json));
    private static JObject Execute(FakeData data, string tool, string json) => JObject.FromObject(new ChatbotToolService(data).Execute(tool, "alice", Input(tool, json), CancellationToken.None));
    private static void Reject(Action action, string code)
    {
        try { action(); throw new Exception("Expected rejection: " + code); }
        catch (ChatbotToolException ex) { Check(ex.Code == code, "Wrong rejection: " + ex.Code); }
    }

    private static void Run()
    {
        Environment.SetEnvironmentVariable("Chatbot_BotId", "test-bot");
        foreach (var tool in ChatbotToolService.AllowedTools)
        {
            Reject(() => Input(tool, "{\"subjectId\":\"admin\"}"), "invalid_input");
            Reject(() => Input(tool, "{\"limit\":51}"), "invalid_input");
            Reject(() => Input(tool, "{\"limit\":1.5}"), "invalid_input");
        }
        Reject(() => Input("get_project_detail", "{}"), "invalid_input");
        Reject(() => Input("get_opportunity_detail", "{\"id\":1,\"keyword\":\"ABC\"}"), "invalid_input");
        Reject(() => Input("get_project_summary", "{\"year\":\"2026\"}"), "invalid_input");
        Check(Input("get_project_summary", "{}").Limit == 10, "Default page size");

        var data = new FakeData();
        var result = Execute(data, "get_project_summary", "{\"limit\":1}");
        Check((int)result["summary"]["totalProjects"] == 2 && result["items"].Count() == 1 && (bool)result["hasMore"], "Aggregate must precede pagination");
        Check(data.LastSubject == "alice", "Authenticated username must reach list query");
        result = Execute(data, "get_project_summary", "{\"customerId\":8}");
        Check((int)result["total"] == 1 && (int)result["items"][0]["customerId"] == 8, "Project customer filter");
        result = Execute(data, "get_opportunity_summary", "{\"limit\":1}");
        Check((decimal)result["summary"]["totalExpectedValue"] == 300 && result["items"].Count() == 1, "Opportunity aggregate must include all pages");
        result = Execute(data, "get_project_detail", "{\"keyword\":\"ABC\"}");
        Check((string)result["status"] == "ambiguous" && data.DetailReads == 0, "Ambiguous names must not load children");
        result = Execute(data, "get_project_detail", "{\"id\":999}");
        Check((string)result["status"] == "not_found" && data.DetailReads == 0, "Unauthorized ID must not load detail");
        result = Execute(data, "get_opportunity_detail", "{\"id\":999}");
        Check((string)result["status"] == "not_found" && data.DetailReads == 0, "Unauthorized opportunity must not load detail");
        result = Execute(data, "get_project_detail", "{\"id\":1}");
        Check((string)result["status"] == "ok" && (int)result["taskSummary"]["totalTasks"] == 3, "Project detail excludes deleted work and includes management source");
        Check((int)result["taskSummary"]["tasksWithoutCompletionPercentage"] == 1, "Missing progress is not zero");
        var managed = result["tasks"]["items"].Single(x => (string)x["source"] == "task_management");
        Check((int)managed["id"] == 44 && managed["endDate"].Type != JTokenType.Null && managed["completedDate"].Type == JTokenType.Null, "Management deadline must not become actual completion date");
        result = Execute(data, "get_opportunity_detail", "{\"id\":1}");
        Check((string)result["status"] == "ok" && result["activities"] != null && result["plans"] != null, "Opportunity detail sections");
        data.Permission = false;
        var reads = data.ListReads;
        Reject(() => Execute(data, "get_project_summary", "{}"), "forbidden");
        Check(data.ListReads == reads, "Module permission must precede data queries");
        data.Permission = true;
        data.ReportedTotal = 5001;
        Reject(() => Execute(data, "get_project_summary", "{}"), "scope_too_large");
        Reject(() => Execute(data, "get_opportunity_summary", "{}"), "scope_too_large");

        DateTimeOffset expires;
        var token = ChatbotIntegration.IssueCapability("alice", out expires);
        ChatbotCapability capability;
        Check(ChatbotIntegration.TryReadCapability(token, out capability) && capability.SubjectId == "alice", "Capability round trip");
        Check(!ChatbotIntegration.TryReadCapability("invalid", out capability), "Malformed capability");
        var raw = HttpServerUtility.UrlTokenDecode(token); raw[raw.Length / 2] ^= 1;
        Check(!ChatbotIntegration.TryReadCapability(HttpServerUtility.UrlTokenEncode(raw), out capability), "Tampered capability");
        var expired = Protect(new ChatbotCapability { SubjectId = "alice", BotId = "test-bot", ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(-1) });
        Check(!ChatbotIntegration.TryReadCapability(expired, out capability), "Expired capability");
        var otherBot = Protect(new ChatbotCapability { SubjectId = "alice", BotId = "other", ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(1) });
        Check(!ChatbotIntegration.TryReadCapability(otherBot, out capability), "Wrong bot capability");
        Check(Call(null, token).StatusCode == HttpStatusCode.BadRequest, "Null request");
        Check(Call(Request("bob"), token).StatusCode == HttpStatusCode.Unauthorized, "Subject substitution");
        Check(Call(Request("alice"), expired).StatusCode == HttpStatusCode.Unauthorized, "Expired HTTP capability");
        Check(Call(Request("alice"), token, "different-id").StatusCode == HttpStatusCode.BadRequest, "Idempotency mismatch");
        Check(Call(Request("alice"), token, "call", true).StatusCode == HttpStatusCode.BadRequest, "Duplicate idempotency headers");
        var unsupported = Request("alice"); unsupported.ToolName = "delete_project";
        Check(Call(unsupported, token).StatusCode == HttpStatusCode.Forbidden, "Unknown tool");
        var invalid = Request("alice"); invalid.Input = JObject.Parse("{\"username\":\"admin\"}");
        var bad = Call(invalid, token);
        Check(bad.StatusCode == HttpStatusCode.BadRequest && bad.Content.Headers.ContentType.MediaType == "application/json" && bad.Headers.CacheControl.NoStore, "Validation response contract");
    }

    private static string Protect(ChatbotCapability value) => HttpServerUtility.UrlTokenEncode(MachineKey.Protect(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), "CRM.Chatbot.Tools.v1"));
    private static ChatbotToolRequest Request(string subject) => new ChatbotToolRequest { Version = 1, BotId = "test-bot", SubjectId = subject, ToolCallId = "call", ToolName = "get_project_summary", Input = new JObject() };
    private static HttpResponseMessage Call(ChatbotToolRequest body, string token, string key = "call", bool duplicate = false)
    {
        using (var controller = new ChatbotToolsController())
        {
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage(HttpMethod.Post, "https://crm.test/api/Chatbot/Tools");
            controller.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            controller.Request.Headers.TryAddWithoutValidation("Idempotency-Key", duplicate ? new[] { key, key } : new[] { key });
            return controller.Tools(body, CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}

public sealed class FakeData : IChatbotData
{
    public bool Permission = true;
    public int DetailReads, ListReads;
    public int? ReportedTotal;
    public string LastSubject;
    private readonly List<RM_ProjectModel> projects = new List<RM_ProjectModel> {
        new RM_ProjectModel { ProjectID = 1, ProjectName = "ABC", CustomerID = 7, Status = 1 },
        new RM_ProjectModel { ProjectID = 2, ProjectName = "ABC 2", CustomerID = 8, Status = 2 } };
    private readonly List<RM_BusinessOpportunityModel> opportunities = new List<RM_BusinessOpportunityModel> {
        new RM_BusinessOpportunityModel { BusinessOpportunityID = 1, ExpectedValue = 100 },
        new RM_BusinessOpportunityModel { BusinessOpportunityID = 2, ExpectedValue = 200 } };
    public bool CanView(string subject, bool project) => Permission;
    public List<RM_ProjectModel> Projects(RM_ProjectSearchModel filter, BaseSearchModel page, out int total)
    { LastSubject = filter.UserName; ListReads++; total = ReportedTotal ?? projects.Count; return projects; }
    public List<RM_BusinessOpportunityModel> Opportunities(RM_BusinessOpportunitySearchModel filter, BaseSearchModel page, out int total)
    { LastSubject = filter.UserName; ListReads++; total = ReportedTotal ?? opportunities.Count; return opportunities; }
    public RM_ProjectModel Project(int id) { DetailReads++; return projects.Single(x => x.ProjectID == id); }
    public RM_BusinessOpportunityModel Opportunity(int id) { DetailReads++; return opportunities.Single(x => x.BusinessOpportunityID == id); }
    public List<RM_ProductProjectModel> Products(int id) => new List<RM_ProductProjectModel>();
    public List<RM_ProjectTaskModel> Tasks(int id) => new List<RM_ProjectTaskModel> {
        new RM_ProjectTaskModel { ProjectTaskID = 1, CompletionPercentage = 100 },
        new RM_ProjectTaskModel { ProjectTaskID = 2 },
        new RM_ProjectTaskModel { ProjectTaskID = 3, IsDeleted = true },
        new RM_ProjectTaskModel { TaskManagementID = 44, IsTaskManagementSource = true, CompletionPercentage = 50, CompletedDate = new DateTime(2026,9,10) } };
    public List<RM_ProjectMemberModel> ProjectMembers(int id) => new List<RM_ProjectMemberModel>();
    public List<RM_OpportunityPlanModel> Plans(int id) => new List<RM_OpportunityPlanModel>();
    public List<RM_SalesTeamMembersModel> OpportunityMembers(int id, BaseSearchModel page, out int total)
    { total = 0; return new List<RM_SalesTeamMembersModel>(); }
    public List<RM_ExchangeHistoryModel> Activities(int id, BaseSearchModel page, out int total)
    { total = 0; return new List<RM_ExchangeHistoryModel>(); }
}
