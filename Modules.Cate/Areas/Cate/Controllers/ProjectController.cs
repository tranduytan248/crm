using ClosedXML.Excel;
using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    /// <summary>
    /// Xử lý các luồng thêm, sửa, xóa, tra cứu và xuất danh sách dự án.
    /// </summary>
    public class ProjectController : AppController
    {
        private readonly RM_ProjectCache _projectCache;
        private readonly RM_CustomerCache _customerCache;
        private readonly RM_StatusCache _statusCache;
        private readonly RM_CustomerTypeCache _customerTypeCache;
        private readonly RM_ContractsCache _contractCache;
        private readonly RM_ProductProjectCache _productProjectCache;
        private readonly RM_ProjectMemberCache _projectMemberCache;
        private readonly RM_ProjectTaskCache _projectTaskCache;
        private readonly RM_ProductCostCache _productCostCache;
        private readonly RM_RolesCache _rolesCache;
        private readonly SysUserCache _userCache;
        private readonly SysUserBoPhanCache _userBoPhanCache;
        private readonly SysUserCache _employeeCache;
        private readonly SysConfigCache _configsCache = new SysConfigCache();
        private readonly string _ProjectTitle = AppProcessor.Messagor.GetMessage("Project_Title");

        /// <summary>
        /// Khởi tạo cache phục vụ xử lý dự án.
        /// </summary>
        public ProjectController()
        {
            _projectCache = new RM_ProjectCache();
            _customerCache = new RM_CustomerCache();
            _statusCache = new RM_StatusCache();
            _contractCache = new RM_ContractsCache();
            _customerTypeCache = new RM_CustomerTypeCache();
            _productProjectCache = new RM_ProductProjectCache();
            _projectMemberCache = new RM_ProjectMemberCache();
            _projectTaskCache = new RM_ProjectTaskCache();
            _productCostCache = new RM_ProductCostCache();
            _rolesCache = new RM_RolesCache();
            _userCache = new SysUserCache();
            _userBoPhanCache = new SysUserBoPhanCache();
            _employeeCache = new SysUserCache();
            _configsCache = new SysConfigCache();
        }

        /// <summary>
        /// Hiển thị màn hình danh sách dự án và khởi tạo bộ lọc tìm kiếm.
        /// </summary>
        /// <returns>Màn hình danh sách dự án.</returns>
        public ActionResult Index()
        {
            var config = _configsCache.GetViaKey("EXCLUSION_STATUS_IDS_KEY");
            var today = DateTime.Today;
            var model = new RM_ProjectSearchModel();
            model.Customers = _customerCache.GetAll();
            model.Status = _statusCache.GetStatusBySearchKey("Project");
            model.CustomerTypes = _customerTypeCache.GetAll();
            model.Year = today.Year;
            model.Departments = GetAccessibleDepartments();
            model.ExcludedStatusIDs = (config?.ConfigValue ?? "")
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    int.TryParse(x, out int id);
                    return id;
                })
                .Where(x => x > 0)
                .ToList();
            return View(model);
        }

        /// <summary>
        /// Trả danh sách dự án theo điều kiện tìm kiếm và phân trang.
        /// </summary>
        /// <param name="model">Điều kiện tìm kiếm dự án.</param>
        /// <returns>Dữ liệu JSON cho DataTable.</returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(RM_ProjectSearchModel model)
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
            model.UserName = User.UserName;
            var data = _projectCache.Get(out var total, model, dataSearch);
            if (data != null)
            {
                foreach (var item in data)
                {
                    item.CanDelete = item.UserCreated == User.UserName;
                }
            }
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
            var model = new RM_ProjectModel
            {
                ListCustomer = _customerCache.GetAll()
                    .Select(d => new SelectListItem { Text = d.CustomerName, Value = d.CustomerID.ToString() }).ToList(),
                ListContract = _contractCache.GetAll()
                    .Select(d => new SelectListItem { Text = d.ContractName, Value = d.ContractID.ToString() }).ToList(),
                ListStatus = _statusCache.GetStatusBySearchKey("Project") ?? new List<RM_StatusModel>()
            };
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
        public ActionResult Add(RM_ProjectModel model)
        {
            if (!string.IsNullOrEmpty(model.SuccessRate.ToString()))
            {
                if (model.SuccessRate < 0 || model.SuccessRate > 100)
                {
                    ModelState.AddModelError("SuccessRate", $"{AppProcessor.Messagor.GetMessage("SuccessRate_Label")} phải từ 0 đến 100.");
                }
            }
            if (!ModelState.IsValid)
            {
                model.ListCustomer = _customerCache.GetAll()
                    .Select(d => new SelectListItem { Text = d.CustomerName, Value = d.CustomerID.ToString() }).ToList();
                model.ListContract = _contractCache.GetAll()
                    .Select(d => new SelectListItem { Text = d.ContractName, Value = d.ContractID.ToString() }).ToList();
                model.ListStatus = _statusCache.GetStatusBySearchKey("Project") ?? new List<RM_StatusModel>();
                return PartialView("_Project", model);
            }
            model.Status = 16;
            var result = _projectCache.Save(model, User.UserName);
            string response;
            if (result == 0) response = CreateMessage($"{_ProjectTitle} [{model.ProjectName}]", EnumProcessType.Add, EnumMsgIcon.Error);
            else if (result == -9) response = CreateMessage($"{_ProjectTitle} [{model.ProjectName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else response = CreateMessage($"{_ProjectTitle} [{model.ProjectName}]", EnumProcessType.Add, EnumMsgIcon.Success);
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
            var model = _projectCache.GetById(id);
            if (model == null)
            {
                return Json(new { status = true, message = CreateMessage(_ProjectTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            }

            model.ListCustomer = _customerCache.GetAll()
                .Select(d => new SelectListItem { Text = d.CustomerName, Value = d.CustomerID.ToString() }).ToList();
            model.ListContract = _contractCache.GetAll()
                .Select(d => new SelectListItem { Text = d.ContractName, Value = d.ContractID.ToString() }).ToList();
            model.ListStatus = _statusCache.GetStatusBySearchKey("Project") ?? new List<RM_StatusModel>();

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
        public ActionResult Edit(RM_ProjectModel model)
        {
            if (!string.IsNullOrEmpty(model.SuccessRate.ToString()))
            {
                if (model.SuccessRate < 0 || model.SuccessRate > 100)
                {
                    ModelState.AddModelError("SuccessRate", $"{AppProcessor.Messagor.GetMessage("SuccessRate_Label")} phải từ 0 đến 100.");
                }
            }
            if (!ModelState.IsValid)
            {
                model.ListCustomer = _customerCache.GetAll()
                    .Select(d => new SelectListItem { Text = d.CustomerName, Value = d.CustomerID.ToString() }).ToList();
                model.ListContract = _contractCache.GetAll()
                    .Select(d => new SelectListItem { Text = d.ContractName, Value = d.ContractID.ToString() }).ToList();
                model.ListStatus = _statusCache.GetStatusBySearchKey("Project") ?? new List<RM_StatusModel>();
                return PartialView("_Project", model);
            }
            var result = _projectCache.Save(model, User.UserName);
            string response;
            if (result == 0) response = CreateMessage($"{_ProjectTitle} [{model.ProjectName}]", EnumProcessType.Edit, EnumMsgIcon.Error);
            else if (result == -9) response = CreateMessage($"{_ProjectTitle} [{model.ProjectName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else response = CreateMessage($"{_ProjectTitle} [{model.ProjectName}]", EnumProcessType.Edit, EnumMsgIcon.Success);
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
            var model = _projectCache.GetById(id);
            if (model == null)
                return Json(new { status = true, message = CreateMessage(_ProjectTitle, EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_ProjectTitle} [{model.ProjectName}]");
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
        public ActionResult Delete(RM_ProjectModel model)
        {
            var deleted = _projectCache.Delete(model, User.UserName);
            var response = CreateMessage($"{_ProjectTitle} [{model.ProjectName}]",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        /// <summary>
        /// Xuất danh sách dự án ra file Excel theo điều kiện lọc.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm.</param>
        /// <param name="customerTypeID">Mã loại khách hàng cần lọc.</param>
        /// <param name="statusID">Mã trạng thái dự án cần lọc.</param>
        /// <param name="cookieName">Tên cookie dùng để báo hoàn tất tải file.</param>
        /// <returns>File Excel danh sách dự án.</returns>
        [HttpGet]
        public ActionResult Export(string keyword, int? customerTypeID, int? statusID,
                                   string cookieName = null)
        {
            var searchModel = new RM_ProjectSearchModel
            {
                Keyword = keyword,
                CustomerTypeID = customerTypeID ?? 0,
                StatusID = statusID ?? 0,
                UserName = User.UserName
            };

            var projects = _projectCache.ExportProjects(searchModel);
            var products = _projectCache.ExportProducts(searchModel);
            var members = _projectCache.ExportMembers(searchModel);
            var tasks = _projectCache.ExportTasks(searchModel);

            var ms = new MemoryStream();
            using (var package = new ExcelPackage())
            {
                BuildSheetProjects(package, projects);
                BuildSheetProducts(package, products);
                BuildSheetMembers(package, members);
                BuildSheetTasks(package, tasks);
                package.SaveAs(ms);
            }
            if (!string.IsNullOrEmpty(cookieName))
            {
                Response.Cookies.Add(new System.Web.HttpCookie(cookieName, "done")
                {
                    Path = "/",
                    Expires = DateTime.Now.AddMinutes(1)
                });
            }

            ms.Seek(0, SeekOrigin.Begin);
            var fileName = $"DanhSachDuAn_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";
            return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = fileName
            };
        }

        /// <summary>
        /// Quy đổi mã màu HTML sang đối tượng màu để dùng cho định dạng file Excel.
        /// </summary>
        /// <param name="htmlColor">Mã màu HTML cần chuyển đổi.</param>
        /// <returns>Đối tượng màu tương ứng.</returns>
        private static Color GetColor(string htmlColor)
        {
            return ColorTranslator.FromHtml(htmlColor);
        }

        /// <summary>
        /// Tạo worksheet danh sách dự án trong file Excel xuất dữ liệu.
        /// </summary>
        /// <param name="package">Gói Excel đang xử lý.</param>
        /// <param name="data">Danh sách dữ liệu dự án cần ghi.</param>
        private static void BuildSheetProjects(ExcelPackage package, List<ProjectExportRow> data)
        {
            var ws = package.Workbook.Worksheets.Add("Danh sach du an");
            var headers = new[]
            {
                "STT", "Mã dự án", "Tên dự án","Tỉ lệ thành công", "Khách hàng", "MST",
                "Loại KH", "Ngày bắt đầu", "Trạng thái", "Hợp đồng",
                "Số SP/DV", "Số thành viên",
                "Tổng doanh thu", "Tổng chi phí", "Lợi nhuận", "Ghi chú"
            };
            var colWidths = new[] { 5, 10, 35,10, 35, 16, 18, 16, 18, 30, 10, 14, 22, 22, 22, 35 };
            ApplyHeader(ws, headers, colWidths);

            if (data == null || !data.Any()) return;
            for (int i = 0; i < data.Count; i++)
            {
                var r = data[i];
                int row = i + 2;
                ws.Cells[row, 1].Value = i + 1;
                ws.Cells[row, 2].Value = r.ProjectID;
                ws.Cells[row, 3].Value = r.ProjectName;
                ws.Cells[row, 3].Value = r.ProjectName;
                ws.Cells[row, 4].Value = r.SuccessRate;
                ws.Cells[row, 5].Value = r.CustomerName;
                ws.Cells[row, 6].Value = r.TaxCode;
                ws.Cells[row, 7].Value = r.CustomerTypeName;
                SetDateCell(ws.Cells[row, 8], r.StartDate);
                ws.Cells[row, 9].Value = r.StatusName;
                ws.Cells[row, 10].Value = r.ContractName;
                ws.Cells[row, 11].Value = r.TotalSanPham;
                ws.Cells[row, 12].Value = r.TotalThanhVien;
                ws.Cells[row, 13].Value = r.TotalDoanhThu;
                ws.Cells[row, 14].Value = r.TotalChiPhi;
                ws.Cells[row, 15].Value = r.LoiNhuan;
                ws.Cells[row, 16].Value = r.Note;
                ws.Cells[row, 13, row, 15].Style.Numberformat.Format = "#,##0";
                ApplyDataRowStyle(ws, row, headers.Length);
            }
            ws.View.FreezePanes(2, 1);
        }

        /// <summary>
        /// Tạo worksheet danh sách sản phẩm dịch vụ của dự án trong file Excel xuất dữ liệu.
        /// </summary>
        /// <param name="package">Gói Excel đang xử lý.</param>
        /// <param name="data">Danh sách sản phẩm dịch vụ cần ghi.</param>
        private static void BuildSheetProducts(ExcelPackage package, List<ProductExportRow> data)
        {
            var ws = package.Workbook.Worksheets.Add("San pham dich vu");
            var headers = new[]
            {
                "STT", "Mã dự án", "Tên dự án", "Khách hàng",
                "Tên sản phẩm/DV", "DT dự kiến (triệu VNĐ)",
                "Ngày bắt đầu", "Ngày kết thúc",
                "Tổng DT (triệu VNĐ)", "Tổng CP (triệu VNĐ)", "Lợi nhuận (triệu VNĐ)", "Số thành viên"
            };
            var colWidths = new[] { 5, 10, 35, 35, 40, 20, 16, 16, 22, 22, 22, 14 };
            ApplyHeader(ws, headers, colWidths);

            if (data == null || !data.Any()) return;
            for (int i = 0; i < data.Count; i++)
            {
                var r = data[i];
                int row = i + 2;
                ws.Cells[row, 1].Value = i + 1;
                ws.Cells[row, 2].Value = r.ProjectID;
                ws.Cells[row, 3].Value = r.ProjectName;
                ws.Cells[row, 4].Value = r.CustomerName;
                ws.Cells[row, 5].Value = r.NameProduct;
                ws.Cells[row, 6].Value = r.ExpectedRevenue;
                SetDateCell(ws.Cells[row, 7], r.StartDate);
                SetDateCell(ws.Cells[row, 8], r.EndDate);
                ws.Cells[row, 9].Value = r.TotalRevenue;
                ws.Cells[row, 10].Value = r.TotalCost;
                ws.Cells[row, 11].Value = r.Profit;
                ws.Cells[row, 12].Value = r.TotalMember;
                ws.Cells[row, 6, row, 6].Style.Numberformat.Format = "#,##0";
                ws.Cells[row, 9, row, 11].Style.Numberformat.Format = "#,##0";
                ApplyDataRowStyle(ws, row, headers.Length);
            }
            ws.View.FreezePanes(2, 1);
        }

        /// <summary>
        /// Tạo worksheet danh sách thành viên dự án trong file Excel xuất dữ liệu.
        /// </summary>
        /// <param name="package">Gói Excel đang xử lý.</param>
        /// <param name="data">Danh sách thành viên cần ghi.</param>
        private static void BuildSheetMembers(ExcelPackage package, List<MemberExportRow> data)
        {
            var ws = package.Workbook.Worksheets.Add("Thanh vien");
            var headers = new[]
            {
                "STT", "Mã dự án", "Tên dự án", "Khách hàng",
                "Tên sản phẩm/DV", "Mã nhân viên", "Tên thành viên", "Vai trò"
            };
            var colWidths = new[] { 5, 10, 35, 35, 40, 18, 28, 30 };
            ApplyHeader(ws, headers, colWidths);

            if (data == null || !data.Any()) return;
            for (int i = 0; i < data.Count; i++)
            {
                var r = data[i];
                int row = i + 2;
                ws.Cells[row, 1].Value = i + 1;
                ws.Cells[row, 2].Value = r.ProjectID;
                ws.Cells[row, 3].Value = r.ProjectName;
                ws.Cells[row, 4].Value = r.CustomerName;
                ws.Cells[row, 5].Value = r.NameProduct;
                ws.Cells[row, 6].Value = r.UserName;
                ws.Cells[row, 7].Value = r.FullName;
                ws.Cells[row, 8].Value = r.RoleNames;
                ApplyDataRowStyle(ws, row, headers.Length);
            }
            ws.View.FreezePanes(2, 1);
        }

        /// <summary>
        /// Tạo worksheet danh sách công việc dự án trong file Excel xuất dữ liệu.
        /// </summary>
        /// <param name="package">Gói Excel đang xử lý.</param>
        /// <param name="data">Danh sách công việc cần ghi.</param>
        private static void BuildSheetTasks(ExcelPackage package, List<TaskExportRow> data)
        {
            var ws = package.Workbook.Worksheets.Add("Cong viec");
            var headers = new[]
            {
                "STT", "Mã dự án", "Tên dự án", "Khách hàng",
                "Tên sản phẩm/DV", "Tên công việc", "Người thực hiện",
                "Ngày bắt đầu", "Ngày hoàn thành",
                "Trạng thái", "% Hoàn thành", "Ghi chú"
            };
            var colWidths = new[] { 5, 10, 35, 35, 35, 35, 25, 16, 16, 20, 12, 35 };
            ApplyHeader(ws, headers, colWidths);

            if (data == null || !data.Any()) return;
            for (int i = 0; i < data.Count; i++)
            {
                var r = data[i];
                int row = i + 2;
                ws.Cells[row, 1].Value = i + 1;
                ws.Cells[row, 2].Value = r.ProjectID;
                ws.Cells[row, 3].Value = r.ProjectName;
                ws.Cells[row, 4].Value = r.CustomerName;
                ws.Cells[row, 5].Value = r.NameProduct;
                ws.Cells[row, 6].Value = r.TaskName;
                ws.Cells[row, 7].Value = r.AssignedEmployeeNames;
                SetDateCell(ws.Cells[row, 8], r.StartDate);
                SetDateCell(ws.Cells[row, 9], r.CompletedDate);
                ws.Cells[row, 10].Value = r.TaskStatus;
                ws.Cells[row, 11].Value = r.CompletionPercentage.HasValue ? r.CompletionPercentage + "%" : "";
                ws.Cells[row, 12].Value = r.Note;
                ApplyDataRowStyle(ws, row, headers.Length);
            }
            ws.View.FreezePanes(2, 1);
        }

        /// <summary>
        /// Ghi ngày vào ô Excel theo định dạng dd/MM/yyyy hoặc để trống nếu không có dữ liệu.
        /// </summary>
        /// <param name="cell">Ô Excel cần ghi dữ liệu.</param>
        /// <param name="date">Ngày cần hiển thị.</param>
        private static void SetDateCell(ExcelRange cell, DateTime? date)
        {
            if (date.HasValue)
            {
                cell.Value = date.Value.ToString("dd/MM/yyyy");
            }
            else
            {
                cell.Value = "";
            }
        }

        /// <summary>
        /// Ghi dòng tiêu đề và định dạng phần header cho worksheet xuất dữ liệu.
        /// </summary>
        /// <param name="ws">Worksheet đang xử lý.</param>
        /// <param name="headers">Danh sách tiêu đề cột.</param>
        /// <param name="widths">Danh sách độ rộng tương ứng của các cột.</param>
        private static void ApplyHeader(ExcelWorksheet ws, string[] headers, int[] widths)
        {
            ws.Row(1).Height = 22;

            for (int c = 0; c < headers.Length; c++)
            {
                ExcelRange cell = ws.Cells[1, c + 1];
                cell.Value = headers[c];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = 11;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(GetColor("#1F4E79"));
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Column(c + 1).Width = widths[c];
            }
        }

        /// <summary>
        /// Áp dụng định dạng chung cho một dòng dữ liệu trong worksheet xuất Excel.
        /// </summary>
        /// <param name="ws">Worksheet đang xử lý.</param>
        /// <param name="row">Chỉ số dòng cần định dạng.</param>
        /// <param name="colCount">Tổng số cột của bảng dữ liệu.</param>
        private static void ApplyDataRowStyle(ExcelWorksheet ws, int row, int colCount)
        {
            ExcelRange range = ws.Cells[row, 1, row, colCount];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            range.Style.Font.Size = 10;

            if (row % 2 == 0)
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(GetColor("#EBF3FB"));
            }

            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        /// <summary>
        /// Lấy danh sách phòng ban mà user hiện tại được gán quyền xem qua Sys_UserBoPhan_GetByEmail.
        /// </summary>
        private List<MN_BoPhanModel> GetAccessibleDepartments()
        {
            var currentUser = _userCache.GetByUserName(User.UserName);
            if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.Email))
            {
                return new List<MN_BoPhanModel>();
            }

            return (_userBoPhanCache.GetByEmail(currentUser.Email) ?? new List<MN_BoPhanModel>())
                .GroupBy(x => x.BoPhan_ID)
                .Select(x => x.First())
                .OrderBy(x => x.TenBoPhanView)
                .ToList();
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo phòng ban
        /// </summary>
        [HttpGet]
        public JsonResult GetEmployeesByDepartment(int departmentId)
        {
            var employees = _employeeCache.GetByBoPhanAndChucVu(departmentId, null)
                .Select(x => new
                {
                    Value = x.UserId,
                    Text = x.FullName + " (" + x.UserName + ")",
                })
                .ToList();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult ViewHistory(int id)
        {
            var model = new RM_ProjectModel();
            model.ProjectID = id;
            return PartialView("_ViewHistory", model);
        }
    }
}
