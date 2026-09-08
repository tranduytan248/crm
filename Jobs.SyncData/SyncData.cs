using Quartz;
using System;
using System.Collections;
using System.Collections.Specialized;
using TSFramework.Libs.Extensions;
using TSFramework.Libs.Models.Job;
using TSFramework.Libs.Providers;

namespace Jobs.SyncData
{
    [JobPlugin("SyncData", "Job đồng bộ dữ liệu từ HRM")]
    public class SyncData : IJobPlugable
    {
        public string PluginName
        {
            get => "Job đồng bộ dữ liệu từ HRM";
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

            var syncDataScheduleJob = new SyncDataScheduleModel()
                .SetJobId(sJobId)
                .SetGroupId(sGroupId)
                .SetDescription(sDescription)
                .SetTriggerId(sTriggerId)
                .StartNow();

            var dataJobContent = dataObjects?[0] as NameValueCollection;
            syncDataScheduleJob.ExecuteNow(dataJobContent);
        }

        public IJobPlugable BuildJob(string sJobId, string sDescription, string sCronExpression,
            params object[] dataObjects)
        {
            var sPreTriggerId = JobProvider.GenerateTriggerId(5); // Can custom
            var sPreGroupId = JobProvider.GenerateGroupId(6); // Can custom
            var sTriggerId = $"{sPreTriggerId}-{sJobId}";
            var sGroupId = $"{sPreGroupId}-{sJobId}";

            var syncDataScheduleJob = new SyncDataScheduleModel()
                .SetJobId(sJobId)
                .SetGroupId(sGroupId)
                .SetDescription(sDescription)
                .SetTriggerId(sTriggerId)
                .WithCronSchedule(sCronExpression);

            var lstParrams = dataObjects?[0] as NameValueCollection;
            if (lstParrams == null)
                return new SyncData
                { MainJob = syncDataScheduleJob, JobType = typeof(SyncDataScheduleModel) };

            foreach (var pKey in lstParrams.AllKeys)
            {
                syncDataScheduleJob.UsingJobData(pKey, lstParrams[pKey]);
            }

            syncDataScheduleJob.JobData.PutAll(new JobDataMap((IDictionary)lstParrams.ToDictionary()));

            return new SyncData { MainJob = syncDataScheduleJob, JobType = typeof(SyncDataScheduleModel) };
        }
    }
}
