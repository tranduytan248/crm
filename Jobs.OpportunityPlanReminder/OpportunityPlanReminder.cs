using Quartz;
using System;
using System.Collections;
using System.Collections.Specialized;
using TSFramework.Libs.Extensions;
using TSFramework.Libs.Models.Job;
using TSFramework.Libs.Providers;

namespace Jobs.OpportunityPlanReminder
{
    [JobPlugin("OpportunityPlanReminder", "Job nhắc việc kế hoạch kinh doanh")]
    public class OpportunityPlanReminder : IJobPlugable
    {
        public string PluginName
        {
            get => "Job nhắc việc kế hoạch kinh doanh";
            set { }
        }

        public Type JobType { get; set; }

        public JobModel MainJob { get; set; }

        public void ExecuteJobNow(string sJobId, string sDescription, params object[] dataObjects)
        {
            var sPreTriggerId = JobProvider.GenerateTriggerId(5);
            var sPreGroupId = JobProvider.GenerateGroupId(6);
            var sTriggerId = $"{sPreTriggerId}-{sJobId}";
            var sGroupId = $"{sPreGroupId}-{sJobId}";

            var scheduleJob = new OpportunityPlanReminderScheduleModel()
                .SetJobId(sJobId)
                .SetGroupId(sGroupId)
                .SetDescription(sDescription)
                .SetTriggerId(sTriggerId)
                .StartNow();

            var dataJobContent = dataObjects?[0] as NameValueCollection;
            scheduleJob.ExecuteNow(dataJobContent);
        }

        public IJobPlugable BuildJob(string sJobId, string sDescription, string sCronExpression, params object[] dataObjects)
        {
            var sPreTriggerId = JobProvider.GenerateTriggerId(5);
            var sPreGroupId = JobProvider.GenerateGroupId(6);
            var sTriggerId = $"{sPreTriggerId}-{sJobId}";
            var sGroupId = $"{sPreGroupId}-{sJobId}";

            var scheduleJob = new OpportunityPlanReminderScheduleModel()
                .SetJobId(sJobId)
                .SetGroupId(sGroupId)
                .SetDescription(sDescription)
                .SetTriggerId(sTriggerId)
                .WithCronSchedule(sCronExpression);

            var lstParrams = dataObjects?[0] as NameValueCollection;
            if (lstParrams == null)
            {
                return new OpportunityPlanReminder
                {
                    MainJob = scheduleJob,
                    JobType = typeof(OpportunityPlanReminderScheduleModel)
                };
            }

            foreach (var pKey in lstParrams.AllKeys)
            {
                scheduleJob.UsingJobData(pKey, lstParrams[pKey]);
            }

            scheduleJob.JobData.PutAll(new JobDataMap((IDictionary)lstParrams.ToDictionary()));

            return new OpportunityPlanReminder
            {
                MainJob = scheduleJob,
                JobType = typeof(OpportunityPlanReminderScheduleModel)
            };
        }
    }
}
