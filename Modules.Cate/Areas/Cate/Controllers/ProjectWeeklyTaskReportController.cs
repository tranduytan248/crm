using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class ProjectWeeklyTaskReportController : AppController
    {

        private const string DateFormat = "dd/MM/yyyy";
        private const string WeeklyPlanDatePattern = @"^(?:Ngày\s*)?(?<date>\d{2}/\d{2}/\d{4})(?:\s+(?<time>\d{2}:\d{2}))?(?:\s*-\s*(?<title>.+))?$";
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private readonly RM_ProjectWeeklyTaskReportCache _reportCache;

        public ProjectWeeklyTaskReportController()
        {
            _reportCache = new RM_ProjectWeeklyTaskReportCache();
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]

        public ActionResult Index()
        {
            return View(CreateSearchModelForCurrentWeek());
        }

        /// <summary>
        /// Quy đổi một ngày bất kỳ về ngày đầu tuần tương ứng.
        /// </summary>
        private DateTime GetMondayOfWeek(DateTime date)
        {
            var diff = date.DayOfWeek == DayOfWeek.Sunday
                ? -6
                : DayOfWeek.Monday - date.DayOfWeek;

            return date.Date.AddDays(diff);
        }


        /// <summary>
        /// Tải partial kết quả báo cáo theo bộ lọc người dùng đã chọn.
        /// </summary>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetReport(RM_ProjectWeeklyTaskReportSearchModel search)
        {
            return PartialView("_Report", BuildReportResult(search));
        }

        /// <summary>
        /// Xuất Excel từ cùng một nguồn dữ liệu đang dùng cho bảng kết quả để tránh lệch logic hiển thị.
        /// </summary>
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Export(RM_ProjectWeeklyTaskReportSearchModel search)
        {
            var report = BuildReportResult(search);
            var fileName = string.Format(
                "Bao_cao_cong_viec_du_an_theo_tuan_{0}_{1}_{2}.xlsx",
                report.WeekFrom.ToString("ddMM"),
                report.WeekTo.ToString("ddMM"),
                DateTime.Now.ToString("ddMMyyyyHHmm"));

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                BuildExcelWorksheet(worksheet, report);

                return File(package.GetAsByteArray(), ExcelContentType, fileName);
            }
        }


        /// <summary>
        /// Đọc dữ liệu thô từ cache, chuẩn hóa lại và đóng gói thành model dùng chung cho grid và export.
        /// </summary>
        private RM_ProjectWeeklyTaskReportModelResultModel BuildReportResult(RM_ProjectWeeklyTaskReportSearchModel search)
        {
            var safeSearch = search ?? new RM_ProjectWeeklyTaskReportSearchModel();
            var weekRange = ResolveWeekRange(safeSearch);

            var rawItems = _reportCache.GetReport(
                               weekRange.Item1,
                               weekRange.Item2,
                               User.UserName)
                           ?? new List<RM_ProjectWeeklyTaskReportModel>();

            return new RM_ProjectWeeklyTaskReportModelResultModel
            {
                WeekFrom = weekRange.Item1,
                WeekTo = weekRange.Item2,
                Items = NormalizeReportItems(rawItems)
            };
        }

        /// <summary>
        /// Làm sạch dữ liệu text trước khi render hoặc export: bỏ HTML và chuẩn hóa nội dung trao đổi, kế hoạch.
        /// </summary>
        private List<RM_ProjectWeeklyTaskReportModel> NormalizeReportItems(IEnumerable<RM_ProjectWeeklyTaskReportModel> items)
            {
                return (items ?? Enumerable.Empty<RM_ProjectWeeklyTaskReportModel>())
                .Select(item =>
                {
                    var comments = new List<ProjectCommentDto>();

                    if (!string.IsNullOrWhiteSpace(item.LastComment))
                    {
                        try
                        {
                            comments =
                                JsonConvert.DeserializeObject<List<ProjectCommentDto>>
                                (
                                    item.LastComment
                                ) ?? new List<ProjectCommentDto>();
                        }
                        catch
                        {
                            comments = new List<ProjectCommentDto>();
                        }
                    }

                    return new RM_ProjectWeeklyTaskReportModel
                    {
                        ProjectID = item.ProjectID,
                        ProjectName = NormalizeSingleLine(item.ProjectName),
                        LastTaskDate = item.LastTaskDate,
                        AMNames = NormalizeSingleLine(item.AMNames),

                        WeeklyTasks = item.WeeklyTasks,

                        LastComment = item.LastComment,
                        CommentItems = comments,

                        LastTaskName = NormalizeSingleLine(item.LastTaskName),
                        LastCommentBy = NormalizeSingleLine(item.LastCommentBy),
                        LastCommentDate = item.LastCommentDate
                    };
                })
                .ToList();
        }

        /// <summary>
        /// Quy đổi giá trị tuần người dùng chọn về khoảng từ Thứ Hai đến Chủ Nhật.
        /// </summary>
        private Tuple<DateTime, DateTime> ResolveWeekRange(RM_ProjectWeeklyTaskReportSearchModel search)
        {
            var fallbackWeekStart = GetMondayOfWeek(DateTime.Today);
            var defaultRange = Tuple.Create(fallbackWeekStart, fallbackWeekStart.AddDays(6));

            if (search == null)
            {
                return defaultRange;
            }

            DateTime fromDate;
            DateTime toDate;
            if (TryParseDate(search.FromDate, out fromDate)
                && TryParseDate(search.ToDate, out toDate)
                && toDate >= fromDate)
            {
                var weekStart = GetMondayOfWeek(fromDate);
                return Tuple.Create(weekStart, weekStart.AddDays(6));
            }

            DateTime weekDate;
            if (TryParseDate(search.WeekDate, out weekDate))
            {
                var weekStart = GetMondayOfWeek(weekDate);
                return Tuple.Create(weekStart, weekStart.AddDays(6));
            }

            return defaultRange;
        }

        /// <summary>
        /// Parse ngày theo đúng định dạng bộ lọc tuần của màn hình.
        /// </summary>
        private bool TryParseDate(string value, out DateTime date)
        {
            return DateTime.TryParseExact(
                value,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date);
        }

        private string CleanRichText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var text = value
                .Replace("<br>", "\n")
                .Replace("<br/>", "\n")
                .Replace("<br />", "\n")
                .Replace("</p>", "\n")
                .Replace("</div>", "\n")
                .Replace("</li>", "\n");

            text = Regex.Replace(text, "<[^>]+>", " ");
            text = HttpUtility.HtmlDecode(text ?? string.Empty) ?? string.Empty;
            text = text.Replace("\r\n", "\n").Replace("\r", "\n");
            text = Regex.Replace(text, @"[ \t]+", " ");
            text = Regex.Replace(text, @"\n{3,}", "\n\n");

            var lines = SplitToLines(text)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            return lines.Any()
                ? string.Join(Environment.NewLine, lines).Trim()
                : string.Empty;
        }

        /// <summary>
        /// Tách text thành danh sách dòng và loại bỏ dòng trống.
        /// </summary>
        private List<string> SplitToLines(string value)
        {
            return (value ?? string.Empty)
                .Split(new[] { '\n' }, StringSplitOptions.None)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        /// <summary>
        /// Chuẩn hóa text kế hoạch tuần về dạng ngày giờ và tên kế hoạch.
        /// </summary>
        private string FormatWeeklyPlan(string value)
        {
            var lines = SplitToLines(CleanRichText(value));
            if (!lines.Any())
            {
                return string.Empty;
            }

            var blocks = new List<string>();

            for (var index = 0; index < lines.Count; index++)
            {
                var currentLine = lines[index];
                var dateMatch = Regex.Match(currentLine, WeeklyPlanDatePattern, RegexOptions.IgnoreCase);

                if (!dateMatch.Success)
                {
                    continue;
                }

                var displayDate = BuildWeeklyPlanDateText(dateMatch);
                var title = ResolveWeeklyPlanTitle(lines, ref index, dateMatch);

                blocks.Add(
                    string.IsNullOrWhiteSpace(title)
                        ? displayDate
                        : string.Format("{0}{1}({2})", displayDate, Environment.NewLine, title));
            }

            return string.Join(Environment.NewLine + Environment.NewLine, blocks.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        /// <summary>
        /// Dựng dòng hiển thị ngày giờ kế hoạch.
        /// </summary>
        private string BuildWeeklyPlanDateText(Match dateMatch)
        {
            var dateText = dateMatch.Groups["date"].Value;
            var timeText = dateMatch.Groups["time"].Value;

            if (string.IsNullOrWhiteSpace(timeText))
            {
                return dateText;
            }

            return string.Format("{0}  {1}", dateText, timeText);
        }

        /// <summary>
        /// Xác định tiêu đề kế hoạch từ cùng dòng ngày hoặc dòng kế tiếp.
        /// </summary>
        private string ResolveWeeklyPlanTitle(IList<string> lines, ref int index, Match dateMatch)
        {
            var title = (dateMatch.Groups["title"].Value ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(title) && HasNextNonDateLine(lines, index))
            {
                index++;
                title = lines[index];
            }

            return NormalizeWeeklyPlanTitle(title);
        }

        /// <summary>
        /// Kiểm tra dòng kế tiếp có phải là nội dung kế hoạch hay đã là block ngày khác.
        /// </summary>
        private bool HasNextNonDateLine(IList<string> lines, int index)
        {
            return index + 1 < lines.Count && !IsWeeklyPlanDateLine(lines[index + 1]);
        }

        /// <summary>
        /// Chuẩn hóa tiêu đề kế hoạch, loại bỏ ngoặc ngoài nếu đã có sẵn.
        /// </summary>
        private string NormalizeWeeklyPlanTitle(string value)
        {
            var title = NormalizeSingleLine(value);
            if (string.IsNullOrWhiteSpace(title))
            {
                return string.Empty;
            }

            if (title.StartsWith("(") && title.EndsWith(")") && title.Length > 1)
            {
                title = title.Substring(1, title.Length - 2).Trim();
            }

            return title;
        }

        /// <summary>
        /// Chuẩn hóa một dòng text về dạng một khoảng trắng giữa các từ.
        /// </summary>
        private string NormalizeSingleLine(string value)
        {
            return Regex.Replace((value ?? string.Empty).Trim(), @"\s+", " ");
        }


        /// <summary>
        /// Kiểm tra một dòng có phải là dòng ngày kế hoạch hay không.
        /// </summary>
        private bool IsWeeklyPlanDateLine(string value)
        {
            return Regex.IsMatch(value ?? string.Empty, WeeklyPlanDatePattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Khởi tạo bộ lọc mặc định cho tuần hiện tại và danh sách phòng ban user được phép xem.
        /// </summary>
        private RM_ProjectWeeklyTaskReportSearchModel CreateSearchModelForCurrentWeek()
        {
            var weekStart = GetMondayOfWeek(DateTime.Today);
            var weekEnd = weekStart.AddDays(6);

            return new RM_ProjectWeeklyTaskReportSearchModel
            {
                WeekDate = weekStart.ToString(DateFormat),
                FromDate = weekStart.ToString(DateFormat),
                ToDate = weekEnd.ToString(DateFormat),
            };
        }

        private void BuildExcelWorksheet(
    ExcelWorksheet ws,
    RM_ProjectWeeklyTaskReportModelResultModel report)
        {
            int row = 1;

            #region Title

            ws.Cells[row, 1].Value = "BÁO CÁO CÔNG VIỆC TUẦN";
            ws.Cells[row, 1, row, 5].Merge = true;
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 1].Style.Font.Size = 16;
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            row += 2;

            ws.Cells[row, 1].Value =
                $"Tuần: {report.WeekFrom:dd/MM/yyyy} - {report.WeekTo:dd/MM/yyyy}";

            ws.Cells[row, 1].Style.Font.Bold = true;

            row += 2;

            #endregion

            #region Header

            ws.Cells[row, 1].Value = "STT";
            ws.Cells[row, 2].Value = "Tên dự án";
            ws.Cells[row, 3].Value = "AM";
            ws.Cells[row, 4].Value = "Công việc gần nhất";
            ws.Cells[row, 5].Value = "Thông tin trao đổi gần nhất";

            using (var header = ws.Cells[row, 1, row, 5])
            {
                header.Style.Font.Bold = true;
                header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                header.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                header.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            row++;

            #endregion

            int stt = 1;

            foreach (var item in report.Items)
            {
                ws.Cells[row, 1].Value = stt++;
                ws.Cells[row, 2].Value = item.ProjectName;
                ws.Cells[row, 3].Value = item.AMNames;

                #region Công việc gần nhất

                string taskText = item.LastTaskName ?? "";

                if (item.LastTaskDate.HasValue)
                {
                    taskText += Environment.NewLine +
                                item.LastTaskDate.Value.ToString("dd/MM/yyyy HH:mm");
                }

                ws.Cells[row, 4].Value = taskText;

                #endregion

                #region Thông tin trao đổi

                string commentText = "";

                if (item.CommentItems != null)
                {
                    var comments = item.CommentItems
                        .OrderByDescending(x => x.CreatedDate)
                        .ToList();

                    foreach (var comment in comments)
                    {
                        commentText += $"{comment.TaskName} | " +
                                       $"{comment.CreatedDate:dd/MM/yyyy} | " +
                                       $"{comment.EmployeeName}";
                        commentText += Environment.NewLine;

                        commentText += HtmlToText(comment.Content);
                        commentText += "--------------------------------";
                        commentText += Environment.NewLine;
                    }
                }

                ws.Cells[row, 5].Value = commentText;
                ws.Cells[row, 5].Style.WrapText = true;

                #endregion

                #region Chèn ảnh từ HTML

                int imageTopOffset = 80;

                if (item.CommentItems != null)
                {
                    foreach (var comment in item.CommentItems)
                    {
                        var matches = Regex.Matches(
                            comment.Content ?? "",
                            "src=\"([^\"]+)\"",
                            RegexOptions.IgnoreCase);

                        foreach (Match match in matches)
                        {
                            try
                            {
                                string imageUrl = match.Groups[1].Value;

                                string physicalPath =
                                    HostingEnvironment.MapPath(imageUrl);

                                if (string.IsNullOrWhiteSpace(physicalPath))
                                    continue;

                                if (!System.IO.File.Exists(physicalPath))
                                    continue;

                                var picture = ws.Drawings.AddPicture(
                                    Guid.NewGuid().ToString(),
                                    new FileInfo(physicalPath));

                                picture.SetPosition(
                                    row - 1,
                                    imageTopOffset,
                                    4,
                                    5);

                                picture.SetSize(180, 120);

                                imageTopOffset += 130;
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                if (imageTopOffset > 80)
                {
                    ws.Row(row).Height = imageTopOffset;
                }
                else
                {
                    ws.Row(row).Height = 60;
                }

                #endregion

                row++;
            }

            #region Format

            using (var range = ws.Cells[5, 1, row - 1, 5])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.WrapText = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            ws.Column(1).Width = 8;
            ws.Column(2).Width = 35;
            ws.Column(3).Width = 35;
            ws.Column(4).Width = 30;
            ws.Column(5).Width = 70;

            #endregion
        }

        private string HtmlToText(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return "";

            html = html.Replace("<br>", "\n")
                       .Replace("<br/>", "\n")
                       .Replace("<br />", "\n")
                       .Replace("</p>", "\n");

            html = Regex.Replace(html, "<.*?>", "");

            return HttpUtility.HtmlDecode(html);
        }
    }
}
