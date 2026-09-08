using System;

namespace Core.Sys.Models.Sys
{
    public class SysIPRequestModel
    {
        public string IP { get; set; }
        public bool IsLock { get; set; }
        public DateTime LastestRequest { get; set; }
    }
}