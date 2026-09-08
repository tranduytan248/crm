using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Cate.Services;
using Core.Services;
using Core.Sys.BaseApp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    /// <summary>
    /// Xử lý các luồng thêm, sửa, xóa, tìm kiếm và thông báo thành viên tham gia cơ hội kinh doanh.
    /// </summary>
    public class SalesTeamMembersController : AppController
    {
        private readonly RM_SalesTeamMembersCache _salesTeamMembersCache;
        private readonly RM_RolesCache _rolesCache;
        private readonly MN_EmployeeCache _employeeCache;
        private readonly MN_BoPhanCache _boPhanCache;
        private readonly RM_BusinessOpportunityCache _businessOpportunityCache;
        private readonly BusinessOpportunityMemberMailService _businessOpportunityMemberMailService;
        private readonly SMSQueueService _smsQueueService;
        private readonly string _salesTeamMembersTitle;
        private readonly string _smsTemplateBOMemberAdded = ConfigurationManager.AppSettings["SMSTemplate_BOMemberAdded"];
        private readonly string _smsTemplateBOMemberDeleted = ConfigurationManager.AppSettings["SMSTemplate_BOMemberDeleted"];

        /// <summary>
        /// Khởi tạo cache và service phục vụ xử lý thành viên cơ hội kinh doanh.
        /// </summary>
        public SalesTeamMembersController()
        {
            _salesTeamMembersCache = new RM_SalesTeamMembersCache();
            _rolesCache = new RM_RolesCache();
            _employeeCache = new MN_EmployeeCache();
            _boPhanCache = new MN_BoPhanCache();
            _businessOpportunityCache = new RM_BusinessOpportunityCache();
            _businessOpportunityMemberMailService = new BusinessOpportunityMemberMailService();
            _smsQueueService = new SMSQueueService();
            _salesTeamMembersTitle = AppProcessor.Messagor.GetMessage("SalesTeamMembers_Title");
        }

        /// <summary>
        /// Hiển thị danh sách thành viên tham gia cơ hội kinh doanh theo mã được chọn.
        /// </summary>
        /// <param name="id">Mã cơ hội kinh doanh cần xem danh sách thành viên.</param>
        /// <returns>Partial view danh sách thành viên.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult List(int id)
        {
            var model = new RM_SalesTeamMembersSearchModel
            {
                BusinessOpportunityID = id
            };

            return PartialView("_List", model);
        }

        /// <summary>
        /// Tải dữ liệu thành viên tham gia cơ hội kinh doanh theo điều kiện tìm kiếm và phân trang.
        /// </summary>
        /// <param name="model">Điều kiện tìm kiếm thành viên.</param>
        /// <returns>Dữ liệu JSON cho lưới danh sách thành viên.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(RM_SalesTeamMembersSearchModel model)
        {
            var search = Request.Form.GetValues("search[value]")?[0];
            var draw = Request.Form.GetValues("draw")?[0];
            var order = Request.Form.GetValues("order[0][column]")?[0];
            var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0]);

            var dataSearch = new BaseSearchModel
            {
                Search = string.IsNullOrEmpty(search) ? null : search,
                Order = order,
                OrderDir = orderDir,
                StartIndex = startRec,
                PageSize = pageSize
            };

            var data = _salesTeamMembersCache.GetByBusinessOpportunityID(model.BusinessOpportunityID, out var total, dataSearch);

            if (data != null && data.Count > 0)
            {
                var roles = _rolesCache.GetAll();

                foreach (var item in data)
                {
                    var roleIds = item.RoleID.Split(';')
                        .Select(int.Parse)
                        .ToList();

                    var roleNames = roles
                        .Where(role => roleIds.Contains(role.RoleID))
                        .Select(role => role.RoleName);

                    item.RoleNames = string.Join(";", roleNames);
                }
            }

            return Json(
                new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = total,
                    recordsFiltered = total,
                    data
                },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị popup thêm mới thành viên cho cơ hội kinh doanh.
        /// </summary>
        /// <param name="id">Mã cơ hội kinh doanh cần thêm thành viên.</param>
        /// <returns>Popup thêm mới thành viên.</returns>
        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult Add(int id)
        {
            var employees = _employeeCache.GetAll();
            var salesTeamMembers = _salesTeamMembersCache.GetByBusinessOpportunityID(id, out var total, null);

            employees.ForEach(employee => employee.IsSaleMember = salesTeamMembers.Any(x => x.EmployeeID == employee.Employee_ID));

            var model = new RM_SalesTeamMembersFormModel
            {
                BusinessOpportunityID = id,
                Employees = employees,
                Roles = _rolesCache.GetAll(),
                Departments = _boPhanCache.GetAll()
            };

            return PartialView("_Add", model);
        }

        /// <summary>
        /// Lọc danh sách nhân sự theo bộ phận để phục vụ chọn thành viên tham gia cơ hội kinh doanh.
        /// </summary>
        /// <param name="model">Điều kiện lọc nhân sự.</param>
        /// <returns>Partial view danh sách nhân sự sau khi lọc.</returns>
        [AjaxOnly]
        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult Search(RM_SalesTeamMembersSearchModel model)
        {
            var departmentSelected = new List<int>();
            var employees = _employeeCache.GetAll();
            var salesTeamMembers = _salesTeamMembersCache.GetByBusinessOpportunityID(model.BusinessOpportunityID, out var total, null);

            if (!string.IsNullOrEmpty(model.DepartmentID))
            {
                departmentSelected = model.DepartmentID.Split(';')
                    .Select(int.Parse)
                    .ToList();
            }

            employees = departmentSelected.Count > 0
                ? employees.Where(employee => departmentSelected.Contains(employee.BoPhan_ID)).ToList()
                : employees;

            employees.ForEach(employee => employee.IsSaleMember = salesTeamMembers.Any(x => x.EmployeeID == employee.Employee_ID));

            var data = new RM_SalesTeamMembersFormModel
            {
                BusinessOpportunityID = model.BusinessOpportunityID,
                Employees = employees,
                Roles = _rolesCache.GetAll(),
                Departments = _boPhanCache.GetAll()
            };

            return PartialView("_AddSalesTeamMembers", data);
        }

        /// <summary>
        /// Lưu danh sách thành viên mới cho cơ hội kinh doanh và gửi thông báo khi thêm thành công.
        /// </summary>
        /// <param name="model">Dữ liệu thành viên cần thêm.</param>
        /// <returns>Kết quả xử lý thêm thành viên.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_SalesTeamMembersFormModel model)
        {
            if (!ModelState.IsValid)
            {
                var employees = _employeeCache.GetAll();
                var salesTeamMembers = _salesTeamMembersCache.GetByBusinessOpportunityID(model.BusinessOpportunityID, out var total, null);

                employees.ForEach(employee => employee.IsSaleMember = salesTeamMembers.Any(x => x.EmployeeID == employee.Employee_ID));

                model.Employees = employees;
                model.Roles = _rolesCache.GetAll();
                model.Departments = _boPhanCache.GetAll();

                return PartialView("_AddSalesTeamMembers", model);
            }

            var result = _salesTeamMembersCache.SaveMulti(model, User.UserName);
            string response;

            if (result == 0)
            {
                response = CreateMessage(_salesTeamMembersTitle, EnumProcessType.Add, EnumMsgIcon.Error);
            }
            else if (result == -9 || result == -8 || result == -7)
            {
                response = CreateMessage(_salesTeamMembersTitle, EnumProcessType.DataExisted, EnumMsgIcon.Error);
            }
            else
            {
                response = CreateMessage(_salesTeamMembersTitle, EnumProcessType.Add, EnumMsgIcon.Success);

                _businessOpportunityMemberMailService.QueueSendMemberAddedMail(
                    model.BusinessOpportunityID,
                    GetSelectedUserNames(model.EmployeeIDs),
                    GetRoleNames(model.RoleIDs),
                    User.UserName);

                var opportunity = _businessOpportunityCache.GetById(model.BusinessOpportunityID);
                opportunity.UpdatedBy = User.FullName;

                var employeeIds = SplitIds(model.EmployeeIDs);

                var phoneNumbers = _employeeCache.GetAll()
                    .Where(x => employeeIds.Contains(x.Employee_ID))
                    .Select(x => x.Phone)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                .ToList();

                _smsQueueService.QueueSendByTemplateCode(_smsTemplateBOMemberAdded, phoneNumbers, User.UserName, opportunity);
            }

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị popup cập nhật thông tin thành viên tham gia cơ hội kinh doanh.
        /// </summary>
        /// <param name="id">Mã thành viên cần cập nhật.</param>
        /// <returns>Popup cập nhật thành viên hoặc thông báo khi dữ liệu không tồn tại.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _salesTeamMembersCache.GetById(id);
            if (model == null)
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage(_salesTeamMembersTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            model.Roles = _rolesCache.GetAll();
            return PartialView("_Edit", model);
        }

        /// <summary>
        /// Cập nhật thông tin thành viên tham gia cơ hội kinh doanh.
        /// </summary>
        /// <param name="model">Dữ liệu thành viên cần cập nhật.</param>
        /// <returns>Kết quả xử lý cập nhật thành viên.</returns>
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(RM_SalesTeamMembersModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = _rolesCache.GetAll();
                return PartialView("_SalesTeamMembers", model);
            }

            var result = _salesTeamMembersCache.Save(model, User.UserName);
            string response;

            if (result == 0)
            {
                response = CreateMessage(_salesTeamMembersTitle, EnumProcessType.Edit, EnumMsgIcon.Error);
            }
            else if (result == -9 || result == -8 || result == -7)
            {
                response = CreateMessage(_salesTeamMembersTitle, EnumProcessType.DataExisted, EnumMsgIcon.Error);
            }
            else
            {
                response = CreateMessage(_salesTeamMembersTitle, EnumProcessType.Edit, EnumMsgIcon.Success);
            }

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị popup xác nhận xóa thành viên khỏi cơ hội kinh doanh.
        /// </summary>
        /// <param name="id">Mã thành viên cần xóa.</param>
        /// <returns>Popup xác nhận xóa hoặc thông báo khi dữ liệu không tồn tại.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _salesTeamMembersCache.GetById(id);
            if (model == null)
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage(_salesTeamMembersTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                _salesTeamMembersTitle);

            return PartialView("_Delete", model);
        }

        /// <summary>
        /// Xóa thành viên khỏi cơ hội kinh doanh và gửi thông báo khi xóa thành công.
        /// </summary>
        /// <param name="model">Dữ liệu thành viên cần xóa.</param>
        /// <returns>Kết quả xử lý xóa thành viên.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_SalesTeamMembersModel model)
        {
            var member = _salesTeamMembersCache.GetById(model.MemberID);
            var deleted = _salesTeamMembersCache.Delete(model, User.UserName);
            var memberUserName = GetMemberUserName(member);

            var response = CreateMessage(
                _salesTeamMembersTitle,
                EnumProcessType.Delete,
                deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);

            if (deleted > 0 && member != null && !string.IsNullOrWhiteSpace(memberUserName))
            {
                _businessOpportunityMemberMailService.QueueSendMemberRemovedMail(
                    member.BusinessOpportunityID,
                    memberUserName,
                    GetRoleNames(member.RoleID),
                    User.UserName);

                var opportunity = _businessOpportunityCache.GetById(member.BusinessOpportunityID);
                opportunity.UpdatedBy = User.FullName;

                _smsQueueService.QueueSendByTemplateCode(_smsTemplateBOMemberDeleted, new List<string> { member.Phone }, User.UserName, opportunity);
            }

            return Json(new { status = true, message = response });
        }

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

        private string GetMemberUserName(RM_SalesTeamMembersModel member)
        {
            if (member == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(member.Employee_Code))
            {
                return member.Employee_Code.Trim();
            }

            var employee = _employeeCache.GetAll()
                .FirstOrDefault(item => item.Employee_ID == member.EmployeeID);

            return employee?.Employee_Code?.Trim() ?? string.Empty;
        }
    }
}
