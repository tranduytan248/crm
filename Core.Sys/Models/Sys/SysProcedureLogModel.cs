using System;
using TSFramework.Libs.Models.Base;

namespace Core.Sys.Models.Sys
{
    public class SysProcedureLogModel : BaseModel
    {
        public Guid LogId { get; set; }
        public DateTime LogDate { get; set; }
        public string ProcedureName { get; set; }
        public int ErrorLine { get; set; }
        public string ErrorMessage { get; set; }
        public string AdditionalInfo { get; set; }
    }
}