using Core.Cate.Caches;
using System;
using System.Linq;
using System.Web.Mvc;
using Core.Cate.Models;
using Core.Cate.Services;
using Core.Services;
using Core.Sys.BaseApp;
using System.Configuration;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class OpportunityPlanRelatedPersonController : AppController
    {
        private readonly RM_OpportunityPlanRelatedPersonCache _cache;
        private readonly RM_OpportunityPlanCache _opportunityPlanCache;
        private readonly MN_EmployeeCache _employeeCache;
        private readonly OpportunityPlanMailService _opportunityPlanMailService;
        private readonly SMSQueueService _smsQueueService = new SMSQueueService();
        private readonly string _title = AppProcessor.Messagor.GetMessage("OpportunityPlanRelatedPerson_Title");
        private readonly string _smsTemplateKHKDMemberAdded = ConfigurationManager.AppSettings["SMSTemplate_KHKDMemberAdded"];

        public OpportunityPlanRelatedPersonController()
        {
            _cache = new RM_OpportunityPlanRelatedPersonCache();
            _opportunityPlanCache = new RM_OpportunityPlanCache();
            _employeeCache = new MN_EmployeeCache();
            _opportunityPlanMailService = new OpportunityPlanMailService();
        }

        /// <summary>
        /// Hiển thị danh sách người liên quan của kế hoạch theo cơ hội kinh doanh được chọn.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult List(int id)
        {
            var plan = _opportunityPlanCache.GetById(id);
            var titleTemplate = AppProcessor.Messagor.GetMessage("OpportunityPlanRelatedPerson_List_Title");

            titleTemplate = string.IsNullOrWhiteSpace(titleTemplate)
                ? "Danh sách người liên quan [{0}]"
                : titleTemplate;

            ViewBag.Title = string.Format(titleTemplate, plan?.PlanName ?? "");

            var model = new RM_OpportunityPlanRelatedPersonSearchModel
            {
                OpportunityPlanID = id
            };

            return PartialView("_List", model);
        }

        /// <summary>
        /// Tải dữ liệu người liên quan theo kế hoạch để hiển thị lên lưới.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(RM_OpportunityPlanRelatedPersonSearchModel model)
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

            var data = _cache.GetByOpportunityPlanID(model.OpportunityPlanID, out var total, dataSearch);

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
        /// Hiển thị popup thêm mới người liên quan cho kế hoạch.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(int id)
        {
            int total;
            var users = _employeeCache.GetAll()
                .Select(u => new RelatedPersonUserModel
                {
                    UserName = u.Employee_Code,
                    FullName = u.FullName,
                    OfficeName = u.TenBoPhan
                })
                .Where(u => !string.IsNullOrEmpty(u.UserName))
                .ToList();

            var relatedPersons = _cache.GetByOpportunityPlanID(id, out total, null);

            var model = new RM_OpportunityPlanRelatedPersonFormModel
            {
                OpportunityPlanID = id,
                Users = users,
                ExistingUsernames = relatedPersons.Select(x => x.Username).ToList()
            };

            return PartialView("_Add", model);
        }

        /// <summary>
        /// Xử lý thêm mới người liên quan cho kế hoạch và queue mail thông báo khi thành công.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_OpportunityPlanRelatedPersonFormModel model)
        {
            if (!ModelState.IsValid)
            {
                int total;
                var relatedPersons = _cache.GetByOpportunityPlanID(model.OpportunityPlanID, out total, null);

                model.Users = _employeeCache.GetAll()
                    .Select(u => new RelatedPersonUserModel
                    {
                        UserName = u.Employee_Code,
                        FullName = u.FullName,
                        OfficeName = u.TenBoPhan
                    })
                    .Where(u => !string.IsNullOrEmpty(u.UserName))
                    .ToList();
                model.ExistingUsernames = relatedPersons.Select(x => x.Username).ToList();

                return PartialView("_AddRelatedPerson", model);
            }

            try
            {
                var result = _cache.SaveMulti(model, User.UserName);
                string response;

                if (result <= 0)
                {
                    response = CreateMessage(_title, EnumProcessType.Add, EnumMsgIcon.Error);
                }
                else
                {
                    response = CreateMessage(_title, EnumProcessType.Add, EnumMsgIcon.Success);
                    _opportunityPlanMailService.QueueSendRelatedPersonAddedMail(model.OpportunityPlanID, model.Usernames);
                    SendSmsToEmployees(model.Usernames, _smsTemplateKHKDMemberAdded, model.OpportunityPlanID);
                }

                return Json(new { status = result > 0, message = response }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);

                var response = CreateMessage(_title, EnumProcessType.Add, EnumMsgIcon.Error);
                return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Hiển thị popup xác nhận xóa người liên quan khỏi kế hoạch.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id, string fullName)
        {
            var personName = string.IsNullOrWhiteSpace(fullName) ? _title : fullName;
            var confirmMessage = AppProcessor.Messagor.GetMessage("OpportunityPlanRelatedPerson_Delete_Confirm");

            confirmMessage = string.IsNullOrWhiteSpace(confirmMessage)
                ? "Bạn muốn xóa người liên quan [{0}]?"
                : confirmMessage;

            var model = new RM_OpportunityPlanRelatedPersonModel
            {
                Id = id,
                FullName = fullName
            };

            ViewBag.ConfirmMessage = string.Format(confirmMessage, personName);
            return PartialView("_Delete", model);
        }

        /// <summary>
        /// Xử lý xóa người liên quan khỏi kế hoạch.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_OpportunityPlanRelatedPersonModel model)
        {
            try
            {
                var deleted = _cache.Delete(model, User.UserName);
                var response = CreateMessage(
                    _title,
                    EnumProcessType.Delete,
                    deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);

                return Json(new { status = deleted > 0, message = response });
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);

                var response = CreateMessage(_title, EnumProcessType.Delete, EnumMsgIcon.Error);
                return Json(new { status = false, message = response });
            }
        }

        /// <summary>
        /// Hàm gửi SMS cho danh sách user được thêm/xóa.
        /// </summary>
        private void SendSmsToEmployees(string employees, string templateCode, int id)
        {
            if (string.IsNullOrWhiteSpace(employees))
                return;

            var project = _opportunityPlanCache.GetById(id);
            project.UpdatedBy = User.FullName;

            var userNames = employees
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            var phoneNumbers = _employeeCache.GetAll()
                .Where(x => userNames.Contains(x.Employee_Code))
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
    }
}
