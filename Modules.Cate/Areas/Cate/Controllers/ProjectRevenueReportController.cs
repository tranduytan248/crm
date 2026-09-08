using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class ProjectRevenueReportController : AppController
    {
        private const string ReportTypeMonth = "Month";
        private const string ReportTypeQuarter = "Quarter";
        private const string ReportTypeYear = "Year";
        private const string DateFormat = "dd/MM/yyyy";
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const string ExcelFileName = "BaocaoTH_DoanhThu_DuAn.xlsx";

        private readonly RM_ProjectRevenueReportCache _reportCache;
        private readonly string _reportTitle;

        private sealed class ReportDateRange
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }

        /// <summary>
        /// Khởi tạo cache và message phục vụ màn hình báo cáo tổng hợp doanh thu dự án.
        /// </summary>
        public ProjectRevenueReportController()
        {
            _reportCache = new RM_ProjectRevenueReportCache();
            _reportTitle = AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Title");
        }

        /// <summary>
        /// Hiển thị màn hình báo cáo tổng hợp doanh thu dự án và khởi tạo bộ lọc mặc định theo tháng hiện tại.
        /// </summary>
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Index()
        {
            return View(CreateDefaultSearchModel());
        }

        /// <summary>
        /// Tải partial kết quả báo cáo theo bộ lọc người dùng đã chọn.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetReport(RM_ProjectRevenueReportSearchModel search)
        {
            return PartialView("_Report", BuildReportResult(search));
        }

        /// <summary>
        /// Xuất Excel báo cáo tổng hợp doanh thu dự án theo cùng bộ dữ liệu đang hiển thị trên màn hình.
        /// </summary>
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Export(RM_ProjectRevenueReportSearchModel search)
        {
            var report = BuildReportResult(search);
            if (!report.IsValid)
            {
                return Content(
                    report.ValidationMessage ?? AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Message_InvalidDateRange"),
                    "text/plain",
                    Encoding.UTF8);
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("ProjectRevenue");
                BuildExcelWorksheet(worksheet, report);

                return File(package.GetAsByteArray(), ExcelContentType, ExcelFileName);
            }
        }

        /// <summary>
        /// Khởi tạo bộ lọc mặc định cho báo cáo theo tháng hiện tại.
        /// </summary>
        private RM_ProjectRevenueReportSearchModel CreateDefaultSearchModel()
        {
            var today = DateTime.Today;
            return new RM_ProjectRevenueReportSearchModel
            {
                ReportType = ReportTypeMonth,
                Month = today.Month,
                Year = today.Year,
                ReportTypes = BuildReportTypeOptions(ReportTypeMonth),
                Months = BuildMonthOptions(today.Month),
                Quarters = BuildQuarterOptions(null)
            };
        }

        /// <summary>
        /// Tạo danh sách lựa chọn loại báo cáo.
        /// </summary>
        private List<SelectListItem> BuildReportTypeOptions(string selectedValue)
        {
            return new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ReportTypeMonth,
                    Text = AppProcessor.Messagor.GetMessage("ProjectRevenueReport_ReportType_Month"),
                    Selected = string.Equals(selectedValue, ReportTypeMonth, StringComparison.OrdinalIgnoreCase)
                },
                new SelectListItem
                {
                    Value = ReportTypeQuarter,
                    Text = AppProcessor.Messagor.GetMessage("ProjectRevenueReport_ReportType_Quarter"),
                    Selected = string.Equals(selectedValue, ReportTypeQuarter, StringComparison.OrdinalIgnoreCase)
                },
                new SelectListItem
                {
                    Value = ReportTypeYear,
                    Text = AppProcessor.Messagor.GetMessage("ProjectRevenueReport_ReportType_Year"),
                    Selected = string.Equals(selectedValue, ReportTypeYear, StringComparison.OrdinalIgnoreCase)
                }
            };
        }

        /// <summary>
        /// Tạo danh sách tháng phục vụ bộ lọc báo cáo tháng.
        /// </summary>
        private List<SelectListItem> BuildMonthOptions(int? selectedMonth)
        {
            return Enumerable.Range(1, 12)
                .Select(month => new SelectListItem
                {
                    Value = month.ToString(),
                    Text = month.ToString(),
                    Selected = selectedMonth.HasValue && selectedMonth.Value == month
                })
                .ToList();
        }

        /// <summary>
        /// Tạo danh sách quý phục vụ bộ lọc báo cáo quý.
        /// </summary>
        private List<SelectListItem> BuildQuarterOptions(int? selectedQuarter)
        {
            return Enumerable.Range(1, 4)
                .Select(quarter => new SelectListItem
                {
                    Value = quarter.ToString(),
                    Text = quarter.ToString(),
                    Selected = selectedQuarter.HasValue && selectedQuarter.Value == quarter
                })
                .ToList();
        }

        /// <summary>
        /// validate bộ lọc báo cáo để loại bỏ giá trị thừa trước khi validate và truy vấn dữ liệu
        /// </summary>
        private RM_ProjectRevenueReportSearchModel NormalizeSearchModel(RM_ProjectRevenueReportSearchModel search)
        {
            var normalized = new RM_ProjectRevenueReportSearchModel
            {
                ReportType = NormalizeReportType(search?.ReportType),
                Month = search?.Month,
                Quarter = search?.Quarter,
                Year = search?.Year
            };

            switch (normalized.ReportType)
            {
                case ReportTypeMonth:
                    normalized.Quarter = null;
                    break;
                case ReportTypeQuarter:
                    normalized.Month = null;
                    break;
                case ReportTypeYear:
                    normalized.Month = null;
                    normalized.Quarter = null;
                    break;
            }

            return normalized;
        }

        /// <summary>
        /// validate giá trị loại báo cáo về các hằng số Month, Quarter hoặc Year
        /// </summary>
        private string NormalizeReportType(string reportType)
        {
            if (string.Equals(reportType, ReportTypeMonth, StringComparison.OrdinalIgnoreCase))
            {
                return ReportTypeMonth;
            }

            if (string.Equals(reportType, ReportTypeQuarter, StringComparison.OrdinalIgnoreCase))
            {
                return ReportTypeQuarter;
            }

            if (string.Equals(reportType, ReportTypeYear, StringComparison.OrdinalIgnoreCase))
            {
                return ReportTypeYear;
            }

            return null;
        }

        /// <summary>
        /// Kiểm tra hợp lệ điều kiện lọc báo cáo doanh thu dự án trước khi truy vấn dữ liệu
        /// </summary>
        private string ValidateSearchModel(RM_ProjectRevenueReportSearchModel search)
        {
            var reportType = search?.ReportType;
            if (string.IsNullOrWhiteSpace(reportType))
            {
                return AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Message_ReportTypeRequired");
            }

            if (!search.Year.HasValue || search.Year.Value <= 0)
            {
                return AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Message_YearRequired");
            }

            switch (reportType)
            {
                case ReportTypeMonth:
                    if (!search.Month.HasValue || search.Month.Value < 1 || search.Month.Value > 12)
                    {
                        return AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Message_MonthRequired");
                    }

                    return null;
                case ReportTypeQuarter:
                    if (!search.Quarter.HasValue || search.Quarter.Value < 1 || search.Quarter.Value > 4)
                    {
                        return AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Message_QuarterRequired");
                    }

                    return null;
                case ReportTypeYear:
                    return null;
                default:
                    return AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Message_InvalidReportType");
            }
        }

        /// <summary>
        /// Đóng gói dữ liệu báo cáo, thông tin kỳ báo cáo và trạng thái hợp lệ để dùng chung cho partial và export
        /// </summary>
        private RM_ProjectRevenueReportResultModel BuildReportResult(RM_ProjectRevenueReportSearchModel search)
        {
            var normalizedSearch = NormalizeSearchModel(search);
            var validationMessage = ValidateSearchModel(normalizedSearch);
            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                return CreateInvalidResult(validationMessage);
            }

            var reportDateRange = ResolveReportDateRange(normalizedSearch);
            var rawItems = _reportCache.GetReport(reportDateRange.FromDate, reportDateRange.ToDate, User.UserName)
                ?? new List<RM_ProjectRevenueReportModel>();

            return new RM_ProjectRevenueReportResultModel
            {
                IsValid = true,
                PeriodDisplayText = BuildPeriodDisplayText(normalizedSearch),
                Items = NormalizeReportItems(rawItems),
                GrandTotalRevenue = rawItems.Sum(item => item.TotalRevenue)
            };
        }

        /// <summary>
        /// Tạo model kết quả không hợp lệ để partial hiển thị thông báo lỗi từ phía server
        /// </summary>
        private RM_ProjectRevenueReportResultModel CreateInvalidResult(string validationMessage)
        {
            return new RM_ProjectRevenueReportResultModel
            {
                IsValid = false,
                ValidationMessage = validationMessage,
                Items = new List<RM_ProjectRevenueReportModel>()
            };
        }

        /// <summary>
        /// Quy đổi điều kiện lọc báo cáo doanh thu dự án về khoảng ngày bắt đầu và ngày kết thúc
        /// </summary>
        private ReportDateRange ResolveReportDateRange(RM_ProjectRevenueReportSearchModel search)
        {
            switch (search.ReportType)
            {
                case ReportTypeMonth:
                    return GetMonthRange(search.Year.Value, search.Month.Value);
                case ReportTypeQuarter:
                    return GetQuarterRange(search.Year.Value, search.Quarter.Value);
                default:
                    return GetYearRange(search.Year.Value);
            }
        }

        /// <summary>
        /// Trả về khoảng ngày từ ngày đầu tháng đến ngày cuối tháng của tháng được chọn
        /// </summary>
        private ReportDateRange GetMonthRange(int year, int month)
        {
            var fromDate = new DateTime(year, month, 1);
            return new ReportDateRange
            {
                FromDate = fromDate,
                ToDate = fromDate.AddMonths(1).AddDays(-1)
            };
        }

        /// <summary>
        /// Trả về khoảng ngày từ ngày đầu quý đến ngày cuối quý của quý được chọn
        /// </summary>
        private ReportDateRange GetQuarterRange(int year, int quarter)
        {
            var startMonth = ((quarter - 1) * 3) + 1;
            var fromDate = new DateTime(year, startMonth, 1);
            return new ReportDateRange
            {
                FromDate = fromDate,
                ToDate = fromDate.AddMonths(3).AddDays(-1)
            };
        }

        /// <summary>
        /// Trả về khoảng ngày từ ngày đầu năm đến ngày cuối năm của năm được chọn.
        /// </summary>
        private ReportDateRange GetYearRange(int year)
        {
            return new ReportDateRange
            {
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31)
            };
        }

        /// <summary>
        /// Dựng text hiển thị kỳ báo cáo theo loại báo cáo và thời gian người dùng đã chọn
        /// </summary>
        private string BuildPeriodDisplayText(RM_ProjectRevenueReportSearchModel search)
        {
            switch (search.ReportType)
            {
                case ReportTypeMonth:
                    return string.Format(
                        "{0} {1}/{2}",
                        AppProcessor.Messagor.GetMessage("ProjectRevenueReport_ReportType_Month"),
                        search.Month,
                        search.Year);
                case ReportTypeQuarter:
                    return string.Format(
                        "{0} {1}/{2}",
                        AppProcessor.Messagor.GetMessage("ProjectRevenueReport_ReportType_Quarter"),
                        search.Quarter,
                        search.Year);
                default:
                    return string.Format(
                        "{0} {1}",
                        AppProcessor.Messagor.GetMessage("ProjectRevenueReport_ReportType_Year"),
                        search.Year);
            }
        }

        /// <summary>
        /// Làm sạch dữ liệu text trước khi render hoặc export và chuẩn hóa thời gian ghi nhận doanh thu.
        /// </summary>
        private List<RM_ProjectRevenueReportModel> NormalizeReportItems(IEnumerable<RM_ProjectRevenueReportModel> items)
        {
            var normalizedItems = (items ?? Enumerable.Empty<RM_ProjectRevenueReportModel>())
                .Select(CreateNormalizedReportItem)
                .OrderBy(item => item.ProjectName)
                .ThenBy(item => item.CustomerName)
                .ThenBy(item => item.RevenueFromDate)
                .ThenBy(item => item.RevenueToDate)
                .ThenBy(item => item.ServiceName)
                .ThenBy(item => item.ProductProjectID)
                .ToList();

            var displayItems = new List<RM_ProjectRevenueReportModel>();
            var previousGroupKey = string.Empty;
            var order = 0;

            foreach (var item in normalizedItems)
            {
                var currentGroupKey = BuildDisplayGroupKey(item);
                var isFirstRowOfGroup = !string.Equals(previousGroupKey, currentGroupKey, StringComparison.Ordinal);
                if (isFirstRowOfGroup)
                {
                    order++;
                }

                item.DisplayOrderText = isFirstRowOfGroup ? order.ToString(CultureInfo.InvariantCulture) : string.Empty;
                item.DisplayProjectName = isFirstRowOfGroup ? item.ProjectName : string.Empty;
                item.DisplayCustomerName = isFirstRowOfGroup ? item.CustomerName : string.Empty;
                item.DisplayRevenueDateText = isFirstRowOfGroup ? item.RevenueDateText : string.Empty;

                displayItems.Add(item);
                previousGroupKey = currentGroupKey;
            }

            return displayItems;
        }

        /// <summary>
        /// Chuẩn hóa một dòng dữ liệu thô từ SP về model đã dùng cho hiển thị và xuất Excel.
        /// </summary>
        private RM_ProjectRevenueReportModel CreateNormalizedReportItem(RM_ProjectRevenueReportModel item)
        {
            return new RM_ProjectRevenueReportModel
            {
                ProductProjectID = item.ProductProjectID,
                ProjectID = item.ProjectID,
                ProjectName = NormalizeText(item.ProjectName),
                CustomerName = NormalizeText(item.CustomerName),
                ServiceName = NormalizeText(item.ServiceName),
                RevenueFromDate = item.RevenueFromDate,
                RevenueToDate = item.RevenueToDate,
                RevenueDateText = BuildRevenueDateText(item.RevenueFromDate, item.RevenueToDate),
                TotalRevenue = item.TotalRevenue
            };
        }

        /// <summary>
        /// Tạo khóa nhóm hiển thị để các dòng cùng dự án không lặp lại STT, dự án, khách hàng và ngày thực hiện.
        /// </summary>
        private string BuildDisplayGroupKey(RM_ProjectRevenueReportModel item)
        {
            return string.Format(
                "{0}|{1}|{2}|{3}",
                item.ProjectID,
                item.ProjectName,
                item.CustomerName,
                item.RevenueDateText);
        }

        /// <summary>
        /// Chuẩn hóa text về dạng một khoảng trắng giữa các từ và loại bỏ giá trị rỗng thừa.
        /// </summary>
        private string NormalizeText(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : string.Join(" ", value.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Dựng text thời gian ghi nhận doanh thu từ ngày nhỏ nhất và ngày lớn nhất trong kỳ của từng sản phẩm dự án.
        /// </summary>
        private string BuildRevenueDateText(DateTime fromDate, DateTime toDate)
        {
            if (fromDate.Date == toDate.Date)
            {
                return fromDate.ToString(DateFormat);
            }

            return string.Format("{0} - {1}", fromDate.ToString(DateFormat), toDate.ToString(DateFormat));
        }

        /// <summary>
        /// Dựng toàn bộ nội dung worksheet báo cáo tổng hợp doanh thu dự án.
        /// </summary>
        private void BuildExcelWorksheet(ExcelWorksheet worksheet, RM_ProjectRevenueReportResultModel report)
        {
            ConfigureExcelWorksheet(worksheet);
            WriteExcelHeader(worksheet, report);
            WriteExcelBody(worksheet, report);
        }

        /// <summary>
        /// Cấu hình font, grid line và độ rộng cột cho worksheet báo cáo.
        /// </summary>
        private void ConfigureExcelWorksheet(ExcelWorksheet worksheet)
        {
            worksheet.Cells.Style.Font.Name = "Times New Roman";
            worksheet.Cells.Style.Font.Size = 11;
            worksheet.View.ShowGridLines = true;

            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 42;
            worksheet.Column(3).Width = 30;
            worksheet.Column(4).Width = 22;
            worksheet.Column(5).Width = 26;
            worksheet.Column(6).Width = 18;
            worksheet.Column(7).Width = 10;
            worksheet.Column(8).Width = 20;
            worksheet.Column(9).Width = 14;
        }

        /// <summary>
        /// Ghi phần tiêu đề, kỳ báo cáo và header cột cho file Excel.
        /// </summary>
        private void WriteExcelHeader(ExcelWorksheet worksheet, RM_ProjectRevenueReportResultModel report)
        {
            worksheet.Cells["A1:I1"].Merge = true;
            worksheet.Cells["A1"].Value = _reportTitle;
            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["A1"].Style.Font.Size = 12;
            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells["A2"].Value = AppProcessor.Messagor.GetMessage("ProjectRevenueReport_ReportPeriod");
            worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells["B2:F2"].Merge = true;
            worksheet.Cells["B2"].Value = report.PeriodDisplayText;

            worksheet.Cells["H4"].Value = AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Column_TotalRevenue");
            worksheet.Cells["I4"].Value = report.GrandTotalRevenue;
            worksheet.Cells["I4"].Style.Numberformat.Format = "#,##0";
            worksheet.Cells["I4"].Style.Font.Bold = true;
            worksheet.Cells["I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            var headers = new[]
            {
                AppProcessor.Messagor.GetMessage("Column_No"),
                AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Column_Project"),
                AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Column_Customer"),
                AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Column_RevenueDate"),
                AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Column_Service"),
                AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Column_TotalRevenue")
            };

            for (var index = 0; index < headers.Length; index++)
            {
                worksheet.Cells[4, index + 1].Value = headers[index];
            }

            ApplyTableHeaderStyle(worksheet.Cells[4, 1, 4, 6]);
            ApplyTableHeaderStyle(worksheet.Cells[4, 8, 4, 8]);
        }

        /// <summary>
        /// Ghi nội dung chi tiết báo cáo vào worksheet theo danh sách sản phẩm dự án.
        /// </summary>
        private void WriteExcelBody(ExcelWorksheet worksheet, RM_ProjectRevenueReportResultModel report)
        {
            var currentRow = 5;
            var items = report.Items ?? new List<RM_ProjectRevenueReportModel>();

            if (!items.Any())
            {
                worksheet.Cells[currentRow, 1, currentRow, 6].Merge = true;
                worksheet.Cells[currentRow, 1].Value = AppProcessor.Messagor.GetMessage("ProjectRevenueReport_Message_NoData");
                worksheet.Cells[currentRow, 1, currentRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, 1, currentRow, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                return;
            }

            foreach (var item in items)
            {
                WriteExcelDetailRow(worksheet, currentRow, item);
                currentRow++;
            }
        }

        /// <summary>
        /// Ghi một dòng dữ liệu doanh thu dự án vào Excel.
        /// </summary>
        private void WriteExcelDetailRow(ExcelWorksheet worksheet, int row, RM_ProjectRevenueReportModel item)
        {
            worksheet.Cells[row, 1].Value = item.DisplayOrderText;
            worksheet.Cells[row, 2].Value = item.DisplayProjectName;
            worksheet.Cells[row, 3].Value = item.DisplayCustomerName;
            worksheet.Cells[row, 4].Value = item.DisplayRevenueDateText;
            worksheet.Cells[row, 5].Value = item.ServiceName;
            worksheet.Cells[row, 6].Value = item.TotalRevenue;

            worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[row, 6].Style.Numberformat.Format = "#,##0";
            worksheet.Cells[row, 2, row, 5].Style.WrapText = true;
            worksheet.Cells[row, 1, row, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        }

        /// <summary>
        /// Áp dụng style cho header của bảng dữ liệu Excel.
        /// </summary>
        private void ApplyTableHeaderStyle(ExcelRange range)
        {
            range.Style.Font.Bold = true;
            range.Style.WrapText = true;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
        }
    }
}
