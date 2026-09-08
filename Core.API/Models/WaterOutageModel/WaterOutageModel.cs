using System;

namespace Modules.API.Models.WaterOutageModel
{
    public class WaterOutageModel
    {
        public int OutageId { get; set; }
        public int ContractId { get; set; }
        public string Area { get; set; }
        public string Reason { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string AffectedStreets { get; set; }
        public string Status { get; set; }
        public bool IsActived { get; set; } = false;
    }
}
