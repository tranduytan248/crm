using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class DigitalSalesController : AppController
    {
        private readonly RM_DigitalSalesCache _salesCache;
        private readonly RM_CustomerCache _customerCache;
        private readonly Cate_ProductServiceCache _productServiceCache;
        private readonly MN_EmployeeCache _employeeCache;
        private readonly MN_BoPhanCache _departmentCache;
        private readonly RM_ContactPersonsCache _contactPersonCache;
        private readonly RM_ContractsCache _contractCache;
        private readonly SysUserCache _userCache;

        private readonly string _title = "Kinh doanh Sản phẩm Dịch vụ Số";
        private readonly string _folderUpload = ConfigurationManager.AppSettings["AppImageRoot_Path"] ?? "/Contents/File";

        public DigitalSalesController()
        {
            _salesCache = new RM_DigitalSalesCache();
            _customerCache = new RM_CustomerCache();
            _productServiceCache = new Cate_ProductServiceCache();
            _employeeCache = new MN_EmployeeCache();
            _departmentCache = new MN_BoPhanCache();
            _contactPersonCache = new RM_ContactPersonsCache();
            _contractCache = new RM_ContractsCache();
            _userCache = new SysUserCache();
        }

        #region 1. Danh sách & Tìm kiếm (List & DataTables)
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index(int? customerId, byte? businessType, int? statusId)
        {
            var model = new RM_DigitalSalesSearchModel
            {
                CustomerID = customerId.GetValueOrDefault(0),
                BusinessType = businessType.GetValueOrDefault(0),
                StatusID = statusId.GetValueOrDefault(0),
                PageNumber = 1,
                PageSize = 20
            };

            PrepareSearchDropdowns(model);
            ViewBag.Title = _title;
            return View(model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(RM_DigitalSalesSearchModel model)
        {
            var search = Request.Form.GetValues("search[value]")?[0];
            var draw = Request.Form.GetValues("draw")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0] ?? "0");
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0] ?? "20");

            if (!string.IsNullOrWhiteSpace(search))
            {
                model.Keyword = search;
            }

            model.PageNumber = (startRec / (pageSize <= 0 ? 20 : pageSize)) + 1;
            model.PageSize = pageSize <= 0 ? 20 : pageSize;
            model.UserName = User.UserName;

            var data = _salesCache.LoadList(out int total, model);

            return Json(new
            {
                draw = Convert.ToInt32(draw ?? "1"),
                recordsTotal = total,
                recordsFiltered = total,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 2. Thêm mới / Cập nhật (Add & Edit)
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(int? customerId, byte? businessType)
        {
            var model = new RM_DigitalSalesModel
            {
                BusinessType = businessType ?? 1,
                StatusID = 1, // Default: Chưa nắm bắt
                CustomerID = customerId.GetValueOrDefault(0),
                StartDate = DateTime.Today,
                ExpectedDate = DateTime.Today.AddMonths(1),
                ClosingProbability = 50
            };

            var currentUser = _userCache.GetByUserName(User.UserName);
            if (currentUser != null)
            {
                model.AssignedEmployeeID = currentUser.UserId;
            }

            PrepareSalesDropdowns(model);
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult Add(RM_DigitalSalesModel model, HttpPostedFileBase fileUpload)
        {
            if (model.CustomerID <= 0)
            {
                return Json(new
                {
                    status = false,
                    message = "Vui lòng chọn khách hàng!"
                });
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return Json(new
                {
                    status = false,
                    message = "Vui lòng nhập tên cơ hội / dự án!"
                });
            }

            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                model.FileAttach = SaveUploadedFile(fileUpload);
            }

            var id = _salesCache.Save(model, User.UserName);
            if (id > 0)
            {
                return Json(new
                {
                    status = true,
                    id = id,
                    message = CreateMessage(_title, EnumProcessType.Add, EnumMsgIcon.Success)
                });
            }

            return Json(new
            {
                status = false,
                message = CreateMessage(_title, EnumProcessType.Add, EnumMsgIcon.Error)
            });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _salesCache.GetByID(id);
            if (model == null)
            {
                return Json(new
                {
                    status = false,
                    message = CreateMessage(_title, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                }, JsonRequestBehavior.AllowGet);
            }

            PrepareSalesDropdowns(model);
            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_DigitalSalesModel model, HttpPostedFileBase fileUpload)
        {
            if (model.DigitalSalesID <= 0)
            {
                return Json(new
                {
                    status = false,
                    message = CreateMessage(_title, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                model.FileAttach = SaveUploadedFile(fileUpload);
            }

            var result = _salesCache.Save(model, User.UserName);
            if (result > 0)
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage(_title, EnumProcessType.Edit, EnumMsgIcon.Success)
                });
            }

            return Json(new
            {
                status = false,
                message = CreateMessage(_title, EnumProcessType.Edit, EnumMsgIcon.Error)
            });
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var result = _salesCache.Delete(id, User.UserName);
            if (result > 0)
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage(_title, EnumProcessType.Delete, EnumMsgIcon.Success)
                });
            }

            return Json(new
            {
                status = false,
                message = CreateMessage(_title, EnumProcessType.Delete, EnumMsgIcon.Error)
            });
        }
        #endregion

        #region 3. Chi tiết 360 độ (Detail View)
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Detail(int id)
        {
            var model = _salesCache.GetByID(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Title = $"Hồ sơ: {model.Code} - {model.Title}";
            ViewBag.UsersList = _userCache.GetAll()?.Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text = $"{u.FullName} ({u.UserName})"
            }).ToList() ?? new List<SelectListItem>();

            ViewBag.ProductList = _productServiceCache.GetAll()?.Select(p => new SelectListItem
            {
                Value = p.ProductServiceID.ToString(),
                Text = string.IsNullOrEmpty(p.ShortNameProduct) ? p.NameProduct : $"{p.ShortNameProduct} - {p.NameProduct}"
            }).ToList() ?? new List<SelectListItem>();

            return View(model);
        }
        #endregion

        #region 4. Chuyển đổi trạng thái (Change Status with Gatekeeper)
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult ChangeStatusModal(int id)
        {
            var sales = _salesCache.GetByID(id);
            if (sales == null)
            {
                return Json(new
                {
                    status = false,
                    message = CreateMessage(_title, EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                }, JsonRequestBehavior.AllowGet);
            }

            var allStatuses = _salesCache.GetStatusList(null);
            var model = new RM_DigitalSalesChangeStatusViewModel
            {
                DigitalSalesID = sales.DigitalSalesID,
                Title = sales.Title,
                CurrentBusinessType = sales.BusinessType,
                CurrentBusinessTypeName = sales.BusinessTypeName,
                CurrentStatusName = sales.StatusName,
                AvailableStatuses = allStatuses.Select(s => new SelectListItem
                {
                    Value = s.StatusID.ToString(),
                    Text = $"[{(s.BusinessType == 1 ? "Cơ hội" : "Dự án")}] {s.StatusName}" + (s.StatusID == sales.StatusID ? " (Hiện tại)" : "")
                }).ToList()
            };

            return PartialView("_ChangeStatusModal", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult ChangeStatus(int digitalSalesId, int newStatusId, string note, HttpPostedFileBase attachmentFile)
        {
            if (digitalSalesId <= 0 || newStatusId <= 0)
            {
                return Json(new
                {
                    status = false,
                    message = "Dữ liệu không hợp lệ!"
                });
            }

            string attachmentPath = null;
            if (attachmentFile != null && attachmentFile.ContentLength > 0)
            {
                attachmentPath = SaveUploadedFile(attachmentFile);
            }

            var code = _salesCache.ChangeStatus(digitalSalesId, newStatusId, note, attachmentPath, User.UserName);

            if (code == 1)
            {
                return Json(new
                {
                    status = true,
                    code = 1,
                    message = "Chuyển trạng thái thành công!"
                });
            }
            else if (code == -3)
            {
                return Json(new
                {
                    status = false,
                    code = -3,
                    message = "RÀNG BUỘC CHUYỂN DỰ ÁN: Chưa có Sản phẩm / Dịch vụ số đính kèm! Vui lòng vào Tab 'Sản phẩm & Doanh thu' để thêm sản phẩm dịch vụ trước khi chuyển sang Dự án."
                });
            }
            else if (code == -4)
            {
                return Json(new
                {
                    status = false,
                    code = -4,
                    message = "RÀNG BUỘC CHUYỂN DỰ ÁN: Chưa có Thành viên tham gia dự án! Vui lòng vào Tab 'Thành viên tham gia' để chỉ định nhân sự trước khi chuyển sang Dự án."
                });
            }
            else if (code == -1)
            {
                return Json(new
                {
                    status = false,
                    code = -1,
                    message = "Không tìm thấy hồ sơ kinh doanh số!"
                });
            }
            else if (code == -2)
            {
                return Json(new
                {
                    status = false,
                    code = -2,
                    message = "Trạng thái mới không tồn tại hoặc đã bị khóa!"
                });
            }

            return Json(new
            {
                status = false,
                code = 0,
                message = "Không thể cập nhật trạng thái. Vui lòng thử lại!"
            });
        }
        #endregion

        #region 5. Quản lý Sản phẩm / Dịch vụ số (Tab 2 - Products)
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult AddProductModal(int digitalSalesId)
        {
            var model = new RM_DigitalSalesProductModel
            {
                DigitalSalesID = digitalSalesId,
                Quantity = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };

            ViewBag.ProductList = _productServiceCache.GetAll()?.Select(p => new SelectListItem
            {
                Value = p.ProductServiceID.ToString(),
                Text = string.IsNullOrEmpty(p.ShortNameProduct) ? p.NameProduct : $"{p.ShortNameProduct} - {p.NameProduct}"
            }).ToList() ?? new List<SelectListItem>();

            return PartialView("_ProductModal", model);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditProductModal(int id, int digitalSalesId)
        {
            var products = _salesCache.GetProductsBySalesID(digitalSalesId);
            var model = products.FirstOrDefault(p => p.SalesProductID == id);
            if (model == null)
            {
                return Json(new
                {
                    status = false,
                    message = CreateMessage("Sản phẩm", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.ProductList = _productServiceCache.GetAll()?.Select(p => new SelectListItem
            {
                Value = p.ProductServiceID.ToString(),
                Text = string.IsNullOrEmpty(p.ShortNameProduct) ? p.NameProduct : $"{p.ShortNameProduct} - {p.NameProduct}"
            }).ToList() ?? new List<SelectListItem>();

            return PartialView("_ProductModal", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult SaveProduct(RM_DigitalSalesProductModel model)
        {
            if (model.DigitalSalesID <= 0 || model.ProductServiceID <= 0)
            {
                return Json(new
                {
                    status = false,
                    message = "Vui lòng chọn sản phẩm / dịch vụ số!"
                });
            }

            var id = _salesCache.SaveProduct(model, User.UserName);
            if (id > 0)
            {
                return Json(new
                {
                    status = true,
                    id = id,
                    message = "Lưu sản phẩm / dịch vụ thành công!"
                });
            }

            return Json(new
            {
                status = false,
                message = "Không thể lưu sản phẩm / dịch vụ!"
            });
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteProduct(int id)
        {
            var result = _salesCache.DeleteProduct(id, User.UserName);
            if (result > 0)
            {
                return Json(new
                {
                    status = true,
                    message = "Xóa sản phẩm thành công!"
                });
            }

            return Json(new
            {
                status = false,
                message = "Không thể xóa sản phẩm!"
            });
        }
        #endregion

        #region 6. Quản lý Thành viên tham gia (Tab 3 - Members)
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult AddMemberModal(int digitalSalesId)
        {
            var model = new RM_DigitalSalesMemberModel
            {
                DigitalSalesID = digitalSalesId,
                IsActive = true
            };

            ViewBag.UserList = _userCache.GetAll()?.Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text = $"{u.FullName} ({u.UserName})"
            }).ToList() ?? new List<SelectListItem>();

            return PartialView("_MemberModal", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult SaveMember(RM_DigitalSalesMemberModel model)
        {
            if (model.DigitalSalesID <= 0 || model.UserID <= 0)
            {
                return Json(new
                {
                    status = false,
                    message = "Vui lòng chọn nhân sự tham gia!"
                });
            }

            var id = _salesCache.SaveMember(model, User.UserName);
            if (id > 0)
            {
                return Json(new
                {
                    status = true,
                    id = id,
                    message = "Lưu thành viên thành công!"
                });
            }

            return Json(new
            {
                status = false,
                message = "Không thể lưu thành viên!"
            });
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteMember(int id)
        {
            var result = _salesCache.DeleteMember(id, User.UserName);
            if (result > 0)
            {
                return Json(new
                {
                    status = true,
                    message = "Xóa thành viên thành công!"
                });
            }

            return Json(new
            {
                status = false,
                message = "Không thể xóa thành viên!"
            });
        }
        #endregion

        #region 7. Quản lý Tiến trình & Checklist (Tab 4 - Tracking)
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult AddTrackingModal(int digitalSalesId)
        {
            var model = new RM_DigitalSalesTrackingModel
            {
                DigitalSalesID = digitalSalesId,
                StartDate = DateTime.Today,
                Deadline = DateTime.Today.AddDays(3),
                Status = 1,
                IsCustomTask = true
            };

            ViewBag.UserList = _userCache.GetAll()?.Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text = $"{u.FullName} ({u.UserName})"
            }).ToList() ?? new List<SelectListItem>();

            return PartialView("_TrackingModal", model);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditTrackingModal(int id, int digitalSalesId)
        {
            var tasks = _salesCache.GetTrackingTasks(digitalSalesId);
            var model = tasks.FirstOrDefault(t => t.TrackingID == id);
            if (model == null)
            {
                return Json(new
                {
                    status = false,
                    message = CreateMessage("Tiến trình", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.UserList = _userCache.GetAll()?.Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text = $"{u.FullName} ({u.UserName})"
            }).ToList() ?? new List<SelectListItem>();

            return PartialView("_TrackingModal", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult SaveTracking(RM_DigitalSalesTrackingModel model, HttpPostedFileBase attachmentFile)
        {
            if (model.DigitalSalesID <= 0 || string.IsNullOrWhiteSpace(model.TaskName))
            {
                return Json(new
                {
                    status = false,
                    message = "Vui lòng nhập tên công việc / tiến trình!"
                });
            }

            if (attachmentFile != null && attachmentFile.ContentLength > 0)
            {
                model.AttachmentFile = SaveUploadedFile(attachmentFile);
            }

            var id = _salesCache.SaveTracking(model, User.UserName);
            if (id > 0)
            {
                return Json(new
                {
                    status = true,
                    id = id,
                    message = "Lưu tiến trình thành công!"
                });
            }

            return Json(new
            {
                status = false,
                message = "Không thể lưu tiến trình!"
            });
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult UpdateTrackingStatus(int trackingId, byte status, string resultNote, HttpPostedFileBase attachmentFile, int? assignedUserId, DateTime? deadline)
        {
            if (trackingId <= 0)
            {
                return Json(new
                {
                    status = false,
                    message = "Mã tiến trình không hợp lệ!"
                });
            }

            string attachmentPath = null;
            if (attachmentFile != null && attachmentFile.ContentLength > 0)
            {
                attachmentPath = SaveUploadedFile(attachmentFile);
            }

            var result = _salesCache.UpdateTrackingStatus(trackingId, status, resultNote, attachmentPath, assignedUserId, deadline, User.UserName);
            if (result > 0)
            {
                return Json(new
                {
                    status = true,
                    message = "Cập nhật tiến trình thành công!"
                });
            }

            return Json(new
            {
                status = false,
                message = "Không thể cập nhật tiến trình!"
            });
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteTracking(int id)
        {
            var result = _salesCache.DeleteTracking(id, User.UserName);
            if (result > 0)
            {
                return Json(new
                {
                    status = true,
                    message = "Xóa tiến trình thành công!"
                });
            }

            return Json(new
            {
                status = false,
                message = "Không thể xóa tiến trình!"
            });
        }
        #endregion

        #region 8. Ajax Helpers
        [AjaxOnly]
        [HttpGet]
        public ActionResult GetContactPersons(int customerId)
        {
            var list = _contactPersonCache.GetByCustomerID(customerId)?.Select(c => new
            {
                id = c.ContactPerson_ID,
                name = $"{c.FullName} - {c.Position} ({c.Phone ?? c.Email ?? ""})"
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        public ActionResult GetStatusesByBusinessType(byte businessType)
        {
            var list = _salesCache.GetStatusList(businessType)?.Select(s => new
            {
                id = s.StatusID,
                name = s.StatusName
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 9. Helpers & Dropdown Population
        private void PrepareSearchDropdowns(RM_DigitalSalesSearchModel model)
        {
            model.ListCustomer = _customerCache.GetAll()?.Select(c => new SelectListItem
            {
                Value = c.CustomerID.ToString(),
                Text = c.CustomerName
            }).ToList() ?? new List<SelectListItem>();

            model.ListProductService = _productServiceCache.GetAll()?.Select(p => new SelectListItem
            {
                Value = p.ProductServiceID.ToString(),
                Text = string.IsNullOrEmpty(p.ShortNameProduct) ? p.NameProduct : $"{p.ShortNameProduct} - {p.NameProduct}"
            }).ToList() ?? new List<SelectListItem>();

            model.ListStatus = _salesCache.GetStatusList(null)?.Select(s => new SelectListItem
            {
                Value = s.StatusID.ToString(),
                Text = $"[{(s.BusinessType == 1 ? "Cơ hội" : "Dự án")}] {s.StatusName}"
            }).ToList() ?? new List<SelectListItem>();

            model.ListEmployee = _employeeCache.GetAll()?.Select(e => new SelectListItem
            {
                Value = e.Employee_ID.ToString(),
                Text = e.FullName
            }).ToList() ?? new List<SelectListItem>();

            model.ListDepartment = _departmentCache.GetAll()?.Select(d => new SelectListItem
            {
                Value = d.BoPhan_ID.ToString(),
                Text = d.TenBoPhan
            }).ToList() ?? new List<SelectListItem>();
        }

        private void PrepareSalesDropdowns(RM_DigitalSalesModel model)
        {
            model.ListCustomer = _customerCache.GetAll()?.Select(c => new SelectListItem
            {
                Value = c.CustomerID.ToString(),
                Text = c.CustomerName
            }).ToList() ?? new List<SelectListItem>();

            if (model.CustomerID > 0)
            {
                model.ListContactPerson = _contactPersonCache.GetByCustomerID(model.CustomerID)?.Select(c => new SelectListItem
                {
                    Value = c.ContactPerson_ID.ToString(),
                    Text = $"{c.FullName} - {c.Position}"
                }).ToList() ?? new List<SelectListItem>();
            }

            model.ListStatus = _salesCache.GetStatusList(model.BusinessType)?.Select(s => new SelectListItem
            {
                Value = s.StatusID.ToString(),
                Text = s.StatusName
            }).ToList() ?? new List<SelectListItem>();

            model.ListEmployee = _userCache.GetAll()?.Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text = $"{u.FullName} ({u.UserName})"
            }).ToList() ?? new List<SelectListItem>();

            model.ListDepartment = _departmentCache.GetAll()?.Select(d => new SelectListItem
            {
                Value = d.BoPhan_ID.ToString(),
                Text = d.TenBoPhan
            }).ToList() ?? new List<SelectListItem>();

            model.ListContract = _contractCache.GetAll()?.Select(ct => new SelectListItem
            {
                Value = ct.ContractID.ToString(),
                Text = $"{ct.ContractCode} - {ct.ContractName}"
            }).ToList() ?? new List<SelectListItem>();
        }

        private string SaveUploadedFile(HttpPostedFileBase file)
        {
            try
            {
                var originalName = Path.GetFileNameWithoutExtension(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                var safeName = UtilString.ConvertToUnSign(originalName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                var relativePath = _folderUpload + "/" + safeName;
                var physicalPath = HostingEnvironment.MapPath(relativePath);

                var dir = Path.GetDirectoryName(physicalPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                file.SaveAs(physicalPath);
                return relativePath;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
