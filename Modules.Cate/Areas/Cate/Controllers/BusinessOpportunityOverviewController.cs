using Core.Cate.Biz;
using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Cate.Services;
using Core.Services;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    /// <summary>
    /// Xử lý màn hình tổng quan cơ hội kinh doanh, lịch sử trao đổi, thành viên và kế hoạch liên quan.
    /// </summary>
    public class BusinessOpportunityOverviewController : AppController
    {
        #region Private Members

        private readonly RM_BusinessOpportunityCache _businessOpportunityCache;
        private readonly RM_ExchangeHistoryCache _exchangeHistoryCache;
        private readonly RM_ExchangeHistoryFilePathCache _exchangeHistoryFilePathCache;
        private readonly RM_SalesTeamMembersCache _salesTeamMembersCache;
        private readonly Cate_ProductServiceCache _productServiceCache;
        private readonly RM_RolesCache _rolesCache;
        private readonly RM_OpportunityPlanCache _opportunityPlanCache;
        private readonly RM_OpportunityPlanRelatedPersonCache _opportunityPlanRelatedPersonCache;
        private readonly RM_OpportunityPlanReminderCache _opportunityPlanReminderCache;
        private readonly OpportunityPlanMailService _opportunityPlanMailService;
        private readonly MN_EmployeeCache _employeeCache;
        private readonly SysUserCache _userCache;
        private readonly RM_ReviewBatchItemCache _reviewBatchItemCache;
        private readonly RM_ReviewBatchItemBiz _reviewBatchItemBiz;
        private readonly RM_BusinessOpportunityFilePathCache _businessOpportunityFilePathCache;
        private readonly string _opportunityPlanTitle = AppProcessor.Messagor.GetMessage("OpportunityPlan_Title");
        private readonly SMSQueueService _smsQueueService = new SMSQueueService();

        private readonly string _smsTemplateKHKDCreated = ConfigurationManager.AppSettings["SMSTemplate_KHKDCreated"];
        private const string _viewRoot = "~/Areas/Cate/Views/BusinessOpportunityOverview/";
        private const string _viewRootRM = "~/Areas/Cate/Views/RM_BusinessOpportunity/";

        #endregion

        #region Constructors

        /// <summary>
        /// Khởi tạo cache và service phục vụ màn hình tổng quan cơ hội kinh doanh.
        /// </summary>
        public BusinessOpportunityOverviewController()
        {
            _businessOpportunityCache = new RM_BusinessOpportunityCache();
            _exchangeHistoryCache = new RM_ExchangeHistoryCache();
            _exchangeHistoryFilePathCache = new RM_ExchangeHistoryFilePathCache();
            _salesTeamMembersCache = new RM_SalesTeamMembersCache();
            _productServiceCache = new Cate_ProductServiceCache();
            _rolesCache = new RM_RolesCache();
            _opportunityPlanCache = new RM_OpportunityPlanCache();
            _opportunityPlanRelatedPersonCache = new RM_OpportunityPlanRelatedPersonCache();
            _opportunityPlanReminderCache = new RM_OpportunityPlanReminderCache();
            _opportunityPlanMailService = new OpportunityPlanMailService();
            _employeeCache = new MN_EmployeeCache();
            _reviewBatchItemCache = new RM_ReviewBatchItemCache();
            _reviewBatchItemBiz = new RM_ReviewBatchItemBiz();
            _businessOpportunityFilePathCache = new RM_BusinessOpportunityFilePathCache();
            _smsQueueService = new SMSQueueService();
            _userCache = new SysUserCache();
        }

        #endregion

        #region Overview

        /// <summary>
        /// Hiển thị màn hình tổng quan cơ hội kinh doanh theo mã được chọn.
        /// </summary>
        /// <param name="id">Mã cơ hội kinh doanh cần xem tổng quan.</param>
        /// <returns>Màn hình tổng quan hoặc chuyển về danh sách khi dữ liệu không tồn tại.</returns>
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Index(int id, int? reviewBatchID)
        {
            var bo = _businessOpportunityCache.GetById(id);
            if (bo == null)
            {
                return RedirectToAction("Index", "RM_BusinessOpportunity", new { area = "Cate" });
            }

            var files = _businessOpportunityFilePathCache.GetByBOID(bo.BusinessOpportunityID);
            bo.ExistingFiles = files.Select(f => new RM_BusinessOpportunityFilePathModel
            {
                FilePathID = f.FilePathID,
                FilePath = f.FilePath
            }).ToList();

            _EnrichBusinessOpportunity(bo);

            var members = _BuildMembers(id);
            var plans = _BuildPlans(id);
            var reviewHistory = _BuildReviewHistoryForm(1, id);

            var model = new RM_BusinessOpportunityOverviewModel
            {
                BusinessOpportunity = bo,
                ExchangeHistoryForm = _BuildExchangeHistoryForm(id),
                Members = members,
                Plans = plans,
                ReviewHistory = reviewHistory
            };

            // thêm cờ hiển thị nút rà soát
            model.ReviewBatchID = reviewBatchID;

            model.TotalExchangeHistory = model.ExchangeHistoryForm.RM_ExchangeHistorys?.Count ?? 0;
            model.TotalMember = model.Members?.Count ?? 0;
            model.TotalPlan = model.Plans?.Count ?? 0;
            model.TotalReviewHistory = reviewHistory.Count;

            return View(model);
        }

        /// <summary>
        /// Tải thông tin chi tiết của cơ hội kinh doanh cho tab tổng quan.
        /// </summary>
        /// <param name="id">Mã cơ hội kinh doanh cần lấy thông tin.</param>
        /// <returns>Partial view chi tiết hoặc kết quả rỗng khi dữ liệu không tồn tại.</returns>
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetBoHeader(int id)
        {
            var bo = _businessOpportunityCache.GetById(id);

            var members = _BuildMembers(id);
            var plans = _BuildPlans(id);

            var model = new RM_BusinessOpportunityOverviewModel
            {
                BusinessOpportunity = bo,
                ExchangeHistoryForm = _BuildExchangeHistoryForm(id),
                Members = members,
                Plans = plans
            };

            model.TotalExchangeHistory = model.ExchangeHistoryForm.RM_ExchangeHistorys?.Count ?? 0;
            model.TotalMember = model.Members?.Count ?? 0;
            model.TotalPlan = model.Plans?.Count ?? 0;

            return PartialView("_BoHeader", model);
        }

        /// <summary>
        /// Tải thông tin chi tiết của cơ hội kinh doanh cho tab tổng quan.
        /// </summary>
        /// <param name="id">Mã cơ hội kinh doanh cần lấy thông tin.</param>
        /// <returns>Partial view chi tiết hoặc kết quả rỗng khi dữ liệu không tồn tại.</returns>
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetBoInfo(int id)
        {
            var bo = _businessOpportunityCache.GetById(id);
            if (bo == null) return new EmptyResult();
            var files = _businessOpportunityFilePathCache.GetByBOID(bo.BusinessOpportunityID);
            bo.ExistingFiles = files.Select(f => new RM_BusinessOpportunityFilePathModel
            {
                FilePathID = f.FilePathID,
                FilePath = f.FilePath
            }).ToList();
            _EnrichBusinessOpportunity(bo);
            return PartialView(_viewRoot + "_DetailInfo.cshtml", bo);
        }

        /// <summary>
        /// Tải danh sách lịch sử trao đổi của cơ hội kinh doanh.
        /// </summary>
        /// <param name="id">Mã cơ hội kinh doanh cần lấy lịch sử trao đổi.</param>
        /// <returns>Partial view lịch sử trao đổi.</returns>
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetBoHistory(int id)
        {
            var model = _BuildExchangeHistoryForm(id);
            return PartialView(_viewRootRM + "_LichSuPhieu.cshtml", model);
        }

        /// <summary>
        /// Tải danh sách thành viên tham gia cơ hội kinh doanh.
        /// </summary>
        /// <param name="id">Mã cơ hội kinh doanh cần lấy danh sách thành viên.</param>
        /// <returns>Partial view danh sách thành viên.</returns>
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetBoMembers(int id)
        {
            var members = _BuildMembers(id);
            ViewData["BusinessOpportunityID"] = id;
            return PartialView(_viewRoot + "_Members.cshtml", members);
        }

        #endregion

        #region Kế hoạch cơ hội kinh doanh

        /// <summary>
        /// Load partial danh sách kế hoạch cho tab Overview
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetBoPlans(int id)
        {
            var data = _BuildPlans(id);
            ViewData["BusinessOpportunityID"] = id;
            return PartialView(_viewRoot + "_Plans.cshtml", data);
        }

        /// <summary>
        /// Mở form thêm mới kế hoạch
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult AddPlan(int businessOpportunityId)
        {
            var model = new RM_OpportunityPlanModel
            {
                BusinessOpportunityID = businessOpportunityId,
                Username = User.UserName
            };

            _PreparePlanRelatedPersons(model);
            return PartialView(_viewRoot + "_PlanForm.cshtml", model);
        }

        /// <summary>
        /// Mở form sửa kế hoạch theo Id
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditPlan(int id)
        {
            var model = _opportunityPlanCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = false,
                    message = CreateMessage(_opportunityPlanTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                }, JsonRequestBehavior.AllowGet);

            _PreparePlanRelatedPersons(model);
            return PartialView(_viewRoot + "_PlanForm.cshtml", model);
        }

        /// <summary>
        /// Lưu kế hoạch cơ hội kinh doanh và đồng bộ reminder, người liên quan, email thông báo.
        /// </summary>
        /// <param name="model">Dữ liệu kế hoạch cần thêm mới hoặc cập nhật.</param>
        /// <returns>Kết quả xử lý lưu kế hoạch.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult SavePlan(RM_OpportunityPlanModel model)
        {
            if (!ModelState.IsValid)
            {
                _PreparePlanRelatedPersons(model);
                return PartialView(_viewRoot + "_PlanForm.cshtml", model);
            }

            model.Username = User.UserName;

            var isNewPlan = model.Id <= 0;
            var result = _opportunityPlanCache.Save(model, User.UserName);
            var planId = _ResolveSavedPlanId(model, result);

            if (result > 0 && planId > 0)
            {
                _SavePlanRelatedPersons(planId, model.RelatedPersonUsernames);

                if (model.WorkingDate.HasValue)
                {
                    if (isNewPlan)
                    {
                        _opportunityPlanReminderCache.SeedForPlan(planId, model.WorkingDate.Value, User.UserName);
                        _opportunityPlanMailService.QueueSendPlanCreatedMail(planId, User.UserName);
                        var user = _userCache.GetByUserName(User.UserName);
                        _smsQueueService.QueueSendByTemplateCode(_smsTemplateKHKDCreated, new List<string> { user.Phone }, User.UserName, model);
                    }
                    else
                    {
                        _opportunityPlanReminderCache.UpdateForPlan(planId, model.WorkingDate.Value, User.UserName);
                    }
                }
            }

            var processType = isNewPlan ? EnumProcessType.Add : EnumProcessType.Edit;

            return Json(new
            {
                status = result > 0,
                message = CreateMessage(
                    _opportunityPlanTitle,
                    processType,
                    result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error)
            });
        }

        /// <summary>
        /// Mở form xác nhận xóa kế hoạch
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeletePlan(int id)
        {
            var model = _opportunityPlanCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = false,
                    message = CreateMessage(_opportunityPlanTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                }, JsonRequestBehavior.AllowGet);

            return PartialView(_viewRoot + "_PlanDelete.cshtml", model);
        }

        /// <summary>
        /// Xóa mềm kế hoạch cơ hội kinh doanh và đồng thời xóa các reminder liên quan.
        /// </summary>
        /// <param name="model">Dữ liệu kế hoạch cần xóa.</param>
        /// <returns>Kết quả xử lý xóa kế hoạch.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeletePlan(RM_OpportunityPlanModel model)
        {
            var result = _opportunityPlanCache.Delete(model.Id, User.UserName);

            if (result > 0)
            {
                _opportunityPlanReminderCache.DeleteForPlan(model.Id, User.UserName);
            }

            return Json(new
            {
                status = result > 0,
                message = CreateMessage(
                    _opportunityPlanTitle,
                    EnumProcessType.Delete,
                    result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error)
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Chuẩn bị danh sách người liên quan hiện có và danh sách người dùng có thể thêm cho form kế hoạch.
        /// </summary>
        /// <param name="model">Dữ liệu kế hoạch cần bổ sung người liên quan.</param>
        private void _PreparePlanRelatedPersons(RM_OpportunityPlanModel model)
        {
            int total;
            var relatedPersons = model.Id > 0
                ? _opportunityPlanRelatedPersonCache.GetByOpportunityPlanID(model.Id, out total, null)
                : new List<RM_OpportunityPlanRelatedPersonModel>();

            model.RelatedPersons = relatedPersons ?? new List<RM_OpportunityPlanRelatedPersonModel>();

            model.ExistingRelatedPersonUsernames = model.RelatedPersons
                .Where(x => !string.IsNullOrEmpty(x.Username))
                .Select(x => x.Username)
                .ToList();

            model.RelatedPersonUsers = _BuildRelatedPersonUsers(model.ExistingRelatedPersonUsernames);
        }

        /// <summary>
        /// Tạo danh sách người dùng có thể chọn làm người liên quan, loại trừ các username đã tồn tại.
        /// </summary>
        /// <param name="existingUsernames">Danh sách username đã được gắn vào kế hoạch.</param>
        /// <returns>Danh sách người dùng còn có thể chọn.</returns>
        private List<RelatedPersonUserModel> _BuildRelatedPersonUsers(List<string> existingUsernames)
        {
            existingUsernames = existingUsernames ?? new List<string>();

            return (_employeeCache.GetAll() ?? new List<MN_EmployeeModel>())
                .Where(x => !string.IsNullOrEmpty(x.Employee_Code)
                    && !existingUsernames.Contains(x.Employee_Code, StringComparer.OrdinalIgnoreCase))
                .Select(x => new RelatedPersonUserModel
                {
                    UserName = x.Employee_Code,
                    FullName = x.FullName,
                    OfficeName = x.TenBoPhan
                })
                .OrderBy(x => x.FullName)
                .ToList();
        }

        /// <summary>
        /// Lưu danh sách người liên quan được chọn cho kế hoạch sau khi thêm mới hoặc cập nhật.
        /// </summary>
        /// <param name="planId">Mã kế hoạch cần lưu người liên quan.</param>
        /// <param name="usernames">Danh sách username người liên quan.</param>
        private void _SavePlanRelatedPersons(int planId, string usernames)
        {
            if (planId <= 0 || string.IsNullOrWhiteSpace(usernames)) return;

            var model = new RM_OpportunityPlanRelatedPersonFormModel
            {
                OpportunityPlanID = planId,
                Usernames = usernames
            };

            _opportunityPlanRelatedPersonCache.SaveMulti(model, User.UserName);
        }

        /// <summary>
        /// Xây dựng danh sách kế hoạch của cơ hội kinh doanh kèm thông tin người liên quan.
        /// </summary>
        /// <param name="businessOpportunityId">Mã cơ hội kinh doanh cần lấy kế hoạch.</param>
        /// <returns>Danh sách kế hoạch đã bổ sung người liên quan.</returns>
        private List<RM_OpportunityPlanModel> _BuildPlans(int businessOpportunityId)
        {
            var plans = _opportunityPlanCache.GetByOpportunityId(businessOpportunityId)
                ?? new List<RM_OpportunityPlanModel>();

            foreach (var plan in plans)
            {
                int total;
                plan.RelatedPersons = _opportunityPlanRelatedPersonCache
                    .GetByOpportunityPlanID(plan.Id, out total, null)
                    ?? new List<RM_OpportunityPlanRelatedPersonModel>();
            }

            return plans;
        }

        /// <summary>
        /// Xác định lại mã kế hoạch vừa lưu trong trường hợp thêm mới.
        /// </summary>
        /// <param name="model">Dữ liệu kế hoạch vừa gửi lưu.</param>
        /// <param name="saveResult">Kết quả trả về từ cache khi lưu kế hoạch.</param>
        /// <returns>Mã kế hoạch đã lưu thành công.</returns>
        private int _ResolveSavedPlanId(RM_OpportunityPlanModel model, int saveResult)
        {
            if (model.Id > 0) return model.Id;
            if (saveResult <= 0) return 0;

            var plans = _opportunityPlanCache.GetByOpportunityId(model.BusinessOpportunityID);
            var savedPlan = plans?
                .Where(x => string.Equals(x.Username, model.Username, StringComparison.OrdinalIgnoreCase)
                    && x.WorkingDate == model.WorkingDate
                    && x.PlanName == model.PlanName)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            return savedPlan?.Id ?? (saveResult > 1 ? saveResult : 0);
        }

        /// <summary>
        /// Bổ sung danh sách tên sản phẩm dịch vụ từ chuỗi mã sản phẩm của cơ hội kinh doanh.
        /// </summary>
        /// <param name="bo">Dữ liệu cơ hội kinh doanh cần làm giàu thông tin.</param>
        private void _EnrichBusinessOpportunity(RM_BusinessOpportunityModel bo)
        {
            if (string.IsNullOrEmpty(bo.ProductServiceIDs)) return;

            var productServiceList = _productServiceCache.GetAll();
            var ids = bo.ProductServiceIDs
                .Split(';')
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(int.Parse)
                .ToList();

            bo.ProductServiceNames = string.Join(";", productServiceList
                .Where(p => ids.Contains(p.pID))
                .Select(p => (p.DisplayName ?? string.Empty)
                    .Replace("&nbsp;", string.Empty)
                    .Trim()));
        }

        /// <summary>
        /// Xây dựng danh sách thành viên của cơ hội kinh doanh kèm vai trò và thứ tự ưu tiên hiển thị.
        /// </summary>
        /// <param name="businessOpportunityId">Mã cơ hội kinh doanh cần lấy thành viên.</param>
        /// <returns>Danh sách thành viên đã bổ sung vai trò.</returns>
        private List<RM_SalesTeamMembersModel> _BuildMembers(int businessOpportunityId)
        {
            int total;
            var rawMembers = _salesTeamMembersCache.GetByBusinessOpportunityID(
                businessOpportunityId, out total, null);

            if (rawMembers == null || !rawMembers.Any())
                return new List<RM_SalesTeamMembersModel>();

            var allRoles = _rolesCache.GetAll();

            foreach (var m in rawMembers)
            {
                if (string.IsNullOrEmpty(m.Employee_Name) && !string.IsNullOrEmpty(m.FullName))
                    m.Employee_Name = m.FullName;

                if (string.IsNullOrEmpty(m.RoleID)) continue;

                var roleIds = m.RoleID
                    .Split(';')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(int.Parse)
                    .ToList();

                m.Roles = allRoles
                    .Where(r => roleIds.Contains(r.RoleID))
                    .ToList();
            }

            return rawMembers
                .OrderBy(m =>
                    m.Roles != null && m.Roles.Any(r => r.RoleID == 1) ? 0 :
                    m.Roles != null && m.Roles.Any(r => r.RoleID == 5) ? 1 : 2)
                .ToList();
        }

        /// <summary>
        /// Xây dựng model lịch sử trao đổi kèm file đính kèm cho cơ hội kinh doanh.
        /// </summary>
        /// <param name="businessOpportunityId">Mã cơ hội kinh doanh cần lấy lịch sử trao đổi.</param>
        /// <returns>Model lịch sử trao đổi phục vụ tab overview.</returns>
        private RM_ExchangeHistoryFormModel _BuildExchangeHistoryForm(int businessOpportunityId)
        {
            int total;

            var filePaths = _exchangeHistoryFilePathCache.Get(
                new RM_ExchangeHistoryFilePathSearchModel { BusinessOpportunityID = businessOpportunityId },
                out total, null);

            var histories = _exchangeHistoryCache.Get(
                new RM_ExchangeHistorySearchModel { BusinessOpportunityID = businessOpportunityId },
                out total, null);

            if (histories != null && histories.Count > 0)
            {
                int index = 1;
                foreach (var item in histories)
                {
                    item.Index = index.ToString();
                    item.ExchangeHistoryFilePath = filePaths
                        .Where(x => x.ExchangeHistoryID == item.ExchangeHistoryID)
                        .ToList();
                    index++;
                }
            }

            return new RM_ExchangeHistoryFormModel
            {
                RM_ExchangeHistorys = histories ?? new List<RM_ExchangeHistoryModel>(),
                RM_ExchangeHistory = new RM_ExchangeHistoryModel
                {
                    BusinessOpportunityID = businessOpportunityId,
                    ExchangeDate = DateTime.Now
                }
            };
        }

        #endregion

        /// <summary>
        /// Xây dựng model lịch sử rà soát kèm file đính kèm cho cơ hội kinh doanh.
        /// </summary>
        /// <returns>Model lịch sử rà soát phục vụ tab overview.</returns>
        private List<RM_ReviewHistoryModel> _BuildReviewHistoryForm(int objectType, int objectID)
        {
            // Lấy lịch sử rà soát
            var histories = _reviewBatchItemCache.GetHistory(objectType, objectID);

            var result = new List<RM_ReviewHistoryModel>();

            if (histories != null && histories.Count > 0)
            {
                int index = 1;

                foreach (var item in histories)
                {
                    // File đính kèm của từng lịch sử rà soát
                    var files = _reviewBatchItemBiz.GetFilePaths(item.ReviewHistoryID);

                    result.Add(new RM_ReviewHistoryModel
                    {
                        ReviewBatchItemID = item.ReviewBatchItemID,
                        ReviewBatchID = item.ReviewBatchID,
                        BatchCode = item.BatchCode,
                        BatchName = item.BatchName,
                        ReviewLevel = item.ReviewLevel,
                        ReviewComment = item.ReviewComment,
                        IsConfirmed = item.IsConfirmed,
                        ExistingFiles = files ?? new List<RM_ReviewBatchFilePathModel>(),
                        ReviewHistoryID = item.ReviewHistoryID,
                        Reviewer = item.Reviewer,
                        CanEdit = item.CreatedBy == User.UserName,
                        CreatedDate = item.CreatedDate
                    });

                    index++;
                }
            }
            return result;
        }

        [HttpGet]
        public ActionResult GetReviewHistory(int id)
        {
            var data = _BuildReviewHistoryForm(1, id);

            return PartialView("_ReviewHistory", data);
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        [AjaxOnly]
        public ActionResult Notification(int id)
        {
            var model = _businessOpportunityCache.GetById(id);
            return PartialView("_Notification", model);
        }
    }
}
