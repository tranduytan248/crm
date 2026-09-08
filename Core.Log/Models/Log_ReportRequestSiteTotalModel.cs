using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Log.Models
{
    public class Log_ReportRequestSiteTotalModel
    {
        public int OnlineNumber { get; set; }
        public int TodayNumber { get; set; }
        public int MonthNumber { get; set; }
        public int YearNumber { get; set; }
    }
}
