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
    /// Xử lý các luồng thêm, sửa, xóa và tra cứu sản phẩm dự án.
    /// </summary>
    public class ProductProjectController : AppController
    {
        private readonly RM_ProductProjectCache _productProjectCache;
        private readonly RM_ProjectCache _projectCache;
        private readonly Cate_ProductServiceCache _productServiceCache;
        private readonly string _ProductProjectTitle = AppProcessor.Messagor.GetMessage("ProductProject_Title");

        /// <summary>
        /// Khởi tạo cache phục vụ xử lý sản phẩm dự án.
        /// </summary>
        public ProductProjectController()
        {
            _productProjectCache = new RM_ProductProjectCache();
            _projectCache = new RM_ProjectCache();
            _productServiceCache = new Cate_ProductServiceCache();
        }

        /// <summary>
        /// Hiển thị màn hình danh sách sản phẩm dự án.
        /// </summary>
        /// <returns>Màn hình danh sách sản phẩm dự án.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Trả danh sách sản phẩm dự án phục vụ hiển thị lưới dữ liệu.
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
            var data = _productProjectCache.Get(out var total, dataSearch);
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        /// <summary>
        /// Hiển thị màn hình thêm mới sản phẩm dự án và khởi tạo dự án nếu có.
        /// </summary>
        /// <param name="id">Mã dự án cần khởi tạo dữ liệu sản phẩm dự án.</param>
        /// <returns>Popup thêm mới sản phẩm dự án.</returns>
        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult Add(int? id)
        {
            var model = new RM_ProductProjectModel();
            if (id.HasValue)
            {
                model.ProjectID = id.Value;
            }
            model.ListProject = _projectCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.ProjectName,
                        Value = d.ProjectID.ToString()
                    }).ToList();
            model.ListProductService = _productServiceCache.GetAll();
            return PartialView("_Add", model);
        }

        /// <summary>
        /// Lưu thông tin sản phẩm dự án từ màn hình.
        /// </summary>
        /// <param name="model">Dữ liệu sản phẩm dự án cần lưu.</param>
        /// <returns>Kết quả xử lý.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_ProductProjectModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListProject = _projectCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.ProjectName,
                        Value = d.ProjectID.ToString()
                    }).ToList();
                model.ListProductService = _productServiceCache.GetAll();
                return PartialView("_ProductProject", model);
            }

            string response;

            var result = _productProjectCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_ProductProjectTitle}",
                    EnumProcessType.Add,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_ProductProjectTitle}",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_ProductProjectTitle}",
                    EnumProcessType.Add,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị màn hình cập nhật sản phẩm dự án theo mã dữ liệu được chọn.
        /// </summary>
        /// <param name="id">Mã sản phẩm dự án cần cập nhật.</param>
        /// <returns>Popup cập nhật sản phẩm dự án hoặc thông báo khi dữ liệu không tồn tại.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _productProjectCache.GetById(id);
            if (model == null)
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_ProductProjectTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            model.ListProject = _projectCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.ProjectName,
                        Value = d.ProjectID.ToString()
                    }).ToList();
            model.ListProductService = _productServiceCache.GetAll();

            return PartialView("_Edit", model);
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm dự án theo dữ liệu từ màn hình.
        /// </summary>
        /// <param name="model">Dữ liệu sản phẩm dự án cần cập nhật.</param>
        /// <returns>Kết quả xử lý.</returns>
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_ProductProjectModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListProject = _projectCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.ProjectName,
                        Value = d.ProjectID.ToString()
                    }).ToList();
                model.ListProductService = _productServiceCache.GetAll();
                return PartialView("_ProductProject", model);
            }
            string response;

            var result = _productProjectCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_ProductProjectTitle}",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_ProductProjectTitle}",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_ProductProjectTitle}",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị màn hình xác nhận xóa sản phẩm dự án theo mã dữ liệu được chọn.
        /// </summary>
        /// <param name="id">Mã sản phẩm dự án cần xóa.</param>
        /// <returns>Popup xác nhận xóa hoặc thông báo khi dữ liệu không tồn tại.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _productProjectCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_ProductProjectTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_ProductProjectTitle}");

            return PartialView("_Delete", model);
        }

        /// <summary>
        /// Xóa thông tin sản phẩm dự án theo dữ liệu được chọn.
        /// </summary>
        /// <param name="model">Dữ liệu sản phẩm dự án cần xóa.</param>
        /// <returns>Kết quả xử lý.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_ProductProjectModel model)
        {
            var deleted = _productProjectCache.Delete(model, User.UserName);

            var response = CreateMessage($"{_ProductProjectTitle}",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
    }
}
