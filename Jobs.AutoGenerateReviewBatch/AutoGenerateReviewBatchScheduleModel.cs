using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.Caches.Sys;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Globalization;
using TSFramework.Libs.Models.Job;
using TSFramework.Libs.Processors;

namespace Jobs.ReviewBatch
{
    /// <summary>
    /// Job tự động tạo đợt rà soát theo cấu hình
    /// Config key:
    /// AUTO_GENERATE_REVIEWBATCH_CONFIG
    /// </summary>
    public class AutoGenerateReviewBatchScheduleModel : JobModel
    {
        private readonly RM_ReviewBatchCache _reviewBatchCache = new RM_ReviewBatchCache();
        private readonly SysConfigCache _configsCache = new SysConfigCache();

        public override async void Execute(IJobExecutionContext context)
        {
            var jobName = "Jobs.AutoGenerateReviewBatch";

            try
            {
                AppProcessor.JobLogger.Message(jobName, "===== START AUTO GENERATE REVIEW BATCH 123=====");

                #region Lấy config

                var configData = _configsCache.GetViaKey("AUTO_GENERATE_REVIEWBATCH_CONFIG");

                // Không tìm thấy config
                if (configData == null ||
                    string.IsNullOrWhiteSpace(configData.ConfigValue))
                {
                    AppProcessor.JobLogger.Message(jobName, "Config AUTO_GENERATE_REVIEWBATCH_CONFIG not found");

                    return;
                }

                JObject config;

                try
                {
                    config = JObject.Parse(configData.ConfigValue);
                }
                catch (Exception ex)
                {
                    AppProcessor.JobLogger.Error(jobName, new Exception("Config JSON invalid", ex));

                    return;
                }

                #endregion

                #region Đọc cấu hình

                // Có bật auto generate không
                var autoGenerate =
                    config.Value<bool?>("autoGenerate") ?? false;

                if (!autoGenerate)
                {
                    AppProcessor.JobLogger.Message(jobName, "Auto generate disabled");

                    return;
                }

                // Loại chu kỳ
                // DAY / WEEK / MONTH / QUARTER
                var cycleType = config.Value<string>("cycleType") ?? "MONTH";

                // Số lượng chu kỳ
                var cycleValue = config.Value<int?>("cycleValue") ?? 1;

                // Format mã đợt
                var batchCodeFormat = config.Value<string>("batchCodeFormat") ?? "REVIEW_{YYYY}";

                // Format tên đợt
                var batchNameFormat = config.Value<string>("batchNameFormat") ?? "Đợt rà soát {YYYY}";

                // Ngày chạy theo tháng
                var reviewDayOfMonth = config.Value<int?>("reviewDayOfMonth");

                // Ngày chạy theo tuần
                // 0 = Sunday
                // 1 = Monday
                var reviewDayOfWeek = config.Value<int?>("reviewDayOfWeek");
                AppProcessor.JobLogger.Message(jobName, $"cycleType ({cycleType})");
                #endregion

                // Giả lập ngày hiện tại từ cấu hình để test
                var fakeToday = config.Value<string>("fakeToday");

                // Nếu có cấu hình fakeToday thì parse, nếu không thì lấy ngày hiện tại
                var today = !string.IsNullOrWhiteSpace(fakeToday) ? DateTime.Parse(fakeToday) : DateTime.Today;

                string batchCode = string.Empty;
                string batchName = string.Empty;

                DateTime fromDate;
                DateTime toDate;

                #region Xử lý theo chu kỳ

                switch (cycleType.ToUpper())
                {
                    #region DAY

                    case "DAY":
                        fromDate = today;
                        toDate = today.AddDays(cycleValue - 1);

                        batchCode = batchCodeFormat
                            .Replace("{DD}", today.Day.ToString("00"))
                            .Replace("{MM}", today.Month.ToString("00"))
                            .Replace("{YYYY}", today.Year.ToString());

                        batchName = batchNameFormat
                            .Replace("{DD}", today.Day.ToString("00"))
                            .Replace("{MM}", today.Month.ToString("00"))
                            .Replace("{YYYY}", today.Year.ToString());

                        break;

                    #endregion

                    #region WEEK

                    case "WEEK":
                        // Nếu có cấu hình ngày chạy
                        if (reviewDayOfWeek.HasValue &&
                            (int)today.DayOfWeek != reviewDayOfWeek.Value)
                        {
                            AppProcessor.JobLogger.Message(jobName, $"Skip generate because today != reviewDayOfWeek ({reviewDayOfWeek})");

                            return;
                        }

                        var calendar =
                            CultureInfo.InvariantCulture.Calendar;

                        // Lấy tuần hiện tại
                        var week = calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

                        // Từ thứ 2
                        fromDate = today.AddDays(-(int)today.DayOfWeek + 1);

                        // Đến cuối chu kỳ tuần
                        toDate = fromDate.AddDays((7 * cycleValue) - 1);

                        batchCode = batchCodeFormat
                            .Replace("{WW}", week.ToString("00"))
                            .Replace("{YYYY}", today.Year.ToString());

                        batchName = batchNameFormat
                            .Replace("{WW}", week.ToString("00"))
                            .Replace("{YYYY}", today.Year.ToString());

                        break;

                    #endregion

                    #region MONTH

                    case "MONTH":
                        // Nếu có cấu hình ngày chạy
                        if (reviewDayOfMonth.HasValue && today.Day != reviewDayOfMonth.Value)
                        {
                            AppProcessor.JobLogger.Message(jobName, $"Skip generate because today != reviewDayOfWeek ({reviewDayOfWeek})");

                            return;
                        }

                        // Đầu tháng
                        fromDate = new DateTime(today.Year, today.Month, 1);

                        // Cuối chu kỳ tháng
                        toDate = fromDate.AddMonths(cycleValue).AddDays(-1);

                        batchCode = batchCodeFormat
                            .Replace("{MM}", today.Month.ToString("00"))
                            .Replace("{YYYY}", today.Year.ToString());

                        batchName = batchNameFormat
                            .Replace("{MM}", today.Month.ToString("00"))
                            .Replace("{YYYY}", today.Year.ToString());

                        break;

                    #endregion

                    #region QUARTER

                    case "QUARTER":
                        // Xác định quý hiện tại
                        var quarter = ((today.Month - 1) / 3) + 1;

                        // Tháng đầu quý
                        var quarterStartMonth = ((quarter - 1) * 3) + 1;

                        fromDate = new DateTime(today.Year, quarterStartMonth, 1);

                        // Cuối quý
                        toDate = fromDate.AddMonths(3 * cycleValue).AddDays(-1);

                        batchCode = batchCodeFormat
                            .Replace("{Q}", quarter.ToString())
                            .Replace("{YYYY}", today.Year.ToString());

                        batchName = batchNameFormat
                            .Replace("{Q}", quarter.ToString())
                            .Replace("{YYYY}", today.Year.ToString());

                        break;

                    #endregion

                    default:

                        AppProcessor.JobLogger.Message(jobName, $"Unsupported cycleType: {cycleType}");

                        return;
                }

                #endregion

                #region Log thông tin

                AppProcessor.JobLogger.Message(jobName, $"Generate BatchCode={batchCode}; From={fromDate:dd/MM/yyyy}; To={toDate:dd/MM/yyyy}");

                #endregion

                #region Tạo đợt rà soát

                var model = new RM_ReviewBatchModel
                {
                    BatchCode = batchCode,
                    BatchName = batchName,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                // Tạo đợt rà soát
                var result = _reviewBatchCache.Save(model, "SYSTEM");

                if (result > 0)
                {
                    AppProcessor.JobLogger.Message(jobName, $"Create review batch success: {batchCode} - ID={result}");
                }
                else
                {
                    AppProcessor.JobLogger.Message(jobName, $"Review batch already exists or save failed: {batchCode}");
                }

                #endregion

                AppProcessor.JobLogger.Message(jobName, "===== END AUTO GENERATE REVIEW BATCH =====");
            }
            catch (Exception ex)
            {
                AppProcessor.JobLogger.Error(jobName, ex);
            }

            await System.Threading.Tasks.Task.CompletedTask;
        }
    }
}