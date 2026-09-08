using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using Core.Cate.Biz;
using Core.Cate.Services;
using Core.Sys.Biz.Sys;
using Quartz;
using TSFramework.Libs.Models.Job;
using TSFramework.Libs.Processors;

namespace Jobs.SyncData
{
    public class SyncDataScheduleModel : JobModel
    {
        readonly MN_BoPhanBiz _boPhanBiz = new MN_BoPhanBiz();
        readonly MN_EmployeeBiz _employeeBiz = new MN_EmployeeBiz();
        readonly MN_ChucVuBiz _chucVuBiz = new MN_ChucVuBiz();

        public override async void Execute(IJobExecutionContext context)
        {
            var jobName = "Jobs.SyncAll";

            try
            {
                DelOldLogFiles();

                AppProcessor.JobLogger.Message(jobName, "===== START SYNC ALL =====");

                var service = new SyncDataService();

                // 1. BoPhan
                var boPhanTable = await service.GetBoPhanAsync();
                var boPhanResult = _boPhanBiz.Sync(boPhanTable, "job-system");

                AppProcessor.JobLogger.Message(jobName, $"BoPhan: {boPhanResult}");

                // 2. ChucVu
                var chucVuTable = await service.GetChucVuAsync();
                var chucVuResult = _chucVuBiz.Sync(chucVuTable, "job-system");

                AppProcessor.JobLogger.Message(jobName, $"ChucVu: {chucVuResult}");

                // 3. User
                var userTable = await service.GetNhanVienAsync();
                var userResult = _employeeBiz.Sync(userTable, "job-system");

                AppProcessor.JobLogger.Message(jobName, $"User: {userResult}");

                AppProcessor.JobLogger.Message(jobName, "===== END SYNC ALL =====");
            }
            catch (Exception ex)
            {
                AppProcessor.JobLogger.Error(jobName, ex);
            }
        }

        private void DelOldLogFiles()
        {
            int daysAgo = 2;

            var relativePath = ConfigurationManager.AppSettings["JobLogPath"];

            if (string.IsNullOrEmpty(relativePath))
                return;

            var dirJobLogs = HostingEnvironment.MapPath("~/" + relativePath);

            if (string.IsNullOrEmpty(dirJobLogs) || !Directory.Exists(dirJobLogs))
                return;

            var jobLogFiles = Directory.GetFiles(dirJobLogs);

            foreach (var logFile in jobLogFiles)
            {
                var logFileInfo = new FileInfo(logFile);

                if (logFileInfo.CreationTime < DateTime.Now.AddDays(-daysAgo))
                {
                    logFileInfo.Delete();
                }
            }
        }
    }
}