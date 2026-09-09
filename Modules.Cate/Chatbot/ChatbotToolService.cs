using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Chatbot
{
    /// <summary>Read-only handlers. Never query a detail/child table before the scoped list grants access.</summary>
    public sealed class ChatbotToolService
    {
        public static readonly IReadOnlyCollection<string> AllowedTools = Array.AsReadOnly(new[]
        {
            "get_project_summary", "get_project_detail", "get_opportunity_summary", "get_opportunity_detail"
        });
        private const int MaximumRows = 5000;
        private readonly IChatbotData data;
        public ChatbotToolService() : this(new ChatbotData()) { }
        public ChatbotToolService(IChatbotData data) { this.data = data ?? throw new ArgumentNullException(nameof(data)); }

        public object Execute(string toolName, string subjectId, ChatbotToolInput input, CancellationToken cancellation)
        {
            bool project = toolName.StartsWith("get_project_", StringComparison.Ordinal);
            bool detail = toolName.EndsWith("_detail", StringComparison.Ordinal);
            if (!AllowedTools.Contains(toolName)) throw new ChatbotToolException(HttpStatusCode.Forbidden, "tool_not_allowed", "Tool is not allowed.");
            if (string.IsNullOrWhiteSpace(subjectId)) throw new ChatbotToolException(HttpStatusCode.Unauthorized, "unauthorized", "Missing authenticated subject.");
            cancellation.ThrowIfCancellationRequested();
            if (!data.CanView(subjectId, project))
                throw new ChatbotToolException(HttpStatusCode.Forbidden, "forbidden", "You do not have permission to view this module.");
            cancellation.ThrowIfCancellationRequested();
            return project ? Projects(subjectId, input, detail, cancellation) : Opportunities(subjectId, input, detail, cancellation);
        }

        private object Projects(string subject, ChatbotToolInput input, bool detail, CancellationToken cancellation)
        {
            int total;
            var filter = new RM_ProjectSearchModel
            {
                UserName = subject, Keyword = input.Keyword,
                BoPhanID = input.DepartmentId ?? 0, EmployeeID = input.EmployeeId ?? 0,
                StatusIDs = input.StatusId?.ToString(), Year = input.Year
            };
            // Use Biz rather than shared detail caches so the current user's scope is re-evaluated.
            var rows = data.Projects(filter, AllRows(), out total) ?? new List<RM_ProjectModel>();
            cancellation.ThrowIfCancellationRequested();
            Complete(total, rows.Count);
            rows = rows.Where(x => !input.CustomerId.HasValue || x.CustomerID == input.CustomerId.Value)
                .GroupBy(x => x.ProjectID).Select(x => x.First()).OrderBy(x => x.ProjectID).ToList();
            if (!detail)
                return new
                {
                    asOf = DateTimeOffset.UtcNow, appliedFilters = input,
                    summary = new { totalProjects = rows.Count, byStatus = rows.GroupBy(x => new { x.Status, x.StatusName })
                        .Select(g => new { statusId = g.Key.Status, statusName = g.Key.StatusName, count = g.Count() }).ToArray() },
                    items = rows.Skip(input.Offset).Take(input.Limit).Select(ProjectRow).ToArray(),
                    total = rows.Count, offset = input.Offset, limit = input.Limit, hasMore = rows.Count > input.Offset + input.Limit
                };

            var candidates = rows.Where(x => !input.Id.HasValue || x.ProjectID == input.Id.Value).ToList();
            if (candidates.Count != 1) return Resolution(candidates.Select(ProjectRow).ToList(), input);
            var selected = candidates[0];
            cancellation.ThrowIfCancellationRequested();
            var project = data.Project(selected.ProjectID);
            if (project == null) return Resolution(new List<object>(), input);
            cancellation.ThrowIfCancellationRequested();
            var products = data.Products(selected.ProjectID) ?? new List<RM_ProductProjectModel>();
            cancellation.ThrowIfCancellationRequested();
            var tasks = (data.Tasks(selected.ProjectID) ?? new List<RM_ProjectTaskModel>())
                .Where(x => x.IsDeleted != true).OrderBy(x => x.IsTaskManagementSource)
                .ThenBy(x => x.TaskManagementID ?? x.ProjectTaskID).ToList();
            cancellation.ThrowIfCancellationRequested();
            var members = data.ProjectMembers(selected.ProjectID) ?? new List<RM_ProjectMemberModel>();
            cancellation.ThrowIfCancellationRequested();
            return new
            {
                status = "ok", asOf = DateTimeOffset.UtcNow, project = ProjectRow(project), note = Text(project.Note),
                sourceUrl = Link("/Cate/ProjectOverview/Index/" + selected.ProjectID),
                financials = new { expectedRevenue = products.Sum(x => x.ExpectedRevenue), revenue = products.Sum(x => x.TotalRevenue), cost = products.Sum(x => x.TotalCost), basis = "CRM product-project totals; no period filter applied" },
                taskSummary = new { totalTasks = tasks.Count, tasksWithReported100Percent = tasks.Count(x => x.CompletionPercentage == 100),
                    tasksWithoutCompletionPercentage = tasks.Count(x => !x.CompletionPercentage.HasValue),
                    byStatus = tasks.GroupBy(x => new { x.Status, x.StatusName }).Select(g => new { statusId = g.Key.Status, statusName = g.Key.StatusName, count = g.Count() }).ToArray() },
                products = Page(products.OrderBy(x => x.ProductProjectID).Select(x => (object)new { id = x.ProductProjectID, productServiceId = x.ProductServiceID, name = Text(x.NameProduct), x.StartDate, x.EndDate, x.ExpectedRevenue, x.TotalRevenue, x.TotalCost }), input),
                members = Page(members.OrderBy(x => x.ProjectMemberID).Select(x => (object)new { employeeId = x.Employee_ID, name = Text(x.FullName), productProjectId = x.ProductProjectID, roleIds = x.RoleID, roleNames = x.RoleNames }), input),
                tasks = Page(tasks.Select(x => (object)new { id = x.TaskManagementID ?? x.ProjectTaskID,
                    source = x.IsTaskManagementSource ? "task_management" : "project_task",
                    productProjectId = x.ProductProjectID, name = Text(x.TaskName), assignees = Text(x.AssignedEmployeeNames),
                    x.StartDate, completedDate = x.IsTaskManagementSource ? null : x.CompletedDate,
                    endDate = x.IsTaskManagementSource ? x.CompletedDate : null,
                    x.Status, x.StatusName, x.CompletionPercentage, note = Text(x.Note), updatedAt = x.LastModifiedDate ?? x.CreatedDate }), input),
                dataNotes = new[] { "SuccessRate is probability of success, not completion percentage.", "CompletedDate is returned as recorded; no overdue classification or overall completion formula is inferred.", "Text fields are limited to 2000 characters; child lists are independently paginated using offset/limit." }
            };
        }

        private object Opportunities(string subject, ChatbotToolInput input, bool detail, CancellationToken cancellation)
        {
            int total;
            var filter = new RM_BusinessOpportunitySearchModel
            {
                UserName = subject, Keyword = input.Keyword, CustomerID = input.CustomerId ?? 0,
                BoPhanID = input.DepartmentId ?? 0, EmployeeID = input.EmployeeId ?? 0, StatusID = input.StatusId ?? 0
            };
            var rows = data.Opportunities(filter, AllRows(), out total) ?? new List<RM_BusinessOpportunityModel>();
            cancellation.ThrowIfCancellationRequested();
            Complete(total, rows.Count);
            rows = rows.GroupBy(x => x.BusinessOpportunityID).Select(x => x.First()).OrderBy(x => x.BusinessOpportunityID).ToList();
            if (!detail)
                return new
                {
                    asOf = DateTimeOffset.UtcNow, appliedFilters = input,
                    summary = new { totalOpportunities = rows.Count, totalExpectedValue = rows.Sum(x => x.ExpectedValue),
                        byStatus = rows.GroupBy(x => new { x.StatusID, x.StatusName }).Select(g => new { statusId = g.Key.StatusID, statusName = g.Key.StatusName, count = g.Count(), expectedValue = g.Sum(x => x.ExpectedValue) }).ToArray() },
                    items = rows.Skip(input.Offset).Take(input.Limit).Select(OpportunityRow).ToArray(),
                    total = rows.Count, offset = input.Offset, limit = input.Limit, hasMore = rows.Count > input.Offset + input.Limit
                };

            var candidates = rows.Where(x => !input.Id.HasValue || x.BusinessOpportunityID == input.Id.Value).ToList();
            if (candidates.Count != 1) return Resolution(candidates.Select(OpportunityRow).ToList(), input);
            int id = candidates[0].BusinessOpportunityID;
            cancellation.ThrowIfCancellationRequested();
            var opportunity = data.Opportunity(id);
            if (opportunity == null) return Resolution(new List<object>(), input);
            cancellation.ThrowIfCancellationRequested();
            var plans = data.Plans(id) ?? new List<RM_OpportunityPlanModel>();
            cancellation.ThrowIfCancellationRequested();
            var members = data.OpportunityMembers(id, new BaseSearchModel { Order = "1", OrderDir = "ASC", StartIndex = input.Offset, PageSize = input.Limit }, out total) ?? new List<RM_SalesTeamMembersModel>();
            int memberTotal = total;
            cancellation.ThrowIfCancellationRequested();
            var history = data.Activities(id, new BaseSearchModel { Order = "0", OrderDir = "DESC", StartIndex = input.Offset, PageSize = input.Limit }, out total) ?? new List<RM_ExchangeHistoryModel>();
            cancellation.ThrowIfCancellationRequested();
            return new
            {
                status = "ok", asOf = DateTimeOffset.UtcNow, opportunity = OpportunityRow(opportunity),
                description = Text(opportunity.Description), contact = new { name = Text(opportunity.ContactPersonName), position = Text(opportunity.ContactPersonPosition) },
                sourceUrl = Link("/Cate/BusinessOpportunityOverview/Index/" + id),
                plans = Page(plans.OrderByDescending(x => x.WorkingDate).ThenBy(x => x.Id).Select(x => (object)new { id = x.Id, name = Text(x.PlanName), content = Text(x.Content), x.WorkingDate, address = Text(x.AddressMeeting), x.Username, relatedPeople = Text(x.RelatedPersonUsernames) }), input),
                members = new { total = memberTotal, offset = input.Offset, limit = input.Limit, hasMore = memberTotal > input.Offset + input.Limit,
                    items = members.Select(x => new { employeeId = x.EmployeeID, name = Text(x.FullName), roleIds = x.RoleID, roleNames = Text(x.RoleNames) }).ToArray() },
                activities = new { total, offset = input.Offset, limit = input.Limit, hasMore = total > input.Offset + input.Limit,
                    items = history.Select(x => new { id = x.ExchangeHistoryID, x.ExchangeDate, content = Text(x.ExchangeContent), author = Text(x.FullName), x.StatusID, x.StatusName, contactName = Text(x.ContactPersonName) }).ToArray() },
                dataNotes = new[] { "ExpectedValue is an opportunity estimate, not realized project revenue.", "Text fields are limited to 2000 characters; child lists are independently paginated using offset/limit." }
            };
        }

        private static BaseSearchModel AllRows() => new BaseSearchModel { Order = "0", OrderDir = "ASC", StartIndex = 0, PageSize = MaximumRows + 1 };
        private static void Complete(int total, int returned)
        {
            if (total > MaximumRows || returned > MaximumRows || total > returned)
                throw new ChatbotToolException(HttpStatusCode.BadRequest, "scope_too_large", "Too many records for a complete report. Narrow keyword/department/status filters; no partial aggregate was calculated.");
        }
        private static object Resolution(List<object> candidates, ChatbotToolInput input) => new
        {
            status = candidates.Count == 0 ? "not_found" : "ambiguous",
            message = candidates.Count == 0 ? "No matching record in your accessible scope." : "Ask the user to select a record, then call this tool with its id.",
            candidates = candidates.Skip(input.Offset).Take(input.Limit).ToArray(), total = candidates.Count,
            offset = input.Offset, limit = input.Limit, hasMore = candidates.Count > input.Offset + input.Limit
        };
        private static object Page(IEnumerable<object> source, ChatbotToolInput input)
        {
            var items = source.ToList();
            return new { items = items.Skip(input.Offset).Take(input.Limit).ToArray(), total = items.Count, offset = input.Offset, limit = input.Limit, hasMore = items.Count > input.Offset + input.Limit };
        }
        private static object ProjectRow(RM_ProjectModel x) => new { id = x.ProjectID, name = Text(x.ProjectName), customerId = x.CustomerID, customerName = Text(x.CustomerName), statusId = x.Status, statusName = x.StatusName, x.StartDate, successProbability = x.SuccessRate, opportunityId = x.BusinessOpportunityID };
        private static object OpportunityRow(RM_BusinessOpportunityModel x) => new { id = x.BusinessOpportunityID, code = x.CodeOpportunity, name = Text(x.OpportunityName), customerId = x.CustomerID, customerName = Text(x.CustomerName), statusId = x.StatusID, statusName = x.StatusName, salesStageId = x.SalesStageID, closingProbability = x.ClosingProbability, expectedValue = x.ExpectedValue, x.ExpectedDate, projectId = x.ProjectID, productServiceIds = x.ProductServiceIDs };
        private static string Text(string value) => value == null || value.Length <= 2000 ? value : value.Substring(0, 1999) + "…";
        private static string Link(string path) => (System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath ?? "").TrimEnd('/') + path;
    }
}
