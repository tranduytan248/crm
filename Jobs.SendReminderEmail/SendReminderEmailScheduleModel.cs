using Core.Cate.Biz;
using Core.Cate.Models;
using Core.Cate.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Job;
using TSFramework.Libs.Processors;

namespace Jobs.SendReminderEmail
{
    /// <summary>
    /// Job tự động tạo đợt rà soát theo cấu hình
    /// </summary>
    public class SendReminderEmailScheduleModel : JobModel
    {
        private readonly ReminderMailBiz _reminderBiz = new ReminderMailBiz();
        private readonly ReminderMailService _mailService = new ReminderMailService();

        public override async void Execute(IJobExecutionContext context)
        {
            const string jobName = "Jobs.SendReminderEmail";

            try
            {
                AppProcessor.JobLogger.Message(jobName, "===== START SEND REMINDER EMAIL =====");

                DateTime today = DateTime.Today;

                // Thứ 2 đầu tuần
                DateTime fromDate = today.AddDays(-(int)today.DayOfWeek + 1);

                // Chủ nhật cuối tuần
                DateTime toDate = fromDate.AddDays(6);

                AppProcessor.JobLogger.Message(jobName,
                    $"FromDate={fromDate:dd/MM/yyyy}; ToDate={toDate:dd/MM/yyyy}");

                var list = _reminderBiz.GetReminderList(fromDate, toDate);

                foreach (var item in list)
                {
                    try
                    {
                        _mailService.QueueSendReminderMail(
                            new ReminderProjectOpportunityModel
                            {
                                ObjectID = item.ObjectID,
                                ObjectType = item.ObjectType,
                                ObjectName = item.ObjectName
                            },
                            item.UserNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

                        AppProcessor.JobLogger.Message(
                            jobName,
                            $"Queue reminder mail: ObjectId={item.ObjectID}, ObjectType={item.ObjectType}");
                    }
                    catch (Exception ex)
                    {
                        AppProcessor.JobLogger.Error(
                            jobName,
                            new Exception($"Send reminder failed. ObjectID={item.ObjectID}", ex));
                    }
                }

                AppProcessor.JobLogger.Message(jobName, "===== END SEND REMINDER EMAIL =====");
            }
            catch (Exception ex)
            {
                AppProcessor.JobLogger.Error(jobName, ex);
            }

            await Task.CompletedTask;
        }
    }
}