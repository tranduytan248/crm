using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_OpportunityDashboardBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private const string SP_DASHBOARD = "RM_OpportunityDashboard_Get";
        private const string SP_CHART = "RM_OpportunityDashboardChart_Get";
        private const string SP_PROJECT_MEMBER = "RM_Dashboard_ProjectMember_Get";

        public RM_OpportunityDashboardViewModel GetViewModel(DashboardSearchModel search)
        {
            var rows = GetDashboard(search);
            var chartRows = GetChart(search);
            var projectMembers = GetProjectMembers(search);

            var totalWon = rows.Sum(r => r.Won);
            var totalClosed = rows.Sum(r => r.Closed);

            return new RM_OpportunityDashboardViewModel
            {
                EmpFromDate = search.EmpFromDate,
                EmpToDate = search.EmpToDate,
                TotalOpportunity = rows.Sum(r => r.Total),
                TotalOpen = rows.Sum(r => r.Open),
                TotalClosed = totalClosed,
                TotalWon = totalWon,
                WinRate = totalClosed > 0 ? Math.Round((decimal)totalWon / totalClosed * 100, 1) : 0,
                Rows = rows,
                ChartRows = chartRows,
                ProjectMembers = projectMembers
            };
        }

        private List<RM_OpportunityDashboardRowModel> GetDashboard(DashboardSearchModel search)
        {
            return AppProcessor.ProcedureProvider
                .ExecuteTypedList<RM_OpportunityDashboardRowModel>(
                    SP_DASHBOARD, DATA_PROVIDER_NAME,
                    search.EmpFromDate, search.EmpToDate, search.EmployeeIds)
                ?? new List<RM_OpportunityDashboardRowModel>();
        }

        private List<RM_OpportunityChartRowModel> GetChart(DashboardSearchModel search)
        {
            return AppProcessor.ProcedureProvider
                .ExecuteTypedList<RM_OpportunityChartRowModel>(
                    SP_CHART, DATA_PROVIDER_NAME,
                    search.EmpFromDate, search.EmpToDate, search.EmployeeIds)
                ?? new List<RM_OpportunityChartRowModel>();
        }

        private List<ProjectMemberDashboardRow> GetProjectMembers(DashboardSearchModel search)
        {
            return AppProcessor.ProcedureProvider
                .ExecuteTypedList<ProjectMemberDashboardRow>(
                    SP_PROJECT_MEMBER, DATA_PROVIDER_NAME,
                    search.EmpFromDate, search.EmpToDate, search.EmployeeIds)
                ?? new List<ProjectMemberDashboardRow>();
        }
    }
}