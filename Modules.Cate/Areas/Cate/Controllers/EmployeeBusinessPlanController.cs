using ClosedXML.Excel;
using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class EmployeeBusinessPlanController : AppController
    {
        private readonly RM_Employee_BusinessPlanCache _employeeBusinessPlanCache;
        private readonly MN_EmployeeCache _employeeCache;
        private readonly RM_BusinessPlanCache _businessPlanCache;
        private readonly MN_BoPhanCache _boPhanCache;
        private readonly string _employeeBusinessPlanTitle = AppProcessor.Messagor.GetMessage("EmployeeBusinessPlan_Title");
        public EmployeeBusinessPlanController() {
            _employeeBusinessPlanCache = new RM_Employee_BusinessPlanCache();
            _employeeCache = new MN_EmployeeCache();
            _businessPlanCache = new RM_BusinessPlanCache();
            _boPhanCache = new MN_BoPhanCache();
        }

        public ActionResult Index()
        {
            var searchModel = new Employee_BusinessPlanSearchModel()
            {
                ListBoPhan = _boPhanCache.GetAll()
            };
            return View(searchModel);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public JsonResult Get(int year, int? boPhanId)
        {
            var data = _employeeBusinessPlanCache.GetData(year, boPhanId, out var total);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult EmployeeBusinessPlan(int id)
        {
            var model = new RM_Employee_BusinessPlanModel();
            model.BusinessPlanID = id;
            return PartialView("EmployeeBusinessPlan", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetEmployeeBusinessPlan(int businessPlanID)
        {
            var draw = Request.Form.GetValues("draw")?[0];
            var order = Request.Form.GetValues("order[0][column]")?[0];
            var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0]);

            var dataSearch = new BaseSearchModel
            {
                Order = order,
                OrderDir = orderDir,
                StartIndex = startRec,
                PageSize = pageSize
            };
            var data = _employeeBusinessPlanCache.Get(businessPlanID, out var total, dataSearch);
            return Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public JsonResult GetEmployeeByDepartment(int boPhanId)
        {
            var data = _employeeCache.GetByBoPhanID(boPhanId).Select(x => new SelectListItem
            {
                Value = x.Employee_ID.ToString(),
                Text = $"{x.FullName} - {x.Employee_Code}"
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult Add(int? id)
        {
            var model = new RM_Employee_BusinessPlanModel();
            if (id.HasValue)
            {
                model.BusinessPlanID = id.Value;
            }
            model.ListBusinessPlan = _businessPlanCache.GetAll()
                .Select(d => new SelectListItem
                {
                    Text = d.BusinessPlanName,
                    Value = d.BusinessPlanID.ToString()
                }).ToList();
            model.ListEmployee = new List<SelectListItem>();
            model.ListBoPhan = _boPhanCache.GetAll();
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(RM_Employee_BusinessPlanModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListBusinessPlan = _businessPlanCache.GetAll()
                   .Select(d => new SelectListItem
                   {
                       Text = d.BusinessPlanName,
                       Value = d.BusinessPlanID.ToString()
                   }).ToList();
                model.ListBoPhan = _boPhanCache.GetAll();
                if (model.BoPhan_ID > 0)
                {
                    model.ListEmployee = _employeeCache.GetByBoPhanID(model.BoPhan_ID)
                        .Select(x => new SelectListItem
                        {
                            Value = x.Employee_ID.ToString(),
                            Text = $"{x.FullName} - {x.Employee_Code}"
                        }).ToList();
                }
                else
                {
                    model.ListEmployee = new List<SelectListItem>();
                }
                return PartialView("_EmployeeBusinessPlan", model);
            }

            string response;

            var result = _employeeBusinessPlanCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_employeeBusinessPlanTitle}",
                    EnumProcessType.Add,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_employeeBusinessPlanTitle}",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_employeeBusinessPlanTitle}",
                    EnumProcessType.Add,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _employeeBusinessPlanCache.GetById(id);
            model.ListBusinessPlan = _businessPlanCache.GetAll()
                .Select(d => new SelectListItem
                {
                    Text = d.BusinessPlanName,
                    Value = d.BusinessPlanID.ToString()
                }).ToList();
            model.ListBoPhan = _boPhanCache.GetAll();
            if (model.BoPhan_ID > 0)
            {
                model.ListEmployee = _employeeCache.GetByBoPhanID(model.BoPhan_ID)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Employee_ID.ToString(),
                        Text = $"{x.FullName} - {x.Employee_Code}"
                    }).ToList();
            }
            else
            {
                model.ListEmployee = new List<SelectListItem>();
            }
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_employeeBusinessPlanTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(RM_Employee_BusinessPlanModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListBusinessPlan = _businessPlanCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.BusinessPlanName,
                        Value = d.BusinessPlanID.ToString()
                    }).ToList();
                model.ListBoPhan = _boPhanCache.GetAll();
                if (model.BoPhan_ID > 0)
                {
                    model.ListEmployee = _employeeCache.GetByBoPhanID(model.BoPhan_ID)
                        .Select(x => new SelectListItem
                        {
                            Value = x.Employee_ID.ToString(),
                            Text = $"{x.FullName} - {x.Employee_Code}"
                        }).ToList();
                }
                else
                {
                    model.ListEmployee = new List<SelectListItem>();
                }
                return PartialView("_EmployeeBusinessPlan", model);
            }
            string response;

            var result = _employeeBusinessPlanCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_employeeBusinessPlanTitle}",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Error);

            else if (result == -9)
                response = CreateMessage($"{_employeeBusinessPlanTitle}",
                    EnumProcessType.DataExisted,
                    EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_employeeBusinessPlanTitle}",
                    EnumProcessType.Edit,
                    EnumMsgIcon.Success);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id)
        {
            var model = _employeeBusinessPlanCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_employeeBusinessPlanTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_employeeBusinessPlanTitle}");

            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(RM_Employee_BusinessPlanModel model)
        {
            var deleted = _employeeBusinessPlanCache.Delete(model, User.UserName);

            var response = CreateMessage($"{_employeeBusinessPlanTitle}",
                EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #region Import
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Import()
        {
            var model = new Import_Employee_BusinessPlanModel();
            model.UserName = User.UserName;
            model.Key = "Import_Employee_BusinessPlan_" + User.UserName + DateTime.Now.ToString("ddMMyyyyHHmmss");
            return View(model);
        }

        // Nút Import dữ liệu
        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult ImportData(string key)
        {
            var importModel = new Import_Employee_BusinessPlanModel();
            importModel.Key = key;
            importModel.ListBusinessPlan = _businessPlanCache.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Text = d.BusinessPlanName,
                        Value = d.BusinessPlanID.ToString()
                    }).ToList();
            return PartialView("_ImportData", importModel);
        }

        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult ImportData(Import_Employee_BusinessPlanModel model)
        {
            var response = "";
            // kiểm tra phải có ít nhất 1 tập tin
            if (Request.Files == null || Request.Files.Count == 0)
            {
                response = CreateMessage($"Không có file",
                       EnumProcessType.NonFormat, EnumMsgIcon.Error);
                return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
            }

            HttpPostedFileBase file = Request.Files[0];

            var dataExcel = ReadDataImport(file);
            if (dataExcel != null)
            {
                var dataValids = _employeeBusinessPlanCache.ValidData(dataExcel, model.BusinessPlanID, model.Key, User.UserName);

                //Lưu thông tin xuông database
                int result = dataValids.Count;

                if (result > 0)
                {
                    response = CreateMessage($"[{result}] dữ liệu import thành công!",
                                           EnumProcessType.NonFormat, EnumMsgIcon.Success);
                    return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    response = CreateMessage($"Import dữ liệu thất bại",
                        EnumProcessType.NonFormat, EnumMsgIcon.Error);
                    return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                response = CreateMessage($"dữ liệu",
                        EnumProcessType.Add, EnumMsgIcon.Error);
                return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
            }
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetDSImport(Import_Employee_BusinessPlanModel model)
        {
            var search = Request.Form.GetValues("search[value]")?[0];
            var draw = Request.Form.GetValues("draw")?[0];
            var order = Request.Form.GetValues("order[0][column]")?[0];
            var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = 50;
            var data = _employeeBusinessPlanCache.GetData(model.Key);
            if (data == null)
            {
                data = new List<Import_Employee_BusinessPlanViewModel>();
            }
            for (int i = 1; i <= data.Count; i++)
            {
                data[i - 1].STT = i;
            }
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = data.Count, recordsFiltered = data.Count, data = data.Where(p => p.STT > startRec && p.STT <= (startRec + pageSize)) },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        // Nút Save
        [HttpGet]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult SaveImport(string key)
        {
            var data = _employeeBusinessPlanCache.GetData(key);

            ViewBag.InfoImport = "";
            if (data == null)
            {
                ViewBag.Message = "Hiện tại không có dữ liệu. Bạn vui lòng Import dữ liệu trước khi thực hiện Xác nhận.";
                ViewBag.Err = "Err";
            }
            else
            {
                if (data.Where(p => !string.IsNullOrEmpty(p.Message)).Count() > 0)
                {
                    ViewBag.Message = "Dữ liệu lỗi, không thể lưu vào hệ thống. Vui lòng kiểm tra lại.";
                    ViewBag.Err = "Err";
                }
                else
                {
                    ViewBag.Message = "Bạn có chắc chắn muốn lưu dữ liệu này vào hệ thống?<br/><b>Số lượng: " + data.Count + "</b>";
                    ViewBag.Err = "";
                }
            }
            var model = new Import_Employee_BusinessPlanModel();
            //model.Key = key;
            return PartialView("_Save", model);
        }

        // Save handle
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Save(Import_Employee_BusinessPlanModel model)
        {
            var response = "";
            var data = _employeeBusinessPlanCache.GetData(model.Key);
            //Lưu thông tin xuông database
            int result = 0;
            result = _employeeBusinessPlanCache.Import(model.Key, User.UserName);
            if (result > 0)
            {
                response = CreateMessage($"[{data.Count}] dữ liệu xác nhận thành công!",
                                       EnumProcessType.NonFormat, EnumMsgIcon.Success);
                return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                response = CreateMessage($"Xác nhận dữ liệu thất bại",
                    EnumProcessType.NonFormat, EnumMsgIcon.Error);
                return Json(new { status = false, message = response }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Đọc file excel trả dữ liệu về dạng DataTable
        /// </summary>
        /// <param name="fileImport"></param>
        /// <returns></returns>
        private DataTable ReadDataImport(HttpPostedFileBase fileImport)
        {
            var workbook = new XLWorkbook(fileImport.InputStream);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null) return null;

            var dataTable = new DataTable();

            // Số dòng thực tế (có data)
            int lastRow = worksheet.LastRowUsed().RowNumber();
            var headerRow = worksheet.Row(1);
            for (int col = 1; col <= 5; col++)
            {
                var header = headerRow.Cell(col).Value.ToString().Trim();

                // Tránh null hoặc trùng tên cột
                if (string.IsNullOrEmpty(header))
                    header = $"Column{col}";

                if (dataTable.Columns.Contains(header))
                    header = $"{header}_{col}";

                dataTable.Columns.Add(header, typeof(string));
            }

            for (int row = 2; row <= lastRow; row++)
            {
                var dataRow = dataTable.NewRow();

                for (int col = 1; col <= 5; col++)
                {
                    var cellValue = worksheet.Cell(row, col).Value?.ToString()?.Trim();
                    dataRow[col - 1] = string.IsNullOrEmpty(cellValue) ? "" : cellValue;
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        // Nút làm mới
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Clear(string key)
        {
            var model = new Import_Employee_BusinessPlanModel();
            model.Key = key;
            return PartialView("_Clear", model);
        }

        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Clear(Import_Employee_BusinessPlanModel model)
        {
            var result = _employeeBusinessPlanCache.Clear(model.Key);

            var response = CreateMessage($"dữ liệu import",
                EnumProcessType.Delete, result > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
        #endregion
    }
}