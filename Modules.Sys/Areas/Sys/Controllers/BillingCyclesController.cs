using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using System;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Sys.Areas.Sys.Controllers
{
    public class BillingCyclesController : AppController
    {
        private readonly RM_BillingCyclesCache _BillingCyclesCache;
        private readonly string _BillingCyclesTitle = AppProcessor.Messagor.GetMessage("BillingCycles_Title");

        /// <summary>
        /// Khởi tạo cache phục vụ xử lý dự án.
        /// </summary>
        public BillingCyclesController()
        {
            _BillingCyclesCache = new RM_BillingCyclesCache();
        }

        /// <summary>
        /// Hiển thị màn hình danh sách dự án và khởi tạo bộ lọc tìm kiếm.
        /// </summary>
        /// <returns>Màn hình danh sách dự án.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Trả danh sách dự án theo điều kiện tìm kiếm và phân trang.
        /// </summary>
        /// <param name="model">Điều kiện tìm kiếm dự án.</param>
        /// <returns>Dữ liệu JSON cho DataTable.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get()
        {
            var draw = Request.Form.GetValues("draw")?[0];
            var order = Request.Form.GetValues("order[0][column]")?[0];
            var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0]);
            var search = Request.Form.GetValues("search[value]")?[0];

            var dataSearch = new BaseSearchModel
            {
                Search = string.IsNullOrEmpty(search) ? null : search,
                Order = order,
                OrderDir = orderDir,
                StartIndex = startRec,
                PageSize = pageSize
            };
            var data = _BillingCyclesCache.Get(out var total, dataSearch);
            return Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị màn hình thêm mới dự án.
        /// </summary>
        /// <returns>Popup thêm mới dự án.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add()
        {
            var model = new RM_BillingCyclesModel();
            return PartialView("_Add", model);
        }

        /// <summary>
        /// Lưu thông tin dự án mới từ màn hình.
        /// </summary>
        /// <param name="model">Dữ liệu dự án cần lưu.</param>
        /// <returns>Kết quả xử lý.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_BillingCyclesModel model)
        {
            if (model.Islimited == false)
            {
                ModelState.Remove("Quantity");
                ModelState.Remove("CycleType");
            }
            if (!ModelState.IsValid)
            {
                return PartialView("_BillingCycles", model);
            }
            var result = _BillingCyclesCache.Save(model, User.UserName);
            string response;
            if (result == 0) response = CreateMessage($"{_BillingCyclesTitle} [{model.CycleName}]", EnumProcessType.Add, EnumMsgIcon.Error);
            else if (result == -9) response = CreateMessage($"{_BillingCyclesTitle} [{model.CycleName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else response = CreateMessage($"{_BillingCyclesTitle} [{model.CycleName}]", EnumProcessType.Add, EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị màn hình cập nhật dự án theo mã dữ liệu được chọn.
        /// </summary>
        /// <param name="id">Mã dự án cần cập nhật.</param>
        /// <returns>Popup cập nhật dự án hoặc thông báo khi dữ liệu không tồn tại.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _BillingCyclesCache.GetById(id);
            if (model == null)
            {
                return Json(new { status = true, message = CreateMessage(_BillingCyclesTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            }
            return PartialView("_Edit", model);
        }

        /// <summary>
        /// Cập nhật thông tin dự án theo dữ liệu từ màn hình.
        /// </summary>
        /// <param name="model">Dữ liệu dự án cần cập nhật.</param>
        /// <returns>Kết quả xử lý.</returns>
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_BillingCyclesModel model)
        {
            if (model.Islimited == false)
            {
                ModelState.Remove("Quantity");
                ModelState.Remove("CycleType");
            }
            if (!ModelState.IsValid)
            {
                return PartialView("_BillingCycles", model);
            }
            var result = _BillingCyclesCache.Save(model, User.UserName);
            string response;
            if (result == 0) response = CreateMessage($"{_BillingCyclesTitle} [{model.CycleName}]", EnumProcessType.Edit, EnumMsgIcon.Error);
            else if (result == -9) response = CreateMessage($"{_BillingCyclesTitle} [{model.CycleName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else response = CreateMessage($"{_BillingCyclesTitle} [{model.CycleName}]", EnumProcessType.Edit, EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị màn hình xác nhận xóa dự án theo mã dữ liệu được chọn.
        /// </summary>
        /// <param name="id">Mã dự án cần xóa.</param>
        /// <returns>Popup xác nhận xóa hoặc thông báo khi dữ liệu không tồn tại.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _BillingCyclesCache.GetById(id);
            if (model == null)
                return Json(new { status = true, message = CreateMessage(_BillingCyclesTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_BillingCyclesTitle} [{model.CycleName}]");
            return PartialView("_Delete", model);
        }

        /// <summary>
        /// Xóa thông tin dự án theo dữ liệu được chọn.
        /// </summary>
        /// <param name="model">Dữ liệu dự án cần xóa.</param>
        /// <returns>Kết quả xử lý.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_BillingCyclesModel model)
        {
            var deleted = _BillingCyclesCache.Delete(model, User.UserName);
            var response = CreateMessage($"{_BillingCyclesTitle} [{model.CycleName}]",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
    }
}