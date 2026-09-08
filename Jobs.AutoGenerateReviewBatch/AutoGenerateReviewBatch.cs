using Jobs.ReviewBatch;
using System;
using TSFramework.Libs.Models.Job;
using TSFramework.Libs.Providers;

namespace Jobs.AutoGenerateReviewBatch
{
    [JobPlugin("AutoGenerateReviewBatch",
        "Job tự động tạo đợt rà soát")]
    public class AutoGenerateReviewBatch : IJobPlugable
    {
        public string PluginName
        {
            get => "Job tự động tạo đợt rà soát";
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

            var scheduleJob = new AutoGenerateReviewBatchScheduleModel()
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

            var scheduleJob = new AutoGenerateReviewBatchScheduleModel()
                .SetJobId(sJobId)
                .SetGroupId(sGroupId)
                .SetDescription(sDescription)
                .SetTriggerId(sTriggerId)
                .WithCronSchedule(sCronExpression);

            return new AutoGenerateReviewBatch
            {
                MainJob = scheduleJob,
                JobType = typeof(AutoGenerateReviewBatchScheduleModel)
            };
        }
    }
}