using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Cate.Services;
using System;
using System.Collections.Generic;
using TSFramework.Libs.Processors;

namespace Core.Cate.Jobs
{
    /// <summary>
    /// Xử lý gửi reminder tới hạn cho kế hoạch cơ hội kinh doanh.
    /// </summary>
    public class OpportunityPlanReminderJob
    {
        private readonly RM_OpportunityPlanReminderCache _reminderCache;
        private readonly OpportunityPlanMailService _mailService;

        public OpportunityPlanReminderJob()
        {
            _reminderCache = new RM_OpportunityPlanReminderCache();
            _mailService = new OpportunityPlanMailService();
        }

        public void Execute()
        {
            try
            {
                var reminders = _reminderCache.GetPending() ?? new List<RM_OpportunityPlanReminderModel>();

                foreach (var reminder in reminders)
                {
                    _ProcessReminder(reminder);
                }
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }
        }

        private void _ProcessReminder(RM_OpportunityPlanReminderModel reminder)
        {
            try
            {
                switch (reminder.ReminderType)
                {
                    case "OneDayBefore":
                        _mailService.SendReminder1DayMail(reminder.OpportunityPlanID);
                        break;

                    case "ThirtyMinBefore":
                        _mailService.SendReminder30MinMail(reminder.OpportunityPlanID);
                        break;

                    default:
                        return;
                }

                _reminderCache.MarkSent(reminder.Id, "system");
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }
        }
    }
}
