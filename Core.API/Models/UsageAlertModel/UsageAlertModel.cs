using System;

namespace Modules.API.Models.UsageAlertModel
{
    public class UsageAlertModel
    {
        public int ContractId { get; set; }
        public string CurrentMonth { get; set; }
        public string PreviousMoth { get; set; }
        
        public decimal CurrentUsage {  get; set; }
        public decimal PreviousUsage {  get; set; }
        public decimal ThresholdPercent { get; set; }
        public decimal ThresholdLimit { get; set; }
        public bool IsExceeded { get; set; }
    }
}
