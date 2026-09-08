using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Log.Models
{
    public class AccessHistoryModel
    {
        public int AccessHistory_ID { get; set; }
        public string Area { get; set; }
        public string Controller { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Action { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public int TotalRow {  get; set; }
    }
    public class AccessHistorySearchModel
    {
        public string TuKhoa { get; set; }
        public DateTime? TuNgay  { get; set; }
        public DateTime? DenNgay  { get; set; }
    }
}
