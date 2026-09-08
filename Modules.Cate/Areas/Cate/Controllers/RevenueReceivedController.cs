using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using System;
using System.Linq;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    /// <summary>
    /// Xử lý các luồng thêm, sửa, xóa và tra cứu doanh thu thực nhận của sản phẩm dự án.
    /// </summary>
    public class RevenueReceivedController : AppController
    {
        private readonly RM_RevenueReceivedCache _revenueReceivedCache;
        private readonly RM_ProductProjectCache _productProjectCache;
        private readonly string _RevenueReceivedTitle = AppProcessor.Messagor.GetMessage("RevenueReceived_Product_Title");

        /// <summary>
        /// Khởi tạo cache phục vụ xử lý doanh thu thực nhận.
        /// </summary>
        public RevenueReceivedController()
        {
            _revenueReceivedCache = new RM_RevenueReceivedCache();
            _productProjectCache = new RM_ProductProjectCache();
        }

        /// <summary>
        /// Hiển thị màn hình danh sách doanh thu thực nhận.
        /// </summary>
        /// <returns>Màn hình danh sách doanh thu thực nhận.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Trả danh sách doanh thu thực nhận phục vụ hiển thị lưới dữ liệu.
        /// </summary>
        /// <returns>Dữ liệu JSON cho DataTable.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get()
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
            var data = _revenueReceivedCache.Get(out var total, dataSearch);
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        /// <summary>
        /// Hiển thị màn hình thêm mới doanh thu thực nhận và khởi tạo dữ liệu sản phẩm dự án nếu có.
        /// </summary>
        /// <param name="id">Mã sản phẩm dự án cần khởi tạo sẵn.</param>
        /// <returns>Popup thêm mới doanh thu thực nhận.</returns>
        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult Add(int? id)
        {
            var model = new RM_RevenueReceivedModel();
            if (id.HasValue)
            {
                var data = _productProjectCache.GetById(id.Value);
                if (data == null)
                {
                    return Json(new
                    {
                        status = true,
                        message = CreateMessage(_RevenueReceivedTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
                }

                model.ProductProjectID = data.ProductProjectID;
                model.NameProduct = data.NameProduct;
            }
            model.ListProductProject = _productProjectCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.NameProduct,
                        Value = d.ProductProjectID.ToString()
                    }).ToList();
            return PartialView("_Add", model);
        }

        /// <summary>
        /// Lưu thông tin doanh thu thực nhận từ màn hình.
        /// </summary>
        /// <param name="model">Dữ liệu doanh thu thực nhận cần lưu.</param>
        /// <returns>Kết quả xử lý.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_RevenueReceivedModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListProductProject = _productProjectCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.NameProduct,
                        Value = d.ProductProjectID.ToString()
                    }).ToList();
                return PartialView("_RevenueReceived", model);
            }

            string response;

            var result = _revenueReceivedCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_RevenueReceivedTitle} [{model.NameProduct}]",
                    EnumProcessType.Add,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_RevenueReceivedTitle} [{model.NameProduct}]",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_RevenueReceivedTitle} [{model.NameProduct}]",
                    EnumProcessType.Add,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị màn hình cập nhật doanh thu thực nhận theo mã dữ liệu được chọn.
        /// </summary>
        /// <param name="id">Mã doanh thu thực nhận cần cập nhật.</param>
        /// <returns>Popup cập nhật doanh thu thực nhận hoặc thông báo khi dữ liệu không tồn tại.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _revenueReceivedCache.GetById(id);
            if (model == null)
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_RevenueReceivedTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            model.ListProductProject = _productProjectCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.NameProduct,
                        Value = d.ProductProjectID.ToString()
                    }).ToList();

            return PartialView("_Edit", model);
        }

        /// <summary>
        /// Cập nhật thông tin doanh thu thực nhận theo dữ liệu từ màn hình.
        /// </summary>
        /// <param name="model">Dữ liệu doanh thu thực nhận cần cập nhật.</param>
        /// <returns>Kết quả xử lý.</returns>
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_RevenueReceivedModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListProductProject = _productProjectCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.NameProduct,
                        Value = d.ProductProjectID.ToString()
                    }).ToList();
                return PartialView("_RevenueReceived", model);
            }
            string response;

            var result = _revenueReceivedCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_RevenueReceivedTitle} [{model.NameProduct}]",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_RevenueReceivedTitle} [{model.NameProduct}]",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_RevenueReceivedTitle} [{model.NameProduct}]",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị màn hình xác nhận xóa doanh thu thực nhận theo mã dữ liệu được chọn.
        /// </summary>
        /// <param name="id">Mã doanh thu thực nhận cần xóa.</param>
        /// <returns>Popup xác nhận xóa hoặc thông báo khi dữ liệu không tồn tại.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _revenueReceivedCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_RevenueReceivedTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_RevenueReceivedTitle} [{model.NameProduct}]");

            return PartialView("_Delete", model);
        }

        /// <summary>
        /// Xóa thông tin doanh thu thực nhận theo dữ liệu được chọn.
        /// </summary>
        /// <param name="model">Dữ liệu doanh thu thực nhận cần xóa.</param>
        /// <returns>Kết quả xử lý.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_RevenueReceivedModel model)
        {
            var deleted = _revenueReceivedCache.Delete(model, User.UserName);

            var response = CreateMessage($"{_RevenueReceivedTitle} [{model.NameProduct}]",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
    }
}
