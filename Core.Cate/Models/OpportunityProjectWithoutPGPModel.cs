using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class OpportunityProjectWithoutPGPModel : BaseModel
    {
        public string ReportID { get; set; }
        public string ReportName { get; set; }
        public string StatusName { get; set; }
        public string StatusClass { get; set; }
        public string ReportType { get; set; }
        public string AM { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class OpportunityProjectWithoutPGSearchPModel : BaseSearchModel
    {
        public int Year { get; set; }
        public string ReportType { get; set; }
    }
}
