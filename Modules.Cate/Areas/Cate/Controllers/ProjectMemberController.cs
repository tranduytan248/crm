using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Cate.Services;
using Core.Services;
using Core.Sys.BaseApp;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class ProjectMemberController : AppController
    {
        private readonly RM_ProjectMemberCache _projectMemberCache;
        private readonly RM_RolesCache _rolesCache;
        private readonly MN_EmployeeCache _employeeCache;
        private readonly MN_BoPhanCache _boPhanCache;
        private readonly RM_ProductProjectCache _productProjectCache;
        private readonly ProjectMailService _projectMailService;
        private readonly SMSQueueService _smsQueueService = new SMSQueueService();
        private readonly string _smsTemplateProjectMemberAdded = ConfigurationManager.AppSettings["SMSTemplate_ProjectMemberAdded"];
        private readonly string _smsTemplateProjectMemberDeleted = ConfigurationManager.AppSettings["SMSTemplate_ProjectMemberDeleted"];
        private readonly string _ProjectMemberTitle = AppProcessor.Messagor.GetMessage("ProjectMember_Title");
        public ProjectMemberController()
        {
            _projectMemberCache = new RM_ProjectMemberCache();
            _rolesCache = new RM_RolesCache();
            _employeeCache = new MN_EmployeeCache();
            _boPhanCache = new MN_BoPhanCache();
            _productProjectCache = new RM_ProductProjectCache();
            _projectMailService = new ProjectMailService();
            _smsQueueService = new SMSQueueService();
        }


        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        // GET: Cate/ProductService
        public ActionResult List(int id)
        {
            var model = new RM_ProjectMemberSearchModel();
            model.ProductProjectID = id;
            return PartialView("_List", model);
        }

        // GET: Cate/ProjectMember
        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult Add(int id)
        {
            int total;
            var employees = _employeeCache.GetAll();
            var projectMember = _projectMemberCache.GetByProductProjectID(id);

            employees.ForEach(employee => employee.IsSaleMember = projectMember.Any(x => x.Employee_ID == employee.Employee_ID));

            var model = new RM_ProjectMemberFormModel()
            {
                ProductProjectID = id,
                Employees = employees,
                Roles = _rolesCache.GetAll(),
                Departments = _boPhanCache.GetAll()
            };
            return PartialView("_Add", model);
        }


        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_ProjectMemberFormModel model)
        {
            if (!ModelState.IsValid)
            {
                int total;
                var employees = _employeeCache.GetAll();
                var projectMember = _projectMemberCache.GetByProductProjectID(model.ProductProjectID);

                employees.ForEach(employee => employee.IsSaleMember = projectMember.Any(x => x.Employee_ID == employee.Employee_ID));

                model.ProductProjectID = model.ProductProjectID;
                model.Employees = employees;
                model.Roles = _rolesCache.GetAll();
                model.Departments = _boPhanCache.GetAll();

                return PartialView("_AddProjectMember", model);
            }

            string response;

            var result = _projectMemberCache.SaveMulti(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_ProjectMemberTitle}",
                    EnumProcessType.Add,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_ProjectMemberTitle}",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);

            else if (result == -8)
                response = CreateMessage($"{_ProjectMemberTitle}",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);

            else if (result == -7)
                response = CreateMessage($"{_ProjectMemberTitle}",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
            {
                response = CreateMessage($"{_ProjectMemberTitle}",
                    EnumProcessType.Add,
                    EnumMsgIcon.Success);

                _projectMailService.QueueSendMemberAddedMail(
                    model.ProductProjectID,
                    GetSelectedUserNames(model.EmployeeIDs),
                    GetRoleNames(model.RoleIDs),
                    User.UserName);

                SendSmsToEmployees(model.EmployeeIDs, _smsTemplateProjectMemberAdded, model.ProductProjectID);
            }

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _projectMemberCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_ProjectMemberTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            model.Roles = _rolesCache.GetAll();
            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_ProjectMemberModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = _rolesCache.GetAll();
                return PartialView("_ProjectMember", model);
            }
            string response;

            var result = _projectMemberCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_ProjectMemberTitle}",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_ProjectMemberTitle}",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);

            else if (result == -8)
                response = CreateMessage($"{_ProjectMemberTitle}",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);

            else if (result == -7)
                response = CreateMessage($"{_ProjectMemberTitle}",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);

            else
                response = CreateMessage($"{_ProjectMemberTitle}",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _projectMemberCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_ProjectMemberTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_ProjectMemberTitle}");

            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_ProjectMemberModel model)
        {
            var member = _projectMemberCache.GetById(model.ProjectMemberID);
            var deleted = _projectMemberCache.Delete(model, User.UserName);
            var employee = _employeeCache.GetById(model.Employee_ID);

            var memberUserName = employee != null
                ? employee.Employee_Code
                : string.Empty;

            var response = CreateMessage(
                $"{_ProjectMemberTitle}",
                EnumProcessType.Delete,
                deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);

            if (deleted > 0)
            {
                _projectMailService.QueueSendMemberRemovedMail(
                    model.ProductProjectID,
                    memberUserName,
                    GetRoleNames(member.RoleID),
                    User.UserName);

                SendSmsToEmployees(model.Employee_ID.ToString(), _smsTemplateProjectMemberDeleted, model.ProductProjectID);
            }

            return Json(new { status = true, message = response });
        }

        /// <summary>
        /// Hàm gửi SMS cho danh sách user được thêm/xóa.
        /// </summary>
        private void SendSmsToEmployees(string employeeIds, string templateCode, int productProjectID)
        {
            if (string.IsNullOrWhiteSpace(employeeIds))
                return;

            var project = _productProjectCache.GetById(productProjectID);
            project.UpdatedBy = User.FullName;

            var ids = employeeIds
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            var phoneNumbers = _employeeCache.GetAll()
                .Where(x => ids.Contains(x.Employee_ID))
                .Select(x => x.Phone)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            if (!phoneNumbers.Any())
                return;

            _smsQueueService.QueueSendByTemplateCode(
                templateCode,
                phoneNumbers,
                User.UserName,
                project);
        }

        /// <summary>
        /// lấy thông tin username từ list id.
        /// </summary>
        private List<string> GetSelectedUserNames(string employeeIdsRaw)
        {
            var employeeIds = SplitIds(employeeIdsRaw);
            if (employeeIds.Count == 0)
            {
                return new List<string>();
            }

            return _employeeCache.GetAll()
                .Where(employee => employeeIds.Contains(employee.Employee_ID))
                .Select(employee => employee.Employee_Code)
                .Where(employeeCode => !string.IsNullOrWhiteSpace(employeeCode))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }


        /// <summary>
        /// Split id từ chuỗi employeeIds.
        /// </summary>
        private List<int> SplitIds(string idsRaw)
        {
            if (string.IsNullOrWhiteSpace(idsRaw))
            {
                return new List<int>();
            }

            return idsRaw
                .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value =>
                {
                    int.TryParse(value, out var id);
                    return id;
                })
                .Where(id => id > 0)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// get RoleName từ danh sách id.
        /// </summary>
        private string GetRoleNames(string roleIdsRaw)
        {
            var roleIds = SplitIds(roleIdsRaw);
            if (roleIds.Count == 0)
            {
                return string.Empty;
            }

            var roleNames = _rolesCache.GetAll()
                .Where(role => roleIds.Contains(role.RoleID))
                .Select(role => role.RoleName)
                .Where(roleName => !string.IsNullOrWhiteSpace(roleName));

            return string.Join("; ", roleNames);
        }
    }
}