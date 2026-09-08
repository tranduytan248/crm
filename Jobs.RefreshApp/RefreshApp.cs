using System;
using System.Collections;
using System.Collections.Specialized;
using Quartz;
using TSFramework.Libs.Extensions;
using TSFramework.Libs.Models.Job;
using TSFramework.Libs.Providers;

namespace Jobs.RefreshApp
{
    [JobPlugin("RefreshApp", "Job tự động gọi request app")]
    public class RefreshApp : IJobPlugable
    {
        public string PluginName
        {
            get => "Job tự động gọi request app";
            set { }
        }

        public Type JobType { get; set; }

        public JobModel MainJob { get; set; }

        public void ExecuteJobNow(string sJobId, string sDescription, params object[] dataObjects)
        {
            var sPreTriggerId = JobProvider.GenerateTriggerId(5); // Can custom
            var sPreGroupId = JobProvider.GenerateGroupId(6); // Can custom
            var sTriggerId = $"{sPreTriggerId}-{sJobId}";
            var sGroupId = $"{sPreGroupId}-{sJobId}";

            var refreshAppScheduleJob = new RefreshAppScheduleModel()
                .SetJobId(sJobId)
                .SetGroupId(sGroupId)
                .SetDescription(sDescription)
                .SetTriggerId(sTriggerId)
                .StartNow();

            var dataJobContents = dataObjects?[0] as NameValueCollection;

            refreshAppScheduleJob.ExecuteNow(dataJobContents);
        }

        public IJobPlugable BuildJob(string sJobId, string sDescription, string sCronExpression,
            params object[] dataObjects)
        {
            var sPreTriggerId = JobProvider.GenerateTriggerId(5); // Can custom
            var sPreGroupId = JobProvider.GenerateGroupId(6); // Can custom
            var sTriggerId = $"{sPreTriggerId}-{sJobId}";
            var sGroupId = $"{sPreGroupId}-{sJobId}";

            var refreshAppScheduleJob = new RefreshAppScheduleModel()
                .SetJobId(sJobId)
                .SetGroupId(sGroupId)
                .SetDescription(sDescription)
                .SetTriggerId(sTriggerId)
                .WithCronSchedule(sCronExpression);

            var lstParrams = dataObjects?[0] as NameValueCollection;
            if (lstParrams == null)
                return new RefreshApp
                { MainJob = refreshAppScheduleJob, JobType = typeof(RefreshAppScheduleModel) };

            foreach (var pKey in lstParrams.AllKeys)
            {
                refreshAppScheduleJob.UsingJobData(pKey, lstParrams[pKey]);
            }

            refreshAppScheduleJob.JobData.PutAll(new JobDataMap((IDictionary)lstParrams.ToDictionary()));

            return new RefreshApp { MainJob = refreshAppScheduleJob, JobType = typeof(RefreshAppScheduleModel) };
        }
    }
}
