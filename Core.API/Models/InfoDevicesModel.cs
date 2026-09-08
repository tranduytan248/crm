using System;

namespace Core.API.Models
{
    public class InfoDevicesModel : APIModel
    {
        public int Device_ID { get; set; } = 0;
        public int Room_ID { get; set; } = 0;
        public string MAC_Address { get; set; }
        public string BranchDevice { get; set; }

    }
}