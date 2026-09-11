using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using Core.Sys.Models.Sys;
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
        private readonly RM_RolesCache _rolesCache;
        private readonly SysUserCache _userCache;
        private readonly SysUserBoPhanCache _userBoPhanCache;

        private readonly string _title = "Danh sách kinh doanh sản phẩm dịch vụ số";
        private readonly string _folderUpload = ConfigurationManager.AppSettings["AppImageRoot_Path"] ?? "/Contents/File";
        private string GetAppMessage(string labelKey, string defaultMessage)
        {
            var msg = AppProcessor.Messagor.GetMessage(labelKey);
            return !string.IsNullOrEmpty(msg) ? msg : defaultMessage;
        }


        public DigitalSalesController()
        {
            _salesCache = new RM_DigitalSalesCache();
            _customerCache = new RM_CustomerCache();
            _productServiceCache = new Cate_ProductServiceCache();
            _employeeCache = new MN_EmployeeCache();
            _departmentCache = new MN_BoPhanCache();
            _contactPersonCache = new RM_ContactPersonsCache();
            _contractCache = new RM_ContractsCache();
            _rolesCache = new RM_RolesCache();
            _userCache = new SysUserCache();
            _userBoPhanCache = new SysUserBoPhanCache();
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
                Code = _salesCache.GenerateNextCode(),
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
                model.DepartmentID = GetDepartmentIdByUserId(currentUser.UserId);
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
                    message = GetAppMessage("DigitalSales_Msg_CustomerRequired", "Vui lòng chọn khách hàng!")
                });
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_TitleRequired", "Vui lòng nhập tên cơ hội / dự án!")
                });
            }

            var uploadedFiles = new List<string>();
            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        var path = SaveUploadedFile(file);
                        if (!string.IsNullOrEmpty(path))
                        {
                            uploadedFiles.Add(path);
                        }
                    }
                }
            }

            if (uploadedFiles.Count > 0)
            {
                model.FileAttach = string.Join(";", uploadedFiles);
            }

            if ((!model.DepartmentID.HasValue || model.DepartmentID.Value <= 0) && model.AssignedEmployeeID.HasValue)
            {
                model.DepartmentID = GetDepartmentIdByUserId(model.AssignedEmployeeID.Value);
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
            if (!HasDetailPermission(id, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền chỉnh sửa hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
                }, JsonRequestBehavior.AllowGet);
            }

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

            if (!HasDetailPermission(model.DigitalSalesID, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền chỉnh sửa hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
                });
            }

            var uploadedFiles = new List<string>();
            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        var path = SaveUploadedFile(file);
                        if (!string.IsNullOrEmpty(path))
                        {
                            uploadedFiles.Add(path);
                        }
                    }
                }
            }

            if (uploadedFiles.Count > 0)
            {
                var newPaths = string.Join(";", uploadedFiles);
                model.FileAttach = !string.IsNullOrEmpty(model.FileAttach)
                    ? model.FileAttach + ";" + newPaths
                    : newPaths;
            }

            if ((!model.DepartmentID.HasValue || model.DepartmentID.Value <= 0) && model.AssignedEmployeeID.HasValue)
            {
                model.DepartmentID = GetDepartmentIdByUserId(model.AssignedEmployeeID.Value);
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
            try
            {
                ViewBag.CanEdit = HasDetailPermission(model, User.UserName);
            }
            catch
            {
                ViewBag.CanEdit = false;
            }
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
            if (!HasDetailPermission(id, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền chuyển trạng thái hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
                }, JsonRequestBehavior.AllowGet);
            }

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
            if (!HasDetailPermission(digitalSalesId, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền chuyển trạng thái hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
                });
            }

            if (digitalSalesId <= 0 || newStatusId <= 0)
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_InvalidData", "Dữ liệu không hợp lệ!")
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
                    message = GetAppMessage("DigitalSales_Msg_ChangeStatusSuccess", "Chuyển trạng thái thành công!")
                });
            }
            else if (code == -3)
            {
                return Json(new
                {
                    status = false,
                    code = -3,
                    message = GetAppMessage("DigitalSales_Msg_ReqProductBeforeProject", "RÀNG BUỘC CHUYỂN DỰ ÁN: Chưa có Sản phẩm / Dịch vụ số đính kèm! Vui lòng vào Tab 'Sản phẩm & Doanh thu' để thêm sản phẩm dịch vụ trước khi chuyển sang Dự án.")
                });
            }
            else if (code == -4)
            {
                return Json(new
                {
                    status = false,
                    code = -4,
                    message = GetAppMessage("DigitalSales_Msg_ReqMemberBeforeProject", "RÀNG BUỘC CHUYỂN DỰ ÁN: Chưa có Thành viên tham gia dự án! Vui lòng vào Tab 'Thành viên tham gia' để chỉ định nhân sự trước khi chuyển sang Dự án.")
                });
            }
            else if (code == -1)
            {
                return Json(new
                {
                    status = false,
                    code = -1,
                    message = GetAppMessage("DigitalSales_Msg_NotFound", "Không tìm thấy hồ sơ kinh doanh số!")
                });
            }
            else if (code == -2)
            {
                return Json(new
                {
                    status = false,
                    code = -2,
                    message = GetAppMessage("DigitalSales_Msg_StatusInvalid", "Trạng thái mới không tồn tại hoặc đã bị khóa!")
                });
            }

            return Json(new
            {
                status = false,
                code = 0,
                message = GetAppMessage("DigitalSales_Msg_ChangeStatusFail", "Không thể cập nhật trạng thái. Vui lòng thử lại!")
            });
        }
        #endregion

        #region 5. Quản lý Sản phẩm / Dịch vụ số (Tab 2 - Products)
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult AddProductModal(int digitalSalesId)
        {
            if (!HasDetailPermission(digitalSalesId, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền thêm sản phẩm trên hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
                }, JsonRequestBehavior.AllowGet);
            }

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
            if (!HasDetailPermission(digitalSalesId, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền chỉnh sửa sản phẩm trên hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
                }, JsonRequestBehavior.AllowGet);
            }

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
                    message = GetAppMessage("DigitalSales_Msg_ProductRequired", "Vui lòng chọn sản phẩm / dịch vụ số!")
                });
            }

            if (!HasDetailPermission(model.DigitalSalesID, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền lưu sản phẩm trên hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
                });
            }

            var id = _salesCache.SaveProduct(model, User.UserName);
            if (id > 0)
            {
                return Json(new
                {
                    status = true,
                    id = id,
                    message = GetAppMessage("DigitalSales_Msg_SaveProductSuccess", "Lưu sản phẩm / dịch vụ thành công!")
                });
            }

            return Json(new
            {
                status = false,
                message = GetAppMessage("DigitalSales_Msg_SaveProductFail", "Không thể lưu sản phẩm / dịch vụ!")
            });
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteProduct(int id, int? salesId = null)
        {
            if (salesId.HasValue && salesId.Value > 0 && !HasDetailPermission(salesId.Value, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền xóa sản phẩm trên hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
                });
            }

            var result = _salesCache.DeleteProduct(id, User.UserName);
            if (result > 0)
            {
                return Json(new
                {
                    status = true,
                    message = GetAppMessage("DigitalSales_Msg_DeleteProductSuccess", "Xóa sản phẩm thành công!")
                });
            }

            return Json(new
            {
                status = false,
                message = GetAppMessage("DigitalSales_Msg_DeleteProductFail", "Không thể xóa sản phẩm!")
            });
        }
        #endregion

        #region 6. Quản lý Thành viên tham gia (Tab 3 - Members)
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult AddMemberModal(int digitalSalesId)
        {
            if (!HasDetailPermission(digitalSalesId, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền thêm thành viên trên hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
                }, JsonRequestBehavior.AllowGet);
            }

            var model = new RM_DigitalSalesMemberModel
            {
                DigitalSalesID = digitalSalesId,
                IsActive = true
            };

            var accessibleDepts = GetAccessibleDepartments() ?? new List<MN_BoPhanModel>();
            var accessibleDeptIds = accessibleDepts.Select(d => d.BoPhan_ID).ToHashSet();

            // Bao gồm cả các đơn vị con thuộc các đơn vị đang quản lý
            var allDepts = _departmentCache.GetAll() ?? new List<MN_BoPhanModel>();
            foreach (var d in allDepts)
            {
                if (d.BoPhanCha_ID.HasValue && accessibleDeptIds.Contains(d.BoPhanCha_ID.Value))
                {
                    accessibleDeptIds.Add(d.BoPhan_ID);
                }
            }

            // Lọc danh sách nhân sự CHỈ thuộc các đơn vị người dùng đang quản lý
            var allEmployees = _employeeCache.GetAll() ?? new List<MN_EmployeeModel>();
            var employees = allEmployees.Where(e => accessibleDeptIds.Contains(e.BoPhan_ID)).ToList();

            // Fallback: nếu danh sách nhân sự rỗng, load theo _userCache dựa trên các đơn vị quản lý
            if (employees.Count == 0)
            {
                var userList = new List<SysUserModel>();
                foreach (var dId in accessibleDeptIds)
                {
                    var uList = _userCache.GetByBoPhanAndChucVu(dId, null);
                    if (uList != null) userList.AddRange(uList);
                }
                employees = userList.GroupBy(u => u.UserId).Select(g =>
                {
                    var u = g.First();
                    return new MN_EmployeeModel
                    {
                        Employee_ID = u.UserId ?? 0,
                        FullName = u.FullName,
                        BoPhan_ID = accessibleDeptIds.FirstOrDefault(),
                        TenBoPhan = u.OfficeName
                    };
                }).Where(e => e.Employee_ID > 0).ToList();
            }

            var existingMembers = _salesCache.GetMembersBySalesID(digitalSalesId) ?? new List<RM_DigitalSalesMemberModel>();
            var existingUserIds = existingMembers.Select(m => m.UserID).ToHashSet();

            employees.ForEach(employee => employee.IsSaleMember = existingUserIds.Contains(employee.Employee_ID));

            ViewBag.Employees = employees;
            ViewBag.Departments = accessibleDepts;

            var roles = _rolesCache.GetAll() ?? new List<RM_RolesModel>();
            if (roles.Count == 0)
            {
                roles = new List<RM_RolesModel>
                {
                    new RM_RolesModel { RoleID = 1, RoleName = "AM Kinh doanh" },
                    new RM_RolesModel { RoleID = 2, RoleName = "Kỹ thuật giải pháp" },
                    new RM_RolesModel { RoleID = 3, RoleName = "Chuyên gia triển khai" },
                    new RM_RolesModel { RoleID = 4, RoleName = "Hỗ trợ PoC" },
                    new RM_RolesModel { RoleID = 5, RoleName = "Quản trị dự án" },
                    new RM_RolesModel { RoleID = 6, RoleName = "Chăm sóc khách hàng" }
                };
            }
            ViewBag.Roles = roles;

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
        public ActionResult SaveMember(RM_DigitalSalesMemberModel model, string EmployeeIDs, string RoleIDs, string CustomRole)
        {
            if (model.DigitalSalesID <= 0)
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_InvalidSalesRecord", "Hồ sơ không hợp lệ!")
                });
            }

            if (!HasDetailPermission(model.DigitalSalesID, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền quản lý thành viên trên hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
                });
            }

            var empIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(EmployeeIDs))
            {
                foreach (var part in EmployeeIDs.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (int.TryParse(part.Trim(), out int eid) && eid > 0 && !empIdList.Contains(eid))
                    {
                        empIdList.Add(eid);
                    }
                }
            }
            else if (model.UserID > 0)
            {
                empIdList.Add(model.UserID);
            }

            if (empIdList.Count == 0)
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_MemberRequired", "Vui lòng chọn ít nhất một nhân sự tham gia!")
                });
            }

            var roleNamesList = new List<string>();
            if (!string.IsNullOrWhiteSpace(RoleIDs))
            {
                var allRoles = _rolesCache.GetAll() ?? new List<RM_RolesModel>();
                var roleIdSet = RoleIDs.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToHashSet();

                foreach (var r in allRoles)
                {
                    if (roleIdSet.Contains(r.RoleID.ToString()))
                    {
                        roleNamesList.Add(r.RoleName);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(CustomRole))
            {
                roleNamesList.Add(CustomRole.Trim());
            }
            else if (!string.IsNullOrWhiteSpace(model.RoleTitle))
            {
                roleNamesList.Add(model.RoleTitle.Trim());
            }

            var finalRoleTitle = roleNamesList.Count > 0 ? string.Join(", ", roleNamesList.Distinct()) : "Thành viên";

            int savedCount = 0;
            foreach (var empId in empIdList)
            {
                var m = new RM_DigitalSalesMemberModel
                {
                    MemberID = 0,
                    DigitalSalesID = model.DigitalSalesID,
                    UserID = empId,
                    RoleTitle = finalRoleTitle,
                    IsAM = model.IsAM, // Tất cả nhân sự được chọn đều nhận quyền cập nhật trạng thái nếu được tích
                    Note = model.Note,
                    IsActive = true
                };
                var id = _salesCache.SaveMember(m, User.UserName);
                if (id > 0) savedCount++;
            }

            if (savedCount > 0)
            {
                return Json(new
                {
                    status = true,
                    message = savedCount == 1 ? "Lưu thành viên thành công!" : $"Đã lưu thành công {savedCount} nhân sự tham gia!"
                });
            }

            return Json(new
            {
                status = false,
                message = GetAppMessage("DigitalSales_Msg_SaveMemberFail", "Không thể lưu thành viên!")
            });
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteMember(int id, int? salesId = null)
        {
            if (salesId.HasValue && salesId.Value > 0 && !HasDetailPermission(salesId.Value, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền xóa thành viên trên hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
                });
            }

            var result = _salesCache.DeleteMember(id, User.UserName);
            if (result > 0)
            {
                return Json(new
                {
                    status = true,
                    message = GetAppMessage("DigitalSales_Msg_DeleteMemberSuccess", "Xóa thành viên thành công!")
                });
            }

            return Json(new
            {
                status = false,
                message = GetAppMessage("DigitalSales_Msg_DeleteMemberFail", "Không thể xóa thành viên!")
            });
        }
        #endregion

        #region 7. Quản lý Tiến trình & Checklist (Tab 4 - Tracking)
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult AddTrackingModal(int digitalSalesId)
        {
            if (!HasDetailPermission(digitalSalesId, User.UserName))
            {
                return Content("<div class='alert alert-warning m-3'><i class='fa fa-lock'></i> Bạn không có quyền thêm tiến trình trên hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.</div>");
            }

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
            if (!HasDetailPermission(digitalSalesId, User.UserName))
            {
                return Content("<div class='alert alert-warning m-3'><i class='fa fa-lock'></i> Bạn không có quyền chỉnh sửa tiến trình trên hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.</div>");
            }

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
                    message = GetAppMessage("DigitalSales_Msg_TaskNameRequired", "Vui lòng nhập tên công việc / tiến trình!")
                });
            }

            if (!HasDetailPermission(model.DigitalSalesID, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền thực hiện trên hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới có quyền thao tác.")
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
                    message = GetAppMessage("DigitalSales_Msg_SaveTaskSuccess", "Lưu tiến trình thành công!")
                });
            }

            return Json(new
            {
                status = false,
                message = GetAppMessage("DigitalSales_Msg_SaveTaskFail", "Không thể lưu tiến trình!")
            });
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult UpdateTrackingStatus(int trackingId, byte status, string resultNote, HttpPostedFileBase attachmentFile, int? assignedUserId, DateTime? deadline, int? salesId = null)
        {
            if (trackingId <= 0)
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_InvalidTaskCode", "Mã tiến trình không hợp lệ!")
                });
            }

            if (salesId.HasValue && salesId.Value > 0 && !HasDetailPermission(salesId.Value, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền cập nhật tiến trình trên hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
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
                    message = GetAppMessage("DigitalSales_Msg_UpdateTaskSuccess", "Cập nhật tiến trình thành công!")
                });
            }

            return Json(new
            {
                status = false,
                message = GetAppMessage("DigitalSales_Msg_UpdateTaskFail", "Không thể cập nhật tiến trình!")
            });
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteTracking(int id, int? salesId = null)
        {
            if (salesId.HasValue && salesId.Value > 0 && !HasDetailPermission(salesId.Value, User.UserName))
            {
                return Json(new
                {
                    status = false,
                    message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền xóa tiến trình trên hồ sơ này! Chỉ tài khoản QTHT hoặc nhân sự được cấp quyền cập nhật trạng thái mới được thực hiện.")
                });
            }

            var result = _salesCache.DeleteTracking(id, User.UserName);
            if (result > 0)
            {
                return Json(new
                {
                    status = true,
                    message = GetAppMessage("DigitalSales_Msg_DeleteTaskSuccess", "Xóa tiến trình thành công!")
                });
            }

            return Json(new
            {
                status = false,
                message = GetAppMessage("DigitalSales_Msg_DeleteTaskFail", "Không thể xóa tiến trình!")
            });
        }
        #endregion

        #region 8. Ajax Helpers
        [AjaxOnly]
        [HttpGet]
        public ActionResult SearchCustomers(string q, int page = 1, int pageSize = 20)
        {
            try
            {
                var searchModel = new RM_CustomerSearchModel
                {
                    Keyword = q
                };
                var baseSearch = new BaseSearchModel
                {
                    StartIndex = (page - 1) * pageSize,
                    PageSize = pageSize,
                    Order = "0",
                    OrderDir = "ASC"
                };

                int total = 0;
                var list = _customerCache.Get(out total, searchModel, baseSearch);

                var items = list?.Select(c => (object)new
                {
                    id = c.CustomerID,
                    text = c.CustomerName,
                    shortName = c.ShortName,
                    taxCode = c.TaxCode,
                    phone = c.Phone,
                    address = c.AddressCus
                }).ToList() ?? new List<object>();

                int totalPages = (int)Math.Ceiling((double)total / (pageSize > 0 ? pageSize : 10));
                bool more = (page * pageSize) < total;

                return Json(new
                {
                    total = total,
                    page = page,
                    pageSize = pageSize,
                    totalPages = totalPages,
                    data = items,
                    results = items,
                    pagination = new { more = more }
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { total = 0, page = 1, totalPages = 0, data = new List<object>(), results = new List<object>(), pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
            }
        }

        [AjaxOnly]
        [HttpGet]
        public ActionResult GetCustomerDetail(int id)
        {
            if (id <= 0) return Json(null, JsonRequestBehavior.AllowGet);
            try
            {
                var c = _customerCache.GetById(id);
                if (c == null) return Json(null, JsonRequestBehavior.AllowGet);
                return Json(new
                {
                    id = c.CustomerID,
                    customerName = c.CustomerName,
                    shortName = c.ShortName,
                    taxCode = c.TaxCode,
                    phone = c.Phone,
                    email = c.Email,
                    address = c.AddressCus
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

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
        public ActionResult GetDepartmentByEmployee(int employeeId)
        {
            var deptId = GetDepartmentIdByUserId(employeeId);
            return Json(new { departmentId = deptId ?? 0 }, JsonRequestBehavior.AllowGet);
        }

        private int? GetDepartmentIdByUserId(int? userId)
        {
            if (!userId.HasValue || userId.Value <= 0) return null;
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings["TOC.Conn.Major"]?.ConnectionString;
                if (!string.IsNullOrEmpty(connStr))
                {
                    using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT TOP 1 bp.BoPhan_ID FROM dbo.Sys_Users u INNER JOIN dbo.MN_BoPhan bp ON u.MaBoPhan = bp.MaBoPhan WHERE u.UserId = @UserId";
                            cmd.Parameters.AddWithValue("@UserId", userId.Value);
                            var obj = cmd.ExecuteScalar();
                            if (obj != null && obj != DBNull.Value)
                            {
                                return Convert.ToInt32(obj);
                            }
                        }
                    }
                }

                var user = _userCache.GetById(userId.Value);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var userBoPhans = _userBoPhanCache.GetByEmail(user.Email);
                    if (userBoPhans != null && userBoPhans.Count > 0)
                    {
                        return userBoPhans.FirstOrDefault()?.BoPhan_ID;
                    }
                }
            }
            catch { }
            return null;
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
        private List<SysUserModel> GetAccessibleEmployees()
        {
            var list = new List<SysUserModel>();
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings["TOC.Conn.Major"]?.ConnectionString 
                    ?? ConfigurationManager.ConnectionStrings["CenITConnection"]?.ConnectionString;
                var currentUser = _userCache.GetByUserName(User.UserName);
                if (currentUser != null && !string.IsNullOrWhiteSpace(currentUser.Email) && !string.IsNullOrEmpty(connStr))
                {
                    using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"
                                SELECT DISTINCT u.UserId, u.UserName, u.FullName
                                FROM Sys_Users u
                                INNER JOIN MN_BoPhan bp ON u.MaBoPhan = bp.MaBoPhan
                                INNER JOIN Sys_UserBoPhan ub ON bp.MaBoPhan = ub.MaBoPhan
                                WHERE ub.Email = @Email AND u.IsActive = 1 AND u.IsDeleted = 0
                                ORDER BY u.FullName";
                            cmd.Parameters.AddWithValue("@Email", currentUser.Email);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    list.Add(new SysUserModel
                                    {
                                        UserId = Convert.ToInt32(reader["UserId"]),
                                        UserName = reader["UserName"]?.ToString(),
                                        FullName = reader["FullName"]?.ToString()
                                    });
                                }
                            }
                        }
                    }
                }

                if (list.Count == 0 && !string.IsNullOrEmpty(connStr))
                {
                    var accessibleDepts = GetAccessibleDepartments();
                    if (accessibleDepts != null && accessibleDepts.Count > 0)
                    {
                        var deptIds = string.Join(",", accessibleDepts.Select(d => d.BoPhan_ID));
                        using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                        {
                            conn.Open();
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = $@"
                                    SELECT DISTINCT u.UserId, u.UserName, u.FullName
                                    FROM Sys_Users u
                                    INNER JOIN MN_BoPhan bp ON u.MaBoPhan = bp.MaBoPhan
                                    WHERE bp.BoPhan_ID IN ({deptIds}) AND u.IsActive = 1 AND u.IsDeleted = 0
                                    ORDER BY u.FullName";
                                using (var reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        list.Add(new SysUserModel
                                        {
                                            UserId = Convert.ToInt32(reader["UserId"]),
                                            UserName = reader["UserName"]?.ToString(),
                                            FullName = reader["FullName"]?.ToString()
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Fallback safe
            }
            return list;
        }

        [HttpGet]
        public JsonResult GetEmployeesByDepartment(int departmentId)
        {
            List<SysUserModel> users;
            if (departmentId > 0)
            {
                users = _userCache.GetByBoPhanAndChucVu(departmentId, null) ?? new List<SysUserModel>();
            }
            else
            {
                var accessibleDepts = GetAccessibleDepartments();
                var allUsers = new List<SysUserModel>();
                foreach (var dept in accessibleDepts)
                {
                    var uList = _userCache.GetByBoPhanAndChucVu(dept.BoPhan_ID, null);
                    if (uList != null) allUsers.AddRange(uList);
                }
                users = allUsers.GroupBy(u => u.UserId).Select(g => g.First()).OrderBy(u => u.FullName).ToList();
            }

            var result = users.Select(x => new
            {
                Value = x.UserId,
                Text = $"{x.FullName} ({x.UserName})"
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<MN_BoPhanModel> GetAccessibleDepartments()
        {
            var currentUser = _userCache.GetByUserName(User.UserName);
            List<MN_BoPhanModel> list = null;
            if (currentUser != null && !string.IsNullOrWhiteSpace(currentUser.Email))
            {
                list = (_userBoPhanCache.GetByEmail(currentUser.Email) ?? new List<MN_BoPhanModel>())
                    .GroupBy(x => x.BoPhan_ID)
                    .Select(x => x.First())
                    .OrderBy(x => x.TenBoPhanView)
                    .ToList();
            }

            if (list == null || list.Count == 0)
            {
                list = (_departmentCache.GetAll() ?? new List<MN_BoPhanModel>())
                    .Where(x => (x.MaBoPhan != null && x.MaBoPhan.StartsWith("239.603")) || x.BoPhan_ID == 5749 || x.BoPhanCha_ID == 5749)
                    .OrderBy(x => x.TenBoPhan)
                    .ToList();
            }

            return list;
        }

        #region Authorization Helper (QTHT & Quyền cập nhật trạng thái)
        private bool IsUserQTHT(string userName, int? userId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName)) return false;
                if (userName.Equals("admin", StringComparison.OrdinalIgnoreCase) || userName.Equals("quantri", StringComparison.OrdinalIgnoreCase)) return true;

                if (!userId.HasValue || userId.Value <= 0)
                {
                    var u = _userCache.GetByUserName(userName);
                    userId = u?.UserId;
                }

                if (userId.HasValue && userId.Value > 0)
                {
                    var roles = _userCache.GetRoles(userId.Value);
                    if (roles != null && roles.Any(r => r.RoleId == 1 || (r.Name != null && (r.Name.Equals("QTHT", StringComparison.OrdinalIgnoreCase) || r.Name.IndexOf("quản trị", StringComparison.OrdinalIgnoreCase) >= 0))))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // Fallback safe
            }

            return false;
        }

        private bool HasDetailPermission(RM_DigitalSalesModel sales, string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName)) return false;

                // 1. Đối với những tài khoản được phân quyền QTHT thì sẽ có quyền thao tác toàn bộ các chức năng
                if (IsUserQTHT(userName)) return true;

                // 2. Đối với những người được check quyền cập nhật trạng thái
                if (sales != null)
                {
                    var currentUser = _userCache.GetByUserName(userName);
                    var currentUserId = currentUser?.UserId;

                    if (sales.Members != null && sales.Members.Count > 0)
                    {
                        var hasStatusPermission = sales.Members.Any(m =>
                            m.IsAM && (
                                (!string.IsNullOrEmpty(m.UserName) && m.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)) ||
                                (currentUserId.HasValue && currentUserId.Value > 0 && m.UserID == currentUserId.Value)
                            )
                        );
                        if (hasStatusPermission) return true;
                    }

                    if (currentUserId.HasValue && currentUserId.Value > 0 && sales.AssignedEmployeeID == currentUserId.Value)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // Fallback safe
            }

            return false;
        }

        private bool HasDetailPermission(int digitalSalesId, string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName)) return false;
                if (IsUserQTHT(userName)) return true;

                if (digitalSalesId > 0)
                {
                    var sales = _salesCache.GetByID(digitalSalesId);
                    return HasDetailPermission(sales, userName);
                }
            }
            catch
            {
                // Fallback safe
            }

            return false;
        }
        #endregion

        private void PrepareSearchDropdowns(RM_DigitalSalesSearchModel model)
        {
            var accessibleDepts = GetAccessibleDepartments();
            model.Departments = accessibleDepts.Select(d => new SelectListItem
            {
                Value = d.BoPhan_ID.ToString(),
                Text = !string.IsNullOrEmpty(d.TenBoPhanView) ? d.TenBoPhanView : d.TenBoPhan
            }).ToList();

            List<SysUserModel> users;
            if (model.DepartmentID > 0)
            {
                users = _userCache.GetByBoPhanAndChucVu(model.DepartmentID, null) ?? new List<SysUserModel>();
            }
            else
            {
                var allUsers = new List<SysUserModel>();
                foreach (var dept in accessibleDepts)
                {
                    var uList = _userCache.GetByBoPhanAndChucVu(dept.BoPhan_ID, null);
                    if (uList != null) allUsers.AddRange(uList);
                }
                users = allUsers.GroupBy(u => u.UserId).Select(g => g.First()).OrderBy(u => u.FullName).ToList();
            }

            model.ListEmployee = users.Select(e => new SelectListItem
            {
                Value = e.UserId.ToString(),
                Text = $"{e.FullName} ({e.UserName})"
            }).ToList();

            model.ListStatus = _salesCache.GetStatusList(null)?.Select(s => new SelectListItem
            {
                Value = s.StatusID.ToString(),
                Text = $"[{(s.BusinessType == 1 ? "Cơ hội" : "Dự án")}] {s.StatusName}"
            }).ToList() ?? new List<SelectListItem>();
        }

        private void PrepareSalesDropdowns(RM_DigitalSalesModel model)
        {
            if (model.CustomerID > 0)
            {
                var cus = _customerCache.GetById(model.CustomerID);
                if (cus != null)
                {
                    model.ListCustomer = new List<SelectListItem>
                    {
                        new SelectListItem { Value = cus.CustomerID.ToString(), Text = cus.CustomerName, Selected = true }
                    };
                    model.CustomerName = cus.CustomerName;
                }
                else
                {
                    model.ListCustomer = new List<SelectListItem>();
                }
            }
            else
            {
                model.ListCustomer = new List<SelectListItem>();
            }

            if (model.CustomerID > 0)
            {
                model.ListContactPerson = _contactPersonCache.GetByCustomerID(model.CustomerID)?.Select(c => new SelectListItem
                {
                    Value = c.ContactPerson_ID.ToString(),
                    Text = $"{c.FullName} - {c.Position}"
                }).ToList() ?? new List<SelectListItem>();
            }
            else
            {
                model.ListContactPerson = new List<SelectListItem>();
            }

            model.ListStatus = _salesCache.GetStatusList(model.BusinessType)?.Select(s => new SelectListItem
            {
                Value = s.StatusID.ToString(),
                Text = s.StatusName
            }).ToList() ?? new List<SelectListItem>();

            var accessibleUsers = GetAccessibleEmployees();

            var currentLoginUser = _userCache.GetByUserName(User.UserName);
            if (currentLoginUser != null && !accessibleUsers.Any(u => u.UserId == currentLoginUser.UserId))
            {
                accessibleUsers.Add(currentLoginUser);
            }

            if (model.AssignedEmployeeID.HasValue && model.AssignedEmployeeID.Value > 0 && !accessibleUsers.Any(u => u.UserId == model.AssignedEmployeeID.Value))
            {
                var assignedUser = _userCache.GetById(model.AssignedEmployeeID.Value);
                if (assignedUser != null)
                {
                    accessibleUsers.Add(assignedUser);
                }
            }

            model.ListEmployee = accessibleUsers
                .GroupBy(u => u.UserId)
                .Select(g => g.First())
                .OrderBy(u => u.FullName)
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = $"{u.FullName} ({u.UserName})"
                }).ToList();

            if ((!model.DepartmentID.HasValue || model.DepartmentID.Value <= 0) && model.AssignedEmployeeID.HasValue)
            {
                model.DepartmentID = GetDepartmentIdByUserId(model.AssignedEmployeeID.Value);
            }

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
