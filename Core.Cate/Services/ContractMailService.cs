using Core.Cate.Caches;
using Core.Cate.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Hosting;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;

namespace Core.Cate.Services
{
    public class ContractMailService
    {
        #region Declaration
        private const string ContractPaymentReminder = "MailTemplate_ContractPaymentReminder";
        #endregion

        private readonly RM_ProductProjectCache _productProjectCache;
        private readonly MailTemplateService _mailTemplateService;


        #region Constructor
        public ContractMailService()
        {
            _productProjectCache = new RM_ProductProjectCache();
            _mailTemplateService = new MailTemplateService();
        }
        #endregion

        public void QueueSendContractPaymentReminderMail(Job_ContractPaymentReminderModel model)
        {
            QueueMailWork(delegate
               {
                   SendContractPaymentReminderMail(model);
               });
        }


        public void SendContractPaymentReminderMail(Job_ContractPaymentReminderModel model)
        {

            if (!CanSend(model.AM_Email))
            {
                return;
            }

            // SysUserMailInfo actionUser = GetUserByUserName(actionUserName);
            // string actionByFullName = actionUser != null ? actionUser.FullName : actionUserName;

            Dictionary<string, object> templateData =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { "AM_FullName", model.AM_FullName ?? string.Empty },
                    { "ContractCode", model.ContractCode ?? string.Empty },
                    { "ContractName", model.ContractName ?? string.Empty },
                    { "CustomerName", model.CustomerName ?? string.Empty },
                    { "NameProduct", model.NameProduct ?? string.Empty },
                    { "ProjectName", model.ProjectName ?? string.Empty },
                };

            string jsonData = JsonConvert.SerializeObject(templateData);

            _mailTemplateService.SendByConfigKey(
                ContractPaymentReminder,
                model.AM_Email,
                jsonData,
                model.AM_FullName);
        }

        private bool CanSend(string email)
        {
            bool canSend = !string.IsNullOrWhiteSpace(email) && UtilString.IsValidEmail(email);
            return canSend;
        }

        private void QueueMailWork(Action action)
        {
            if (action == null)
            {
                return;
            }

            HostingEnvironment.QueueBackgroundWorkItem(delegate
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    AppProcessor.Logger.Error(ex);
                }
                finally
                {
                    // Không có xử lý bổ sung.
                }

                return Task.CompletedTask;
            });
        }


    }
}
