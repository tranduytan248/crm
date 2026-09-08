using Core.Cate.Biz;
using Quartz;
using System;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Job;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Providers;

namespace Job.ClearData
{
    public class ClearDataModel : JobModel
    {
        private static PO_PaidInvoiceBiz _paidInvoiceBiz = new PO_PaidInvoiceBiz();
        public override void Execute(IJobExecutionContext context)
        {
            var jobName = "Jobs.ClearData";
            try
            {
                Task.Run(() => ClearData());
            }
            catch (Exception ex)
            {
                AppProcessor.JobLogger.Error(jobName, ex);
            }
        }

        public override void ExecuteNow(object data)
        {
            new LogProvider().Message("Bắt đầu chạy job ClearData.,...");
            var jobName = "Jobs.ClearData";
            try
            {
                Task.Run(() => ClearData());
            }
            catch (Exception ex)
            {
                AppProcessor.JobLogger.Error(jobName, ex);
            }
        }

        public void ClearData()
        {
            new LogProvider().Message("Gọi JOB cleardata...");

            try
            {
                var dataNhacViec = _paidInvoiceBiz.ClearDataQRCode();                
                new LogProvider().Message("Chạy thành công.");
            }
            catch (Exception e)
            {
                new LogProvider().Message($"Lỗi khi chạy job nhắc việc: {e.Message}");
                throw;
            }
        }



    }
}
