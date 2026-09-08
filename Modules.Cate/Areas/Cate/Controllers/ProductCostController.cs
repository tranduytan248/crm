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
    /// Xử lý các luồng thêm, sửa, xóa và tra cứu chi phí của sản phẩm dự án.
    /// </summary>
    public class ProductCostController : AppController
    {
        private readonly RM_ProductCostCache _productCostCache;
        private readonly RM_ProductProjectCache _productProjectCache;
        private readonly RM_CostTypeCache _costTypeCache;
        private readonly string _ProductCostTitle = AppProcessor.Messagor.GetMessage("ProductCost_Title");

        /// <summary>
        /// Khởi tạo cache phục vụ xử lý chi phí sản phẩm dự án.
        /// </summary>
        public ProductCostController()
        {
            _productCostCache = new RM_ProductCostCache();
            _productProjectCache = new RM_ProductProjectCache();
            _costTypeCache = new RM_CostTypeCache();
        }

        /// <summary>
        /// Hiển thị màn hình danh sách chi phí sản phẩm dự án.
        /// </summary>
        /// <returns>Màn hình danh sách chi phí sản phẩm dự án.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Trả danh sách chi phí sản phẩm dự án phục vụ hiển thị lưới dữ liệu.
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
            var data = _productCostCache.Get(out var total, dataSearch);
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        /// <summary>
        /// Hiển thị màn hình thêm mới chi phí và khởi tạo sẵn sản phẩm dự án nếu có.
        /// </summary>
        /// <param name="id">Mã sản phẩm dự án cần khởi tạo dữ liệu.</param>
        /// <returns>Popup thêm mới chi phí sản phẩm dự án.</returns>
        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult Add(int? id)
        {
            var model = new RM_ProductCostModel();
            if (id.HasValue)
            {
                var data = _productProjectCache.GetById(id.Value);
                if (data == null)
                {
                    return Json(new
                    {
                        status = true,
                        message = CreateMessage(_ProductCostTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
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
            model.ListCostType = _costTypeCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.CostTypeName,
                        Value = d.CostTypeID.ToString()
                    }).ToList();
            return PartialView("_Add", model);
        }

        /// <summary>
        /// Lưu thông tin chi phí sản phẩm dự án từ màn hình.
        /// </summary>
        /// <param name="model">Dữ liệu chi phí sản phẩm dự án cần lưu.</param>
        /// <returns>Kết quả xử lý.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_ProductCostModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListProductProject = _productProjectCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.NameProduct,
                        Value = d.ProductProjectID.ToString()
                    }).ToList();
                model.ListCostType = _costTypeCache.GetAll()
                        .Select(d => new SelectListItem
                        {
                            Text = d.CostTypeName,
                            Value = d.CostTypeID.ToString()
                        }).ToList();
                return PartialView("_ProductCost", model);
            }

            string response;

            var result = _productCostCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_ProductCostTitle} [{model.NameProduct}]",
                    EnumProcessType.Add,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_ProductCostTitle} [{model.NameProduct}]",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_ProductCostTitle} [{model.NameProduct}]",
                    EnumProcessType.Add,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị màn hình cập nhật chi phí sản phẩm dự án theo mã dữ liệu được chọn.
        /// </summary>
        /// <param name="id">Mã chi phí sản phẩm dự án cần cập nhật.</param>
        /// <returns>Popup cập nhật chi phí hoặc thông báo khi dữ liệu không tồn tại.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _productCostCache.GetById(id);
            if (model == null)
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_ProductCostTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            model.ListProductProject = _productProjectCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.NameProduct,
                        Value = d.ProductProjectID.ToString()
                    }).ToList();
            model.ListCostType = _costTypeCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.CostTypeName,
                        Value = d.CostTypeID.ToString()
                    }).ToList();

            return PartialView("_Edit", model);
        }

        /// <summary>
        /// Cập nhật thông tin chi phí sản phẩm dự án theo dữ liệu từ màn hình.
        /// </summary>
        /// <param name="model">Dữ liệu chi phí sản phẩm dự án cần cập nhật.</param>
        /// <returns>Kết quả xử lý.</returns>
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_ProductCostModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListProductProject = _productProjectCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.NameProduct,
                        Value = d.ProductProjectID.ToString()
                    }).ToList();
                model.ListCostType = _costTypeCache.GetAll()
                        .Select(d => new SelectListItem
                        {
                            Text = d.CostTypeName,
                            Value = d.CostTypeID.ToString()
                        }).ToList();
                return PartialView("_ProductCost", model);
            }
            string response;

            var result = _productCostCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_ProductCostTitle} [{model.NameProduct}]",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_ProductCostTitle} [{model.NameProduct}]",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_ProductCostTitle} [{model.NameProduct}]",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị màn hình xác nhận xóa chi phí sản phẩm dự án theo mã dữ liệu được chọn.
        /// </summary>
        /// <param name="id">Mã chi phí sản phẩm dự án cần xóa.</param>
        /// <returns>Popup xác nhận xóa hoặc thông báo khi dữ liệu không tồn tại.</returns>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _productCostCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_ProductCostTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_ProductCostTitle} [{model.NameProduct}]");

            return PartialView("_Delete", model);
        }

        /// <summary>
        /// Xóa thông tin chi phí sản phẩm dự án theo dữ liệu được chọn.
        /// </summary>
        /// <param name="model">Dữ liệu chi phí sản phẩm dự án cần xóa.</param>
        /// <returns>Kết quả xử lý.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_ProductCostModel model)
        {
            var deleted = _productCostCache.Delete(model, User.UserName);

            var response = CreateMessage($"{_ProductCostTitle} [{model.NameProduct}]",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
    }
}
