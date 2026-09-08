using System;

namespace Modules.API.Models.ReadingScheduleModel
{
    public class ReadingScheduleModel
    {
        public int year {  get; set; }
        public int month {  get; set; }
        public string readingDate {  get; set; }
    }
    public class ReadingScheduleRequest
    {
        public int ContractId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
