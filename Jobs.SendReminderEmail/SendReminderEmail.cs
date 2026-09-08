using Jobs.SendReminderEmail;
using System;
using TSFramework.Libs.Models.Job;
using TSFramework.Libs.Providers;

namespace Jobs.SendReminderEmail
{
    [JobPlugin("SendReminderEmail",
        "Job tự động gửi email nhắc nhở cập nhật cơ hội, dự án")]
    public class SendReminderEmail : IJobPlugable
    {
        public string PluginName
        {
            get => "Job tự động gửi email nhắc nhở cập nhật cơ hội, dự án";
            set { }
        }

        public Type JobType { get; set; }

        public JobModel MainJob { get; set; }

        public void ExecuteJobNow(
            string sJobId,
            string sDescription,
            params object[] dataObjects)
        {
            var sPreTriggerId = JobProvider.GenerateTriggerId(5);
            var sPreGroupId = JobProvider.GenerateGroupId(6);

            var sTriggerId = $"{sPreTriggerId}-{sJobId}";
            var sGroupId = $"{sPreGroupId}-{sJobId}";

            var scheduleJob = new SendReminderEmailScheduleModel()
                .SetJobId(sJobId)
                .SetGroupId(sGroupId)
                .SetDescription(sDescription)
                .SetTriggerId(sTriggerId)
                .StartNow();

            scheduleJob.ExecuteNow(null);
        }

        public IJobPlugable BuildJob(
            string sJobId,
            string sDescription,
            string sCronExpression,
            params object[] dataObjects)
        {
            var sPreTriggerId = JobProvider.GenerateTriggerId(5);
            var sPreGroupId = JobProvider.GenerateGroupId(6);

            var sTriggerId = $"{sPreTriggerId}-{sJobId}";
            var sGroupId = $"{sPreGroupId}-{sJobId}";

            var scheduleJob = new SendReminderEmailScheduleModel()
                .SetJobId(sJobId)
                .SetGroupId(sGroupId)
                .SetDescription(sDescription)
                .SetTriggerId(sTriggerId)
                .WithCronSchedule(sCronExpression);

            return new SendReminderEmail
            {
                MainJob = scheduleJob,
                JobType = typeof(SendReminderEmailScheduleModel)
            };
        }
    }
}