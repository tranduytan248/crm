using System.Collections.Generic;
using System.Linq;
using System.Net;
using Core.Cate.Biz;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Chatbot
{
    public interface IChatbotData
    {
        bool CanView(string subject, bool project);
        List<RM_ProjectModel> Projects(RM_ProjectSearchModel filter, BaseSearchModel page, out int total);
        List<RM_BusinessOpportunityModel> Opportunities(RM_BusinessOpportunitySearchModel filter, BaseSearchModel page, out int total);
        RM_ProjectModel Project(int id);
        RM_BusinessOpportunityModel Opportunity(int id);
        List<RM_ProductProjectModel> Products(int id);
        List<RM_ProjectTaskModel> Tasks(int id);
        List<RM_ProjectMemberModel> ProjectMembers(int id);
        List<RM_OpportunityPlanModel> Plans(int id);
        List<RM_SalesTeamMembersModel> OpportunityMembers(int id, BaseSearchModel page, out int total);
        List<RM_ExchangeHistoryModel> Activities(int id, BaseSearchModel page, out int total);
    }

    public sealed class ChatbotData : IChatbotData
    {
        public bool CanView(string subject, bool project) => AppProcessor.Author.IsAllow(subject, "Cate", project ? "Project" : "RM_BusinessOpportunity", "View");
        public List<RM_ProjectModel> Projects(RM_ProjectSearchModel filter, BaseSearchModel page, out int total) => new RM_ProjectBiz().LoadList(out total, filter, page);
        public List<RM_BusinessOpportunityModel> Opportunities(RM_BusinessOpportunitySearchModel filter, BaseSearchModel page, out int total) => new RM_BusinessOpportunityBiz().LoadList(out total, filter, page);
        public RM_ProjectModel Project(int id) => new RM_ProjectBiz().LoadDetail(id);
        public RM_BusinessOpportunityModel Opportunity(int id) => new RM_BusinessOpportunityBiz().LoadDetail(id);
        public List<RM_ProductProjectModel> Products(int id) => new RM_ProductProjectBiz().GetByProjectID(id);
        public List<RM_ProjectTaskModel> Tasks(int id)
        {
            var tasks = new RM_ProjectTaskBiz().GetByProjectID(id) ?? new List<RM_ProjectTaskModel>();
            int total;
            // Same second source used by the CRM project work screen. Explicit page size is
            // essential: GetAll() uses the default search page and can silently omit tasks.
            var managed = new RM_TaskManagementBiz().LoadList(out total,
                new RM_TaskManagementSearchModel { ProjectID = id },
                new BaseSearchModel { Order = "4", OrderDir = "ASC", StartIndex = 0, PageSize = 5001 })
                ?? new List<RM_TaskManagementModel>();
            if (total > 5000 || managed.Count > 5000 || total > managed.Count)
                throw new ChatbotToolException(HttpStatusCode.BadRequest, "scope_too_large", "Too many work items for a complete project report.");
            tasks.AddRange(managed.Select(x => new RM_ProjectTaskModel
            {
                ProjectID = id, TaskManagementID = x.TaskManagementID, IsTaskManagementSource = true,
                ProductProjectID = x.ProductProjectID, TaskID = x.TaskID, TaskName = x.TaskName,
                AssignedEmployeeNames = x.AssigneeNames, StartDate = x.StartDate, CompletedDate = x.EndDate,
                Status = x.StatusID.HasValue ? checked((byte?)x.StatusID.Value) : null, StatusName = x.StatusName,
                CompletionPercentage = x.CompletionPercentage, Note = x.Description, IsDeleted = x.IsDeleted,
                CreatedDate = x.CreatedDate, LastModifiedDate = x.UpdatedDate
            }));
            return tasks;
        }
        public List<RM_ProjectMemberModel> ProjectMembers(int id) => new RM_ProjectMemberBiz().GetAll(id);
        public List<RM_OpportunityPlanModel> Plans(int id) => new RM_OpportunityPlanBiz().GetByOpportunityId(id);
        public List<RM_SalesTeamMembersModel> OpportunityMembers(int id, BaseSearchModel page, out int total) => new RM_SalesTeamMembersBiz().GetByBusinessOpportunityID(id, out total, page);
        public List<RM_ExchangeHistoryModel> Activities(int id, BaseSearchModel page, out int total) => new RM_ExchangeHistoryBiz().LoadList(new RM_ExchangeHistorySearchModel { BusinessOpportunityID = id }, out total, page);
    }
}
