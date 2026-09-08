using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;
using System.Linq;
using TSFramework.Libs.Providers;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class ProductServiceGroupFilePathController : AppController
    {
        private readonly Cate_ProductServiceCache _productServiceCache;
        private readonly RM_ProductServiceFilePathCache _productServiceFilePathCache;
        private readonly RM_ProductServiceGroupFilePathCache _productServiceGroupFilePathCache;
        private readonly string _productServiceGroupFilePathTitle = AppProcessor.Messagor.GetMessage("ProductServiceGroupFilePath_Title");
        private readonly string _productServiceTitle = AppProcessor.Messagor.GetMessage("ProductService_Title");
        private readonly string _folderFile = "/Contents/Files/ProductService";

        public ProductServiceGroupFilePathController()
        {
            _productServiceCache = new Cate_ProductServiceCache();
            _productServiceFilePathCache = new RM_ProductServiceFilePathCache();
            _productServiceGroupFilePathCache = new RM_ProductServiceGroupFilePathCache();
        }

        /// <summary>
        /// Hiển thị màn hình danh sách sản phẩm dịch vụ.
        /// </summary>
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult List(int ProductServiceID)
        {
            var model = _productServiceCache.GetById(ProductServiceID);
            if (model == null)
            {
                return Json(
                    new
                    {
                        status = false,
                        message = CreateMessage(_productServiceTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
            }
            ViewBag.Title = _productServiceGroupFilePathTitle + " " + model.NameProduct;
            return PartialView("_List", model);
        }

        /// <summary>
        /// Tải danh sách sản phẩm dịch vụ để hiển thị lên lưới.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(Cate_ProductServiceModel model)
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
            var data = _productServiceGroupFilePathCache.Get(out var total, model, dataSearch);

            return Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị popup thêm mới sản phẩm dịch vụ.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(int ProductServiceID)
        {
            var psModel = _productServiceCache.GetById(ProductServiceID);
            if (psModel == null)
            {
                return Json(
                    new
                    {
                        status = false,
                        message = CreateMessage(_productServiceTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
            }


            var model = new RM_ProductServiceGroupFilePathModel
            {
                ProductServiceID = ProductServiceID
            };
            return PartialView("_Add", model);
        }

        /// <summary>
        /// Xử lý thêm mới sản phẩm dịch vụ.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_ProductServiceGroupFilePathModel model)
        {
            var listFile = Request.Files;
            if (listFile.Count != 0) { ModelState.Remove("ListFiles"); }
            if (!ModelState.IsValid)
            {
                return PartialView("_ProductServiceGroupFilePath", model);
            }
            if (listFile.Count != 0)
            {
                List<HttpPostedFileBase> postedFiles = new List<HttpPostedFileBase>();
                // Truy cập theo index i từ 0 đến hết để lấy chính xác từng file riêng biệt
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        postedFiles.Add(file);
                    }
                }
                model.ListFiles = FilenamesSaved(postedFiles);
            }

            var result = _productServiceGroupFilePathCache.Save(model, User.UserName);

            string response;
            string title = UtilString.TruncateText(model.Title, 50);

            if (result == -9)
                response = CreateMessage($"{_productServiceGroupFilePathTitle} {title}",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_productServiceGroupFilePathTitle} {title}",
                    EnumProcessType.Add,
                    result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = result > 0, message = response }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Hiển thị popup cập nhật sản phẩm dịch vụ.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _productServiceGroupFilePathCache.GetById(id);
            if (model == null)
            {
                return Json(
                    new
                    {
                        status = false,
                        message = CreateMessage(_productServiceGroupFilePathTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
            }
            return PartialView("_Edit", model);
        }

        /// <summary>
        /// Xử lý cập nhật sản phẩm dịch vụ.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_ProductServiceGroupFilePathModel model)
        {
            var listFile = Request.Files;
            var slFileDangCo = _productServiceFilePathCache.GetAll(model.GroupFilePathID)?.Count() ?? 0;
            if (listFile.Count != 0 || slFileDangCo != 0) { ModelState.Remove("ListFiles"); }
            if (!ModelState.IsValid)
            {
                return PartialView("_ProductServiceGroupFilePath", model);
            }
            if(listFile.Count != 0)
            {
                List<HttpPostedFileBase> postedFiles = new List<HttpPostedFileBase>();
                // Truy cập theo index i từ 0 đến hết để lấy chính xác từng file riêng biệt
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        postedFiles.Add(file);
                    }
                }
                model.ListFiles = FilenamesSaved(postedFiles);
            }

            var result = _productServiceGroupFilePathCache.Save(model, User.UserName);

            string response;
            string title = UtilString.TruncateText(model.Title, 50);

            if (result == -9)
                response = CreateMessage($"{_productServiceGroupFilePathTitle} {title}",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_productServiceGroupFilePathTitle} {title}",
                    EnumProcessType.Edit,
                    result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = result > 0, message = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị popup xác nhận xóa sản phẩm dịch vụ.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _productServiceGroupFilePathCache.GetById(id);
            if (model == null)
            {
                return Json(
                    new
                    {
                        status = false,
                        message = CreateMessage(_productServiceGroupFilePathTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
            }

            string title = UtilString.TruncateText(model.Title, 100);
            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                string.Format("{0} [{1}]", _productServiceGroupFilePathTitle, title));

            return PartialView("_Delete", model);
        }

        /// <summary>
        /// Xử lý xóa sản phẩm dịch vụ.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        [ValidateInput(false)]
        public ActionResult Delete(RM_ProductServiceGroupFilePathModel model)
        {
            var deleted = _productServiceGroupFilePathCache.Delete(model, User.UserName);
            string title = UtilString.TruncateText(model.Title, 50);
            var response = CreateMessage(
                string.Format("{0} [{1}]", _productServiceTitle, title),
                EnumProcessType.Delete,
                deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);

            return Json(new { status = deleted > 0, message = response });
        }

        private string FilenamesSaved(List<HttpPostedFileBase> files)
        {
            List<string> FilePahtAttachs = new List<string>();
            if (files == null || files.Count == 0) return string.Empty;

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file != null)
                {
                    var result = new FileProvider().UploadFile(file, _folderFile);
                    if (result.ErrorCode == 1) { FilePahtAttachs.Add(Path.Combine(_folderFile, result.FileName)); }
                }
            }
            return string.Join(",", FilePahtAttachs);
        }

        #region File 
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult GetFilePathsByGroupFilePathID(int id, string mode = "")
        {
            var data = _productServiceFilePathCache.GetAll(id);
            ViewBag.Action = mode;
            return PartialView("_FilePaths", data);
        }
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult DeleteFilePaths(int id)
        {
            var file = _productServiceFilePathCache.GetById(id);
            if (file == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"Tập tin",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            var result = _productServiceFilePathCache.Delete(id, "");
            var response = CreateMessage($"Tập tin [{Path.GetFileName(Path.GetFileName(file.FilePath))}]",
              EnumProcessType.Delete, result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        /// <summary>
        /// Hiển thị popup xác nhận xóa sản phẩm dịch vụ.
        /// </summary>
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult ViewFile(int FilePathID)
        {
            var model = _productServiceFilePathCache.GetById(FilePathID);
            if (model == null)
            {
                return Json(
                    new
                    {
                        status = false,
                        message = CreateMessage("Tập tin", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
            }
            return PartialView("_ViewFile", model);
        }
        #endregion

    }
}
