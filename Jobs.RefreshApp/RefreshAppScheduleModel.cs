using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using Core.Sys.Caches.Sys;
using Newtonsoft.Json;
using Quartz;
using TSFramework.Libs.Models.Job;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Jobs.RefreshApp
{
    public class RefreshAppScheduleModel : JobModel
    {
        public override void Execute(IJobExecutionContext context)
        {
            var jobName = "Jobs.RefreshApp";

            try
            {
                SysJobCache jobCache = new SysJobCache();

                var jobModel = jobCache.GetById(context.JobDetail.Key.Name);
                NameValueCollection collectionParrams = null;
                if (!string.IsNullOrEmpty(jobModel.JobParrams) && UtilString.IsValidJson(jobModel.JobParrams))
                {
                    var dictParrams = JsonConvert.DeserializeObject<Dictionary<string, string>>(jobModel.JobParrams);
                    if (dictParrams != null)
                    {
                        collectionParrams = new NameValueCollection(dictParrams.Count);
                        foreach (var k in dictParrams) collectionParrams.Add(k.Key, k.Value);
                    }
                }

                if(collectionParrams == null) return;
                var hostUrl = collectionParrams["Host_Url"] ?? "";

                AppProcessor.JobLogger.Message(jobName, "========================================");
                AppProcessor.JobLogger.Message(jobName, "============Gọi Request tới app=========");
                AppProcessor.JobLogger.Message(jobName, "========================================");

                using (var client = new WebClient())
                {
                    client.DownloadString(hostUrl);
                }
            }
            catch (Exception ex)
            {
                AppProcessor.JobLogger.Error(jobName, ex);
            }
        }

        public override void ExecuteNow(object data)
        {
            var jobName = "Jobs.RefreshApp";
            try
            {
                if (data is NameValueCollection jobParrams)
                {
                    var hostUrl = jobParrams["Host_Url"] ?? "";

                    AppProcessor.JobLogger.Message(jobName, "=====================================================");
                    AppProcessor.JobLogger.Message(jobName, "============Gọi Request tới app - ExecuteNow=========");
                    AppProcessor.JobLogger.Message(jobName, "=====================================================");

                    using (var client = new WebClient())
                    {
                        client.DownloadString(hostUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                AppProcessor.JobLogger.Error(jobName, ex);
            }
        }
    }
}