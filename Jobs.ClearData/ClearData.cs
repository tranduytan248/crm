using Quartz;
using System;
using System.Collections;
using System.Collections.Specialized;
using TSFramework.Libs.Extensions;
using TSFramework.Libs.Models.Job;
using TSFramework.Libs.Providers;

namespace Job.ClearData
{
    [JobPlugin("ClearData", "Job clear data")]
    public class ClearData : IJobPlugable
    {
        public string PluginName
        {
            get => "Job clear data";
            set { }
        }

        public Type JobType { get; set; }

        public JobModel MainJob { get; set; }

        public void ExecuteJobNow(string sJobId, string sDescription, params object[] dataObjects)
        {
            var sPreTriggerId = JobProvider.GenerateTriggerId(7); // Can custom
            var sPreGroupId = JobProvider.GenerateGroupId(8); // Can custom
            var sTriggerId = $"{sPreTriggerId}-{sJobId}";
            var sGroupId = $"{sPreGroupId}-{sJobId}";

            var refreshAppScheduleJob = new ClearDataModel()
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
            var sPreTriggerId = JobProvider.GenerateTriggerId(7); // Can custom
            var sPreGroupId = JobProvider.GenerateGroupId(8); // Can custom
            var sTriggerId = $"{sPreTriggerId}-{sJobId}";
            var sGroupId = $"{sPreGroupId}-{sJobId}";

            var refreshAppScheduleJob = new ClearDataModel()
                .SetJobId(sJobId)
                .SetGroupId(sGroupId)
                .SetDescription(sDescription)
                .SetTriggerId(sTriggerId)
                .WithCronSchedule(sCronExpression);

            var lstParrams = dataObjects?[0] as NameValueCollection;
            if (lstParrams == null)
                return new ClearData
                { MainJob = refreshAppScheduleJob, JobType = typeof(ClearDataModel) };

            foreach (var pKey in lstParrams.AllKeys)
            {
                refreshAppScheduleJob.UsingJobData(pKey, lstParrams[pKey]);
            }

            refreshAppScheduleJob.JobData.PutAll(new JobDataMap((IDictionary)lstParrams.AsDictionary()));

            return new ClearData { MainJob = refreshAppScheduleJob, JobType = typeof(ClearDataModel) };
        }
    }
}
