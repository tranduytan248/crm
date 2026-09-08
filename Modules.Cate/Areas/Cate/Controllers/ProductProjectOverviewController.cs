using Core.Cate.Biz;
using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using System;
using System.Linq;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class ProductProjectOverviewController : AppController
    {
        private readonly RM_ProductCostCache _productCostCache;
        private readonly RM_ProductProjectCache _productProjectCache;
        private readonly RM_RevenueReceivedCache _revenueReceivedCache;
        private readonly RM_ProjectMemberCache _projectMemberCache;
        private readonly RM_RolesCache _rolesCache;
        private readonly RM_TaskManagementCache _projectTaskCache;
        private readonly MN_EmployeeCache _employeeCache;
        private readonly RM_TaskStatusCache _taskStatusCache;
        private readonly RM_ContractsCache _contractsCache;
        private readonly SysUserCache _sysUserCache;
        private readonly RM_ContractsBiz _contractsBiz;

        public ProductProjectOverviewController()
        {
            _productCostCache = new RM_ProductCostCache();
            _productProjectCache = new RM_ProductProjectCache();
            _revenueReceivedCache = new RM_RevenueReceivedCache();
            _projectMemberCache = new RM_ProjectMemberCache();
            _rolesCache = new RM_RolesCache();
            _projectTaskCache = new RM_TaskManagementCache();
            _employeeCache = new MN_EmployeeCache();
            _taskStatusCache = new RM_TaskStatusCache();
            _contractsCache = new RM_ContractsCache();
            _contractsBiz = new RM_ContractsBiz();
            _sysUserCache = new SysUserCache();
        }

        private string ResolveAssignedUserName(string assignedEmployeeIDs)
        {
            if (string.IsNullOrEmpty(assignedEmployeeIDs)) return null;
            var assigned = assignedEmployeeIDs.Trim();
            if (int.TryParse(assigned, out int empId))
            {
                var u = _sysUserCache.GetById(empId);
                return u?.UserName;
            }
            return assigned;
        }
        private string ResolveAssignedFullName(string assignedEmployeeIDs)
        {
            if (string.IsNullOrEmpty(assignedEmployeeIDs)) return null;
            var assigned = assignedEmployeeIDs.Trim();

            if (int.TryParse(assigned, out int empId))
            {
                var u = _sysUserCache.GetById(empId);
                return u?.FullName;
            }
            var user = _sysUserCache.GetByUserName(assigned);
            return user?.FullName;
        }

        public ActionResult Index(int id)
        {
            var model = new ProductProjectOverviewModel();
            var productProject = _productProjectCache.GetById(id);
            var productCost = _productCostCache.GetByProductProjectID(id);
            var revenueReceived = _revenueReceivedCache.GetByProductProjectID(id);
            var projectMember = _projectMemberCache.GetByProductProjectID(id);
            var projectTasks = _projectTaskCache.GetByProductProjectID(id);
            var contracts = _contractsCache.GetByProductProjectID(id);

            foreach (var item in contracts)
            {
                item.ExistingFiles = _contractsBiz.GetFilePaths(item.ContractID);
            }

            //if (projectTasks != null && projectTasks.Count > 0)
            //{
            //    foreach (var item in projectTasks)
            //    {
            //        item.AssignedEmployeeNames = ResolveAssignedFullName(item.AssignedEmployeeIDs);
            //    }
            //}

            if (projectMember != null && projectMember.Count > 0)
            {
                var roles = _rolesCache.GetAll();
                foreach (var item in projectMember)
                {
                    var idRoles = item.RoleID.Split(';').Select(x => int.Parse(x)).ToList();
                    item.RoleNames = roles.Where(r => idRoles.Contains(r.RoleID))
                                         .Select(r => r.RoleName).ToList();
                }
            }

            decimal totalRevenue = revenueReceived?.Sum(x => x.Amount) ?? 0;
            decimal totalCost = productCost?.Sum(x => x.Amount) ?? 0;
            decimal profit = totalRevenue - totalCost;
            int totalMember = projectMember?.Count ?? 0;
            int totalTasks = projectTasks?.Count ?? 0;
            int totalContracts = contracts?.Count ?? 0;
            int countCost = productCost?.Count ?? 0;
            int countRevenue = revenueReceived?.Count ?? 0;

            var bas = projectMember?
                .Where(x => x.RoleNames != null && x.RoleNames.Contains("BA"))
                .Select(x => x.FullName).Distinct().ToList();

            var pms = projectMember?
                .Where(x => x.RoleNames != null && x.RoleNames.Contains("PM"))
                .Select(x => x.FullName).Distinct().ToList();

            var ams = projectMember?
                .Where(x => x.RoleNames != null && x.RoleNames.Contains("AM"))
                .Select(x => x.FullName).Distinct().ToList();

            model.ProductProjectModel = productProject;
            model.ProductCosts = productCost;
            model.RevenueReceiveds = revenueReceived;
            model.ProjectMembers = projectMember;
            model.ProjectTasks = projectTasks;
            model.Contracts = contracts;
            model.TotalRevenue = totalRevenue;
            model.TotalCost = totalCost;
            model.Profit = profit;
            model.TotalMember = totalMember;
            model.TotalTask = totalTasks;
            model.TotalContract = totalContracts;
            model.CountCost = countCost;
            model.CountRevenue = countRevenue;
            model.PMs = pms;
            model.AMs = ams;

            ViewBag.TaskStatuses = _taskStatusCache.GetAll();
            ViewBag.CurrentUserName = User.UserName;
            return View(model);
        }

        public ActionResult ListProductCostByProductProject(int id)
        {
            ViewData["ProductProjectID"] = id;
            var data = _productCostCache.GetByProductProjectID(id);
            return PartialView("_ProductCost", data);
        }

        public ActionResult ListRevenueReceivedByProductProject(int id)
        {
            ViewData["ProductProjectID"] = id;
            var data = _revenueReceivedCache.GetByProductProjectID(id);
            return PartialView("_RevenueReceived", data);
        }

        public ActionResult ListProjectMemberByProductProject(int id)
        {
            ViewData["ProductProjectID"] = id;
            var data = _projectMemberCache.GetByProductProjectID(id);
            if (data != null && data.Count > 0)
            {
                var roles = _rolesCache.GetAll();
                foreach (var item in data)
                {
                    var idRoles = item.RoleID.Split(';').Select(x => int.Parse(x)).ToList();
                    item.RoleNames = roles.Where(r => idRoles.Contains(r.RoleID))
                                         .Select(r => r.RoleName).ToList();
                }
            }
            return PartialView("_Member", data);
        }

        public ActionResult ListProjectTaskByProductProject(int id)
        {
            var data = _projectTaskCache.GetByProductProjectID(id);
            //if (data != null && data.Count > 0)
            //{
            //    foreach (var item in data)
            //    {
            //        item.AssignedEmployeeNames = ResolveAssignedFullName(item.AssignedEmployeeIDs);
            //    }
            //}

            ViewBag.TaskStatuses = _taskStatusCache.GetAll();
            ViewBag.ProductProjectID = id;
            ViewBag.CurrentUserName = User.UserName;
            return PartialView("_ProjectTask", data);
        }

        public ActionResult ListContractsByProductProject(int id)
        {
            ViewData["ProductProjectID"] = id;
            var data = _contractsCache.GetByProductProjectID(id);
            foreach (var item in data)
            {
                item.ExistingFiles = _contractsBiz.GetFilePaths(item.ContractID);
            }
            return PartialView("_Contracts", data);
        }

        public ActionResult GetProductProjectInfo(int id)
        {
            var model = new ProductProjectOverviewModel();
            var productProject = _productProjectCache.GetById(id);
            var productCost = _productCostCache.GetByProductProjectID(id);
            var revenueReceived = _revenueReceivedCache.GetByProductProjectID(id);
            var projectMember = _projectMemberCache.GetByProductProjectID(id);
            var projectTasks = _projectTaskCache.GetByProductProjectID(id);

            if (projectTasks != null && projectTasks.Count > 0)
            {
                var employees = _employeeCache.GetAll();
                //foreach (var item in projectTasks)
                //{
                //    if (!string.IsNullOrEmpty(item.AssignedEmployeeIDs))
                //    {
                //        var idList = item.AssignedEmployeeIDs
                //        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                //        .Select(x => { int v; return int.TryParse(x.Trim(), out v) ? v : 0; })
                //        .Where(x => x > 0)
                //        .ToList();
                //        item.AssignedEmployeeNames = string.Join(", ",
                //            employees.Where(e => idList.Contains(e.Employee_ID))
                //                     .Select(e => e.FullName));
                //    }
                //}
            }

            if (projectMember != null && projectMember.Count > 0)
            {
                var roles = _rolesCache.GetAll();
                foreach (var item in projectMember)
                {
                    var idRoles = item.RoleID.Split(';').Select(x => int.Parse(x)).ToList();
                    item.RoleNames = roles.Where(r => idRoles.Contains(r.RoleID))
                                         .Select(r => r.RoleName).ToList();
                }
            }

            decimal totalRevenue = revenueReceived?.Sum(x => x.Amount) ?? 0;
            decimal totalCost = productCost?.Sum(x => x.Amount) ?? 0;
            decimal profit = totalRevenue - totalCost;
            int totalMember = projectMember?.Count ?? 0;

            // BA
            var bas = projectMember?
                .Where(x => x.RoleNames != null && x.RoleNames.Contains("BA"))
                .Select(x => x.FullName)
                .Distinct()
                .ToList();

            // PM
            var pms = projectMember?
                .Where(x => x.RoleNames != null && x.RoleNames.Contains("PM"))
                .Select(x => x.FullName)
                .Distinct()
                .ToList();

            var ams = projectMember?
                .Where(x => x.RoleNames != null && x.RoleNames.Contains("AM"))
                .Select(x => x.FullName)
                .Distinct()
                .ToList();
            model.ProductProjectModel = productProject;
            model.ProductCosts = productCost;
            model.RevenueReceiveds = revenueReceived;
            model.ProjectMembers = projectMember;
            model.ProjectTasks = projectTasks;
            model.TotalRevenue = totalRevenue;
            model.TotalCost = totalCost;
            model.Profit = profit;
            model.TotalMember = totalMember;
            model.PMs = pms;
            model.AMs = ams;

            ViewBag.TaskStatuses = _taskStatusCache.GetAll();
            return PartialView("_ProductProjectOverview", model);
        }
    }
}