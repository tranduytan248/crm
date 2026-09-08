using Core.Cate.Jobs;
using Quartz;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Job;
using TSFramework.Libs.Processors;

namespace Jobs.OpportunityPlanReminder
{
    public class OpportunityPlanReminderScheduleModel : JobModel
    {
        private const string JobName = "Jobs.OpportunityPlanReminder";

        public override void Execute(IJobExecutionContext context)
        {
            Task.Factory.StartNew(RunReminderJob);
        }

        public override void ExecuteNow(object data)
        {
            Task.Factory.StartNew(RunReminderJob);
        }

        private void RunReminderJob()
        {
            try
            {
                AppProcessor.JobLogger.Message(JobName, "Bắt đầu quét reminder kế hoạch kinh doanh");

                var reminderJob = new OpportunityPlanReminderJob();
                reminderJob.Execute();

                AppProcessor.JobLogger.Message(JobName, "Hoàn thành quét reminder kế hoạch kinh doanh");
            }
            catch (Exception ex)
            {
                AppProcessor.JobLogger.Error(JobName, ex);
            }
        }
    }
}
