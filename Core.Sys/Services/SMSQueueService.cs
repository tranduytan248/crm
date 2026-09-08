using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Hosting;
using TSFramework.Libs.Processors;

namespace Core.Services
{
    public class SMSQueueService
    {
        private readonly SMSService _smsService = new SMSService();

        /// <summary>
        /// Queue gửi SMS background theo template code.
        /// </summary>
        public void QueueSendByTemplateCode(string templateCode, List<string> phoneNumbers, string username, object model = null)
        {
            if (phoneNumbers == null || phoneNumbers.Count == 0)
                return;

            QueueSmsWork(() =>
            {
                _smsService.SendByTemplateCode(
                    templateCode,
                    phoneNumbers,
                    username,
                    model);
            });
        }

        /// <summary>
        /// Queue background work gửi SMS.
        /// </summary>
        private void QueueSmsWork(Action action)
        {
            if (action == null)
                return;

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    AppProcessor.Logger.Error(ex);
                }

                return Task.CompletedTask;
            });
        }
    }
}